using System.Numerics;
using static Nemonuri.ByteChars.Extensions.GuardExtensions;
using Vp = Nemonuri.ByteChars.ByteCharTheory.ByteVectorPremise;
using Bp = Nemonuri.ByteChars.ByteCharTheory.BytePremise;
using Vs = Nemonuri.ByteChars.Internal.ByteVectorSizePremise;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars.Internal;

internal readonly struct ByteVectorSizePremise : IFixedSizePremise<ByteVectorSizePremise>
{
    public readonly int FixedSize => Vector<byte>.Count;
}

internal readonly struct StackLimitSizePremise : IFixedSizePremise<StackLimitSizePremise>
{
    public readonly int FixedSize => ByteStringConstants.ByteCharStackLimitSize;
}

internal static partial class ByteCharSpanTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetConstant(ReadOnlySpan<byte> bytes, out byte constant)
    {
        if (bytes.Length == 1)
        {
            constant = bytes[0];
            return true;
        }
        else
        {
            constant = default;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<byte> LoadVector(ReadOnlySpan<byte> chunk)
    {
#if NET8_0_OR_GREATER
        return Vector.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#else
        return Nemonuri.NetStandards.Numerics.VectorTheory.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void StoreVector(Span<byte> chunk, Vector<byte> vector)
    {
#if NET8_0_OR_GREATER
        vector.StoreUnsafe(ref MemoryMarshal.GetReference(chunk));
#else
        Nemonuri.NetStandards.Numerics.VectorTheory.StoreUnsafe(vector, ref MemoryMarshal.GetReference(chunk));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsProperToUseVector(ReadOnlySpan<byte> target) => Vector.IsHardwareAccelerated && target.Length >= Vs.GetFixedSize();

#if !NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<byte> GetVectorConstant(byte value) => (new Vp()).GetTemporaryConstant(value);
#endif

    internal static bool LessThanOrEqualAll(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
#if NET8_0_OR_GREATER
        if (TryGetConstant(right, out var rConstant))
        {
            var sr = Sls.SplitSpan(left);
            Span<byte> dest = stackalloc byte[Sls.GetFixedSize()];
            foreach (ReadOnlySpan<byte> chunk in sr.Chunks)
            {
                TensorPrimitives.Min(chunk, rConstant, dest);
                if (!chunk.SequenceEqual(dest)) {return false;}
            }
            if (sr.Remainder is { Length: > 0 } rmd)
            {
                dest = dest[..rmd.Length];
                TensorPrimitives.Min(rmd, rConstant, dest);
                if (!rmd.SequenceEqual(dest)) {return false;}
            }
            return true;
        }
        else
        {
            Guard.HasSizeEqualTo(left, right);

            var srL = Sls.SplitSpan(left);
            var srR = Sls.SplitSpan(right);
            Span<byte> dest = stackalloc byte[Sls.GetFixedSize()];

            var chunksL = srL.Chunks;
            var chunksR = srR.Chunks;
            for (int i = 0; i < chunksL.Length; i++)
            {
                var chunkL = chunksL[i];
                TensorPrimitives.Min(chunkL, chunksR[i], dest);
                if (!chunkL.SequenceEqual(dest)) {return false;}
            }
            if (srL.Remainder is { Length: > 0 } rmdL)
            {
                var rmdR = srR.Remainder;
                dest = dest[..rmdL.Length];
                TensorPrimitives.Min(rmdL, rmdR, dest);
                if (!rmdL.SequenceEqual(dest)) {return false;}
            }
            return true;
        }
#else
        if (TryGetConstant(right, out var rConstant))
        {
            static bool ByteFallback(ReadOnlySpan<byte> spanL, byte constR)
            {
                Bp bth = new();
                for (int i = 0; i < spanL.Length; i++)
                {
                    if (!bth.LessThanOrEqualAll(spanL[i], constR)) { return false; }
                }
                return true;
            }

            if (IsProperToUseVector(left))
            {
                Vp vth = new();
                var sr = Vs.SplitSpan(left);
                Vector<byte> vbR = GetVectorConstant(rConstant);
                foreach (ReadOnlySpan<byte> chunk in sr.Chunks)
                {
                    Vector<byte> vbL = LoadVector(chunk);
                    if (!vth.LessThanOrEqualAll(vbL, vbR)) { return false; };
                }
                return ByteFallback(sr.Remainder, rConstant);
            }
            else
            {
                return ByteFallback(left, rConstant);
            }
        }
        else
        {
            static bool ByteFallback(ReadOnlySpan<byte> spanL, ReadOnlySpan<byte> spanR)
            {
                Bp bth = new();
                for (int i = 0; i < spanL.Length; i++)
                {
                    if (!bth.LessThanOrEqualAll(spanL[i], spanR[i])) { return false; }
                }
                return true;
            }

            Guard.HasSizeEqualTo(left, right);

            if (IsProperToUseVector(left))
            {
                Vp vth = new();
                var srL = Vs.SplitSpan(left);
                var srR = Vs.SplitSpan(right);
                var chunksL = srL.Chunks;
                var chunksR = srR.Chunks;

                for (int i = 0; i < chunksL.Length; i++)
                {
                    var vecL = LoadVector(chunksL[i]);
                    var vecR = LoadVector(chunksR[i]);
                    if (!vth.LessThanOrEqualAll(vecL, vecR)) {return false;}
                }
                return ByteFallback(srL.Remainder, srR.Remainder);
            }
            else
            {
                return ByteFallback(left, right);
            }
        }
#endif
    }

    internal static bool EqualsAll(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        if (TryGetConstant(right, out var rConstant))
        {
#if NET8_0_OR_GREATER
            return left.IndexOfAnyExcept(rConstant) == -1;
#else
            var rs = Sls.SplitSpan(left);
            if (rs.Chunks is { Length: > 0 } chunks)
            {
                Span<byte> chunkR = stackalloc byte[Sls.GetFixedSize()];
                chunkR.Fill(rConstant);
                foreach (ReadOnlySpan<byte> chunkL in chunks)
                {
                    if (!chunkL.SequenceEqual(chunkR)) { return false; }
                }
                if (rs.Remainder is { Length: > 0 } rmdL)
                {
                    chunkR = chunkR[..rmdL.Length];
                    if (!rmdL.SequenceEqual(chunkR)) { return false; }
                }                
            }
            else
            {
                if (rs.Remainder is { Length: > 0 } rmdL)
                {
                    Span<byte> chunkR = stackalloc byte[rmdL.Length];
                    chunkR.Fill(rConstant);
                    if (!rmdL.SequenceEqual(chunkR)) { return false; }
                }   
            }

            return true;
#endif
        }
        else
        {
            Guard.HasSizeEqualTo(left, right);
            return left.SequenceEqual(right);
        }
    }

    internal static void Add(Span<byte> left, ReadOnlySpan<byte> right)
    {
#if NET8_0_OR_GREATER
        if (TryGetConstant(right, out var rConstant))
        {
            TensorPrimitives.Add(left, rConstant, left);
        }
        else
        {
            TensorPrimitives.Add(left, right, left);
        }
#else
        unsafe
        {
            static byte ByteOp(byte l, byte r) => new Bp().Add(l,r);
            static Vector<byte> VectorOp(Vector<byte> l, Vector<byte> r) => new Vp().Add(l,r);

            UnsafeByteSpanUpdate(left, right, &ByteOp, &VectorOp);            
        }
#endif
    }

    internal static void Subtract(Span<byte> left, ReadOnlySpan<byte> right)
    {
#if NET8_0_OR_GREATER
        if (TryGetConstant(right, out var rConstant))
        {
            TensorPrimitives.Subtract(left, rConstant, left);
        }
        else
        {
            TensorPrimitives.Subtract(left, right, left);
        }
#else
        unsafe
        {
            static byte ByteOp(byte l, byte r) => new Bp().Subtract(l,r);
            static Vector<byte> VectorOp(Vector<byte> l, Vector<byte> r) => new Vp().Subtract(l,r);

            UnsafeByteSpanUpdate(left, right, &ByteOp, &VectorOp);            
        }
#endif
    }

    internal static void Modulus(Span<byte> left, ReadOnlySpan<byte> right)
    {
#if NET8_0_OR_GREATER
        if (TryGetConstant(right, out var rConstant))
        {
            TensorPrimitives.Remainder(left, rConstant, left);
        }
        else
        {
            TensorPrimitives.Remainder(left, right, left);
        }
#else
        unsafe
        {
            static byte ByteOp(byte l, byte r) => new Bp().Modulus(l,r);
            static Vector<byte> VectorOp(Vector<byte> l, Vector<byte> r) => new Vp().Modulus(l,r);

            UnsafeByteSpanUpdate(left, right, &ByteOp, &VectorOp);            
        }
#endif
    }

    internal static bool TryUnsafeDecomposeToByteSpan(Span<byte> composed, out Span<byte> unsafeBytes)
    {
        unsafeBytes = composed;
        return true;
    }


    private static readonly PersistedPinnedArray<byte> s_tempConstants = new (ByteCharConstants.CodePageSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (byte[] Buffer, int Index) GetTemporaryConstantLocation(byte value)
    {
        var buffer = s_tempConstants.Values;
        buffer[value] = value;
        return (buffer, value);
    }

    internal static ReadOnlySpan<byte> GetTemporaryConstant(byte value)
    {
        (byte[] b, int i) = GetTemporaryConstantLocation(value);
        return new(b, i, 1);
    }


}

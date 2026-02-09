using System.Numerics;
using CommunityToolkit.HighPerformance;
using Vp = Nemonuri.OCamlDotNet.ByteCharTheory.ByteVectorPremise;
using Bp = Nemonuri.OCamlDotNet.ByteCharTheory.BytePremise;
using Vs = Nemonuri.OCamlDotNet.ByteCharTheory.ByteVectorSizePremise;
using Uis = Nemonuri.OCamlDotNet.ByteCharTheory.UIntPtrSizePremise;
using Sls = Nemonuri.OCamlDotNet.ByteCharTheory.StackLimitSizePremise;

namespace Nemonuri.OCamlDotNet;

public static partial class ByteCharTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetConstant(Span<byte> bytes, out byte constant)
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

    internal readonly struct ByteVectorSizePremise : IFixedSizePremise<ByteVectorSizePremise>
    {
        public readonly int FixedSize => Vector<byte>.Count;
    }

    internal readonly struct UIntPtrSizePremise : IFixedSizePremise<UIntPtrSizePremise>
    {
        public readonly int FixedSize => UIntPtr.Size;
    }

    internal readonly struct StackLimitSizePremise : IFixedSizePremise<StackLimitSizePremise>
    {
        private const int StackLimitSize = 256; // 이 정도면 적당한가?

        public readonly int FixedSize => StackLimitSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<byte> LoadVector(Span<byte> chunk)
    {
#if NET8_0_OR_GREATER
        return Vector.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#else
        return Nemonuri.NetStandards.Numerics.VectorTheory.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#endif
    }

    private static void StoreVector(Span<byte> chunk, Vector<byte> vector)
    {
#if NET8_0_OR_GREATER
        vector.StoreUnsafe(ref MemoryMarshal.GetReference(chunk));
#else
        Nemonuri.NetStandards.Numerics.VectorTheory.StoreUnsafe(vector, ref MemoryMarshal.GetReference(chunk));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsProperToUseVector(Span<byte> target) => Vector.IsHardwareAccelerated && target.Length >= Vs.GetFixedSize();

    internal static bool LessThanOrEqualAll(Span<byte> left, Span<byte> right)
    {
#if NET8_0_OR_GREATER
        if (TryGetConstant(right, out var rConstant))
        {
            var sr = Sls.SplitSpan(left);
            Span<byte> dest = stackalloc byte[Sls.GetFixedSize()];
            foreach (Span<byte> chunk in sr.Chunks)
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
            static bool ByteFallback(Span<byte> spanL, byte constR)
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
                Vector<byte> vbR = new(rConstant);
                foreach (Span<byte> chunk in sr.Chunks)
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
            static bool ByteFallback(Span<byte> spanL, Span<byte> spanR)
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

    internal static bool EqualsAll(Span<byte> left, Span<byte> right)
    {
        if (TryGetConstant(right, out var rConstant))
        {
#if NET8_0_OR_GREATER
            return left.IndexOfAnyExcept(rConstant) == -1;
#else
            Span<byte> chunkR = stackalloc byte[Sls.GetFixedSize()];
            chunkR.Fill(rConstant);
            var rs = Sls.SplitSpan(left);
            foreach (Span<byte> chunkL in rs.Chunks)
            {
                if (!chunkL.SequenceEqual(chunkR)) { return false; }
            }
            if (rs.Remainder is { Length: > 0 } rmdL)
            {
                chunkR = chunkR[..rmdL.Length];
                if (!rmdL.SequenceEqual(chunkR)) { return false; }
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

    internal static Span<byte> Add(Span<byte> left, Span<byte> right)
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
        return left;
    }

    internal static Span<byte> Subtract(Span<byte> left, Span<byte> right)
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
        return left;
    }

    internal static Span<byte> Modulus(Span<byte> left, Span<byte> right)
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
        return left;
    }

    internal static bool TryUnsafeDecomposeToByteSpan(Span<byte> composed, out Span<byte> unsafeBytes)
    {
        unsafeBytes = composed;
        return true;
    }


    private static readonly byte[] s_temporaryConstantsBuffer = new byte[byte.MaxValue+1];

    internal static Span<byte> GetTemporaryConstant(byte value)
    {
        Span<byte> singletonSpan = new(s_temporaryConstantsBuffer, value, 1);
        singletonSpan[0] = value;
        return singletonSpan;
    }
}
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
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        return new Vector<byte>(chunk);
#else
        return Nemonuri.NetStandards.Numerics.VectorTheory.Create(chunk);
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
            static bool SoftwareFallback(Span<byte> spanL, byte constR)
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
                return SoftwareFallback(sr.Remainder, rConstant);
            }
            else
            {
                return SoftwareFallback(left, rConstant);
            }
        }
        else
        {
            static bool SoftwareFallback(Span<byte> spanL, Span<byte> spanR)
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
                return SoftwareFallback(srL.Remainder, srR.Remainder);
            }
            else
            {
                return SoftwareFallback(left, right);
            }
        }
#endif
    }

#if false
    internal static bool EqualsAll(Span<byte> left, Span<byte> right)
    {
        if (TryGetConstant(right, out var rConstant))
        {
#if NET8_0_OR_GREATER
            return left.IndexOfAnyExcept(rConstant) == -1;
#else
            (int vectorLength, int byteLength) = SpanAndVectorDivRem(left);
            Span<byte> remainedLeft = left[^byteLength..];
            Vp vth = new();
            Bp bth = new();

            Vector<byte> rightVector = new(rConstant);
            
            for (int vi = 0; vi < vectorLength; vi++)
            {
                Vector<byte> leftVector = LoadVector(left, vi);
                if (!vth.EqualsAll(leftVector, rightVector)) {return false;}
            }

            for (int bi = 0; bi < remainedLeft.Length; bi++)
            {
                if (!bth.EqualsAll(remainedLeft[bi], rConstant)) {return false;}
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
            // Guard.HasSizeEqualTo(left, right);
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
#endif
        return left;
    }
#endif

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
using System.Numerics;
using CommunityToolkit.HighPerformance;
using Vp = Nemonuri.OCamlDotNet.ByteCharTheory.ByteVectorPremise;
using Bp = Nemonuri.OCamlDotNet.ByteCharTheory.BytePremise;

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

    private static int VectorByteCount 
    { 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Vector<byte>.Count; 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int VectorLength, int RemainedByteLength) SpanAndVectorDivRem(Span<byte> left)
    {
#if NET8_0_OR_GREATER
        return Math.DivRem(left.Length, VectorByteCount);
#else
        int q = Math.DivRem(left.Length, VectorByteCount, out int r);
        return (q, r);        
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int VectorIndexToSpanIndex(int vectorIndex) => vectorIndex * VectorByteCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector<byte> SpanAndVectorIndexToVector(Span<byte> left, int vectorIndex)
    {
        int spanIndex = VectorIndexToSpanIndex(vectorIndex);

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        return new Vector<byte>(left.Slice(spanIndex));
#else
        var rentedBuffer = ArrayPool<byte>.Shared.Rent(VectorByteCount);
        var splicedLeft = left.Slice(spanIndex, VectorByteCount);
        splicedLeft.CopyTo(rentedBuffer);
        Vector<byte> result = new(rentedBuffer);
        ArrayPool<byte>.Shared.Return(rentedBuffer);
        return result;        
#endif
    }

    internal static bool LessThanOrEqualAll(Span<byte> left, Span<byte> right)
    {
        (int vectorLength, int byteLength) = SpanAndVectorDivRem(left);
        Span<byte> remainedLeft = left[^byteLength..];
        Vp vth = new();
        Bp bth = new();

        if (TryGetConstant(right, out var rConstant))
        {
            Vector<byte> rightVector = new(rConstant);
            
            for (int vi = 0; vi < vectorLength; vi++)
            {
                Vector<byte> leftVector = SpanAndVectorIndexToVector(left, vi);
                if (!vth.LessThanOrEqualAll(leftVector, rightVector)) {return false;}
            }

            for (int bi = 0; bi < remainedLeft.Length; bi++)
            {
                if (!bth.LessThanOrEqualAll(remainedLeft[bi], rConstant)) {return false;}
            }

            return true;
        }
        else
        {
            Guard.HasSizeEqualTo(left, right);

            for (int vi = 0; vi < vectorLength; vi++)
            {
                Vector<byte> leftVector = SpanAndVectorIndexToVector(left, vi);
                Vector<byte> rightVector = SpanAndVectorIndexToVector(right, vi);
                if (!vth.LessThanOrEqualAll(leftVector, rightVector)) {return false;}
            }

            Span<byte> remainedRight = right[^byteLength..];
            for (int bi = 0; bi < remainedLeft.Length; bi++)
            {
                if (!bth.LessThanOrEqualAll(remainedLeft[bi], remainedRight[bi])) {return false;}
            }

            return true;
        }
    }

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
                Vector<byte> leftVector = SpanAndVectorIndexToVector(left, vi);
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
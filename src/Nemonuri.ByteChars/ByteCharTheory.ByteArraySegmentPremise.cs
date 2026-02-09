using Nemonuri.ByteChars.Internal;

namespace Nemonuri.ByteChars;

public static partial class ByteCharTheory
{
    public readonly struct ByteArraySegmentPremise : IByteCharPremise<ByteArraySegmentPremise, ArraySegment<byte>>
    {
        public bool LessThanOrEqualAll(ArraySegment<byte> left, ArraySegment<byte> right) => ByteCharSpanTheory.LessThanOrEqualAll(left, right);

        public bool EqualsAll(ArraySegment<byte> left, ArraySegment<byte> right) => ByteCharSpanTheory.EqualsAll(left, right);

        public ArraySegment<byte> Add(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            ByteCharSpanTheory.Add(left, right);
            return left;
        }

        public ArraySegment<byte> Subtract(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            ByteCharSpanTheory.Subtract(left, right);
            return left;
        }

        public ArraySegment<byte> Modulus(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            ByteCharSpanTheory.Modulus(left, right);
            return left;
        }

        public bool TryUnsafeDecomposeToByteSpan(ArraySegment<byte> composed, out Span<byte> unsafeBytes) => ByteCharSpanTheory.TryUnsafeDecomposeToByteSpan(composed, out unsafeBytes);
            

        public ArraySegment<byte> GetTemporaryConstant(byte value)
        {
            var (b, i) = ByteCharSpanTheory.GetTemporaryConstantLocation(value);
            return new(b, i, 1);
        }
    }
}

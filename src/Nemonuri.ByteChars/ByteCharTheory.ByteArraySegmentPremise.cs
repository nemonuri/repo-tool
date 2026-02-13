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

        public ArraySegment<byte> GetTemporaryConstant(byte value)
        {
            var (b, i) = ByteCharSpanTheory.GetTemporaryConstantLocation(value);
            return new(b, i, 1);
        }

        public bool TryDecomposeToReadOnlyByteSpan(ArraySegment<byte> source, out ReadOnlySpan<byte> readOnlyByteSpan)
        {
            readOnlyByteSpan = source;
            return true;
        }

        public bool TryDecomposeToByteSpan(ArraySegment<byte> source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux)
        {
            byteSpan = source;
            aux = null;
            return true;
        }

        public unsafe delegate*<ReadOnlySpan<byte>, object?, ArraySegment<byte>> ComposeFromByteSpan => null;
    }
}

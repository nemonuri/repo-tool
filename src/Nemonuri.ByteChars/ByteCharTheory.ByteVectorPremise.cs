using System.Buffers;
using System.Numerics;
using Nemonuri.ByteChars.Internal;
using Vs = Nemonuri.ByteChars.Internal.ByteVectorSizePremise;

namespace Nemonuri.ByteChars;


public static partial class ByteCharTheory
{
    public readonly struct ByteVectorPremise : IByteCharPremise<ByteVectorPremise, Vector<byte>>
    {
        public bool LessThanOrEqualAll(Vector<byte> left, Vector<byte> right) => Vector.LessThanOrEqualAll(left, right);

        public bool EqualsAll(Vector<byte> left, Vector<byte> right) => Vector.EqualsAll(left, right);

        public Vector<byte> Add(Vector<byte> left, Vector<byte> right) => Vector.Add(left, right);

        public Vector<byte> Subtract(Vector<byte> left, Vector<byte> right) => Vector.Subtract(left, right);

        public Vector<byte> Modulus(Vector<byte> left, Vector<byte> right)
        {
            var quotient = Vector.Divide(left, right);
            var remainder = Vector.Subtract(left, Vector.Multiply(quotient, right));
            return remainder;
        }

        private static int SpanSize => Vs.GetFixedSize();

        public bool TryDecomposeToReadOnlyByteSpan(Vector<byte> source, out ReadOnlySpan<byte> readOnlyByteSpan)
        {
            byte[] spanSource = new byte[SpanSize]; // allocation
            source.CopyTo(spanSource);
            readOnlyByteSpan = spanSource;
            return true;
        }

        public bool TryDecomposeToByteSpan(Vector<byte> source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux)
        {
            byte[] spanSource = ArrayPool<byte>.Shared.Rent(SpanSize);
            source.CopyTo(spanSource);
            byteSpan = spanSource.AsSpan()[..SpanSize];
            aux = spanSource;
            return true;
        }

        private static Vector<byte> ComposeFromByteSpanImpl(ReadOnlySpan<byte> span, object? aux)
        {
            byte[]? spanSourceFromAux = aux as byte[];
            Guard.IsNotNull(spanSourceFromAux);
            Vector<byte> resultVector = ByteVectorTheory.LoadVector(span);
            ArrayPool<byte>.Shared.Return(spanSourceFromAux);
            return resultVector;
        }

        public unsafe delegate*<ReadOnlySpan<byte>, object?, Vector<byte>> ComposeFromByteSpan => &ComposeFromByteSpanImpl;

        public Vector<byte> GetTemporaryConstant(byte value) => new (value);
    }
}

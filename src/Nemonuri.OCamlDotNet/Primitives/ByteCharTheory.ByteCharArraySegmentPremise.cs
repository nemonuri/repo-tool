namespace Nemonuri.OCamlDotNet;

public static partial class ByteCharTheory
{
    public readonly struct ByteCharArraySegmentPremise : IByteCharPremise<ByteCharArraySegmentPremise, ArraySegment<byte>>
    {
        public bool LessThanOrEqualAll(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            throw new NotImplementedException();
        }

        public bool EqualsAll(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> Add(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> Subtract(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> Modulus(ArraySegment<byte> left, ArraySegment<byte> right)
        {
            throw new NotImplementedException();
        }

        public bool TryUnsafeDecomposeToByteSpan(ArraySegment<byte> composed, out Span<byte> unsafeBytes)
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> GetTemporaryConstant(byte value)
        {
            throw new NotImplementedException();
        }
    }
}

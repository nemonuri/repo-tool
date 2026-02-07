namespace Nemonuri.OCamlDotNet;

public static partial class ByteCharTheory
{
    public readonly struct ByteCharSpanPremise : IByteCharPremise<ByteCharSpanPremise, UnsafePinnedSpanPointer<byte>>
    {
        public bool LessThanOrEqualAll(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            throw new NotImplementedException();
        }

        public bool EqualsAll(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            throw new NotImplementedException();
        }

        public UnsafePinnedSpanPointer<byte> Add(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            throw new NotImplementedException();
        }

        public UnsafePinnedSpanPointer<byte> Subtract(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            throw new NotImplementedException();
        }

        public UnsafePinnedSpanPointer<byte> Modulus(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            throw new NotImplementedException();
        }

        public bool TryUnsafeDecomposeToByteSpan(UnsafePinnedSpanPointer<byte> composed, out Span<byte> unsafeBytes)
        {
            throw new NotImplementedException();
        }

        public UnsafePinnedSpanPointer<byte> GetTemporaryConstant(byte value)
        {
            throw new NotImplementedException();
        }
    }
}

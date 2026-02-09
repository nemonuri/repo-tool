namespace Nemonuri.OCamlDotNet;

public static partial class ByteCharTheory
{
    public readonly struct BytePremise : IByteCharPremise<BytePremise, byte>
    {
        public bool LessThanOrEqualAll(byte left, byte right) => left <= right;

        public bool EqualsAll(byte left, byte right) => left == right;

        public byte Add(byte left, byte right) => unchecked((byte)(left + right));

        public byte Subtract(byte left, byte right) => unchecked((byte)(left - right));

        public byte Modulus(byte left, byte right) => unchecked((byte)(left % right));

        public bool TryUnsafeDecomposeToByteSpan(byte composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = default;
            return false;
        }

        public byte GetTemporaryConstant(byte value) => value;
    }
}

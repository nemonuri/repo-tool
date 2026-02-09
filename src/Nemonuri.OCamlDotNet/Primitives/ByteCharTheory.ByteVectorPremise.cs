namespace Nemonuri.OCamlDotNet;

using System.Numerics;

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

        public bool TryUnsafeDecomposeToByteSpan(Vector<byte> composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = default;
            return false;
        }

        public Vector<byte> GetTemporaryConstant(byte value) => new (value);
    }
}

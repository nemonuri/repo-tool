namespace Nemonuri.OCamlDotNet;

using System.Numerics;

public static class ByteCharOperationTheory
{
    public readonly struct BytePremise : IByteCharOperationPremise<BytePremise, byte>
    {
        public bool LessThanOrEqualAll(byte left, byte right) => left <= right;

        public bool EqualsAll(byte left, byte right) => left == right;

        public byte AddAll(byte left, byte right) => (byte)(left + right);

        public byte SubtractAll(byte left, byte right) => (byte)(left - right);

        public byte ModulusAll(byte left, byte right) => (byte)(left % right);

        public bool TryGetUnsafeDecompositionPremise<TDecomposed>(out UnsafeDecompositionPremise<byte, TDecomposed> premise)
        {
            premise = default; return false;
        }
    }

    public readonly struct ByteVectorPremise : IByteCharOperationPremise<ByteVectorPremise, Vector<byte>>
    {
        public bool LessThanOrEqualAll(Vector<byte> left, Vector<byte> right)
        {
            return Vector.LessThanOrEqualAll(left, right);
        }

        public bool EqualsAll(Vector<byte> left, Vector<byte> right)
        {
            return Vector.EqualsAll(left, right);
        }

        public Vector<byte> AddAll(Vector<byte> left, Vector<byte> right)
        {
            return Vector.Add(left, right);
        }

        public Vector<byte> SubtractAll(Vector<byte> left, Vector<byte> right)
        {
            return Vector.Subtract(left, right);
        }

        public Vector<byte> ModulusAll(Vector<byte> left, Vector<byte> right)
        {
            var quotient = Vector.Divide(left, right);
            var remainder = Vector.Subtract(left, Vector.Multiply(quotient, right));
            return remainder;
        }

        public unsafe bool TryGetUnsafeDecompositionPremise<TDecomposed>(out UnsafeDecompositionPremise<Vector<byte>, TDecomposed> premise)
        {
            if (typeof(TDecomposed) == typeof(byte))
            {
                static int GetLengthImpl(ref Vector<byte> bytes) => Vector<byte>.Count;
                static ref byte GetItemRef(ref Vector<byte> bytes, int index) => bytes.

                premise = new
                (
                    getLength: &GetLengthImpl,
                    getItemRef: 
                )
            }
            else
            {
                premise = default;
                return false;
            }
        }
    }
}

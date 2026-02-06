namespace Nemonuri.OCamlDotNet;

using System.Numerics;
using System.Runtime.CompilerServices;

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

    public static bool LessThanOrEqualAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.LessThanOrEqualAll(left, right);
    }

    public static bool EqualsAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.EqualsAll(left, right);
    }

    public static Vector<byte> AddAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.Add(left, right);
    }

    public static Vector<byte> SubtractAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.Subtract(left, right);
    }

    public static Vector<byte> ModulusAll(Vector<byte> left, Vector<byte> right)
    {
        var quotient = Vector.Divide(left, right);
        var remainder = Vector.Subtract(left, Vector.Multiply(quotient, right));
        return remainder;
    }

    public readonly struct ByteVectorPremise : IByteCharOperationPremise<ByteVectorPremise, UnsafePinnedVectorPointer<byte>>
    {
        public bool LessThanOrEqualAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
            ByteCharOperationTheory.LessThanOrEqualAll(left.LoadVector(), right.LoadVector());

        public bool EqualsAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
            ByteCharOperationTheory.EqualsAll(left.LoadVector(), right.LoadVector());

        public UnsafePinnedVectorPointer<byte> AddAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.AddAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> SubtractAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.SubtractAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> ModulusAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.ModulusAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public unsafe bool TryGetUnsafeDecompositionPremise<TDecomposed>(out UnsafeDecompositionPremise<UnsafePinnedVectorPointer<byte>, TDecomposed> premise)
        {
            if (typeof(TDecomposed) == typeof(byte))
            {
                static int GetLengthImpl(ref UnsafePinnedVectorPointer<byte> p) => Vector<byte>.Count;
                static ref TDecomposed GetItemRef(ref UnsafePinnedVectorPointer<byte> p, int index) => 
                    ref Unsafe.As<byte, TDecomposed>(ref Unsafe.AddByteOffset(ref Unsafe.AsRef<byte>(p.Pointer), index));

                premise = new (getLength: &GetLengthImpl, getItemRef: &GetItemRef);
                return true;
            }
            else
            {
                premise = default;
                return false;
            }
        }
    }
}

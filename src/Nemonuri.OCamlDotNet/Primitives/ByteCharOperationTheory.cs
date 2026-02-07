namespace Nemonuri.OCamlDotNet;

using System.Numerics;
using System.Runtime.CompilerServices;

public static partial class ByteCharOperationTheory
{
    public readonly struct BytePremise : IByteCharOperationPremise<BytePremise, byte>
    {
        public bool LessThanOrEqualAll(byte left, byte right) => left <= right;

        //public bool LessThanOrEqualAny(byte left, byte right) => LessThanOrEqualAll(left, right);

        public bool EqualsAll(byte left, byte right) => left == right;

        //public bool EqualsAny(byte left, byte right) => EqualsAll(left, right);

        public byte Add(byte left, byte right) => unchecked((byte)(left + right));

        public byte Subtract(byte left, byte right) => unchecked((byte)(left - right));

        public byte Modulus(byte left, byte right) => unchecked((byte)(left % right));

        public bool TryUnsafeDecomposeToByteSpan(byte composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = default;
            return false;
        }

        public byte GetConstant(byte value) => value;
    }

    public static bool LessThanOrEqualAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.LessThanOrEqualAll(left, right);
    }

/*
    public static bool LessThanOrEqualAny(Vector<byte> left, Vector<byte> right)
    {
        return Vector.LessThanOrEqualAny(left, right);
    }
*/


    public static bool EqualsAll(Vector<byte> left, Vector<byte> right)
    {
        return Vector.EqualsAll(left, right);
    }

/*
    public static bool EqualsAny(Vector<byte> left, Vector<byte> right)
    {
        return Vector.EqualsAny(left, right);
    }
*/

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

        //public bool LessThanOrEqualAny(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
        //    ByteCharOperationTheory.LessThanOrEqualAny(left.LoadVector(), right.LoadVector());

        public bool EqualsAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
            ByteCharOperationTheory.EqualsAll(left.LoadVector(), right.LoadVector());

        //public bool EqualsAny(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
        //    ByteCharOperationTheory.EqualsAny(left.LoadVector(), right.LoadVector());

        public UnsafePinnedVectorPointer<byte> Add(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.AddAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> Subtract(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.SubtractAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> Modulus(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharOperationTheory.ModulusAll(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

/*
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
*/

        public unsafe bool TryUnsafeDecomposeToByteSpan(UnsafePinnedVectorPointer<byte> composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = new Span<byte>(composed.Pointer, composed.SpanLength);
            return true;
        }

        private const int s_constantsLength0 = byte.MaxValue;
        private readonly static uint s_constantsLength1 = (uint)Vector<byte>.Count;
        private readonly static byte[] s_constants = CreatePinnedTable();

        private static byte[] CreatePinnedTable()
        {
            var length = s_constantsLength0 * s_constantsLength1;
#if NET8_0_OR_GREATER
            return GC.AllocateArray<byte>((int)length, pinned: true);
#else
            var table = new byte[length];
            GCHandle handle = GCHandle.Alloc(table, GCHandleType.Pinned);
            return table;
#endif
        }

        public unsafe UnsafePinnedVectorPointer<byte> GetConstant(byte value)
        {
            fixed (byte* pRow = &s_constants[value * s_constantsLength1])
            {
                if (value != 0)
                {
                    // Check the table row is initialized.
                    if (!(*pRow == value))
                    {
                        // If not, initialize.
                        Unsafe.InitBlock(pRow, value, s_constantsLength1);
                    }
                }

                return new (pRow);
            }
        }
    }
}

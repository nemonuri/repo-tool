namespace Nemonuri.OCamlDotNet;

using System.Numerics;
using System.Runtime.CompilerServices;

public static partial class ByteCharTheory
{
    public readonly struct ByteVectorPinPremise : IByteCharPremise<ByteVectorPinPremise, UnsafePinnedVectorPointer<byte>>
    {
        public bool LessThanOrEqualAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
            ByteCharTheory.LessThanOrEqualAll(left.LoadVector(), right.LoadVector());

        public bool EqualsAll(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right) =>
            ByteCharTheory.EqualsAll(left.LoadVector(), right.LoadVector());

        public UnsafePinnedVectorPointer<byte> Add(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharTheory.Add(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> Subtract(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharTheory.Subtract(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }

        public UnsafePinnedVectorPointer<byte> Modulus(UnsafePinnedVectorPointer<byte> left, UnsafePinnedVectorPointer<byte> right)
        {
            var toStore = ByteCharTheory.Modulus(left.LoadVector(), right.LoadVector());
            left.StoreVector(toStore);
            return left;
        }



        public unsafe bool TryUnsafeDecomposeToByteSpan(UnsafePinnedVectorPointer<byte> composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = new Span<byte>(composed.PinnedPointer, composed.SpanLength);
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

        public unsafe UnsafePinnedVectorPointer<byte> GetTemporaryConstant(byte value)
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

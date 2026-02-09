using Nemonuri.ByteChars.Internal;

namespace Nemonuri.ByteChars;

public static partial class ByteCharTheory
{
    public readonly unsafe struct ImmutableByteArrayPremise : IByteCharPremise<ImmutableByteArrayPremise, ImmutableArray<byte>>
    {
        public bool LessThanOrEqualAll(ImmutableArray<byte> left, ImmutableArray<byte> right) => 
            ByteCharSpanTheory.LessThanOrEqualAll(left.AsSpan(), left.AsSpan());

        public bool EqualsAll(ImmutableArray<byte> left, ImmutableArray<byte> right) =>
            ByteCharSpanTheory.EqualsAll(left.AsSpan(), left.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableArray<byte> UnsafeOperate
        (
            ImmutableArray<byte> left, 
            ImmutableArray<byte> right,
            delegate*<Span<byte>, ReadOnlySpan<byte>, void> op
        )
        {
            ImmutableArray<byte>.Builder dest = left.ToBuilder();
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
            op(MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in dest.ItemRef(0)), left.Length), right.AsSpan());
#else
            fixed (void* pByteL = &dest.ItemRef(0))
            {
                op(new Span<byte>(pByteL, left.Length), right.AsSpan());
            }
#endif
            return dest.DrainToImmutable();
        }


        public ImmutableArray<byte> Add(ImmutableArray<byte> left, ImmutableArray<byte> right) => UnsafeOperate(left, right, &ByteCharSpanTheory.Add);

        public ImmutableArray<byte> Subtract(ImmutableArray<byte> left, ImmutableArray<byte> right) => UnsafeOperate(left, right, &ByteCharSpanTheory.Subtract);

        public ImmutableArray<byte> Modulus(ImmutableArray<byte> left, ImmutableArray<byte> right) => UnsafeOperate(left, right, &ByteCharSpanTheory.Modulus);

        public bool TryUnsafeDecomposeToByteSpan(ImmutableArray<byte> composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = default;
            return false;
        }

        private static readonly ImmutableArray<byte>[] s_constants = 
            [..
                Enumerable.Range(0, ByteCharConstants.CodePageSize)
                    .Select(static n => unchecked((byte)n))
                    .Select(static n => ImmutableArray.Create(n))
            ];

        public ImmutableArray<byte> GetTemporaryConstant(byte value) => s_constants[value];
    }
}

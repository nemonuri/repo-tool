namespace Nemonuri.ByteChars;

public static partial class ByteCharTheory
{
    public readonly struct BytePremise : IByteCharPremise<BytePremise, byte>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool LessThanOrEqualAll(byte left, byte right) => left <= right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsAll(byte left, byte right) => left == right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Add(byte left, byte right) => unchecked((byte)(left + right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Subtract(byte left, byte right) => unchecked((byte)(left - right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Modulus(byte left, byte right) => unchecked((byte)(left % right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDecomposeToReadOnlyByteSpan(byte source, out ReadOnlySpan<byte> readOnlyByteSpan)
        {
            readOnlyByteSpan = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDecomposeToByteSpan(byte source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux)
        {
            byteSpan = default;
            aux = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetTemporaryConstant(byte value) => value;

        public unsafe delegate*<ReadOnlySpan<byte>, object?, byte> ComposeFromByteSpan => null;
    }
}

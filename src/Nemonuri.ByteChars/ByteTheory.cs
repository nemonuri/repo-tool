namespace Nemonuri.ByteChars;

public static class ByteTheory
{
    public const byte MostSignificantBitMask = 0b_1000_0000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMostSignificantBitSet(byte value) => (value & MostSignificantBitMask) != 0;
}
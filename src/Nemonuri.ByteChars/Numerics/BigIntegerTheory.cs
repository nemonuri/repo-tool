using System.Numerics;


namespace Nemonuri.ByteChars.Numerics;

public static class BigIntegerTheory
{
/**
- Reference: https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor?view=net-10.0#system-numerics-biginteger-ctor(system-byte())
*/

    public static BigInteger IntegerSpanToBigInteger(ReadOnlySpan<byte> byteIntegers, bool isUnsigned = false, bool isBigEndian = false)
    {
#if NETSTANDARD2_1_OR_GREATER
        // 딱 이게 필요한데, netstandard2.0 에는 없다! ...직접 만들어야지 뭐
        return new BigInteger(byteIntegers, isUnsigned, isBigEndian); 
#else
        return Nemonuri.NetStandards.Numerics.BigIntegerTheory.Create(byteIntegers, isUnsigned, isBigEndian);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<byte> ByteTrimStart(Span<byte> span, byte trimElement)
    {
#if NET8_0_OR_GREATER
        return span.TrimStart(trimElement);
#else
        return Nemonuri.NetStandards.MemoryTheory.TrimStart(span, trimElement);
#endif
    }


    /// <param name="integerString">Positive signed, little endian integer string.</param>
    public static bool TryParseAsciiByteSpanToBigInteger(ReadOnlySpan<byte> digits, out BigInteger bigint)
    {
        if (!ByteStringTheory.TryAsciiByteSpanToUtf16DotNetString(digits, out var dotnetString))
        {
            bigint = default; return false;
        }

        return BigInteger.TryParse(dotnetString, out bigint);
    }

    public static ImmutableArray<byte> FormatBigIntegerToAsciiDecimalByteString(BigInteger bigint)
    {
        var str = bigint.ToString("D");
        return ByteStringTheory.DotNetStringToUtf8ByteString(str);
    }


}
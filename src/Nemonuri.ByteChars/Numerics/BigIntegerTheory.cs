using System.Numerics;

namespace Nemonuri.ByteChars.Numerics;

public static class BigIntegerTheory
{
/**
- Reference: https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor?view=net-10.0#system-numerics-biginteger-ctor(system-byte())
*/

    public static BigInteger ByteIntegerSpanToBigInteger(ReadOnlySpan<byte> byteIntegers, bool isUnsigned = false, bool isBigEndian = false)
    {
        return new BigInteger(byteIntegers, isUnsigned, isBigEndian); 
        // 딱 이게 필요한데, netstandard2.0 에는 없다! ...직접 만들어야지 뭐
    }

}
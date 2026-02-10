using System.Numerics;
using System.Buffers.Text;
using System.Buffers.Binary;
using E9s = Nemonuri.ByteChars.Internal.Base1E9Premise;

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
    public static bool TryParseUnsignedDecimalDigitSpanToIntegerString(ReadOnlySpan<byte> digits, out ImmutableArray<byte> integerString)
    {
        var builder = ImmutableArray.CreateBuilder<byte>();

        Span<byte> tempStorage = stackalloc byte[sizeof(uint)];

        foreach (var digit1E9 in E9s.SplitSpan(digits))
        {
            if (!Utf8Parser.TryParse(digit1E9, out uint chunkValue, out _))
            {
                integerString = default; return false;
            }

            BinaryPrimitives.WriteUInt32BigEndian(tempStorage, chunkValue);
            
            var leadingZeroTrimmed = ByteTrimStart(tempStorage, ByteCharConstants.AsciiNull);
            builder.AddRange(leadingZeroTrimmed);
        }

        // To little endian
        builder.Reverse(); 

        // To positive signed
        if (ByteTheory.IsMostSignificantBitSet(builder[^1]))
        {
            builder.Add(ByteCharConstants.AsciiNull);
        }

        integerString = builder.DrainToImmutable();
        return true;
    }

    public static bool TryParseUnsignedDecimalDigitSpanToBigInteger(ReadOnlySpan<byte> digits, out BigInteger bigInteger)
    {
        if (TryParseUnsignedDecimalDigitSpanToIntegerString(digits, out var integerString))
        {
            bigInteger = IntegerSpanToBigInteger(integerString.AsSpan());
            return true;
        }
        bigInteger = default;
        return false;
    }

}
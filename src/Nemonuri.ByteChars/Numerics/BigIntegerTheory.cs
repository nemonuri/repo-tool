using System.Numerics;
using System.Buffers.Text;
using System.Buffers.Binary;
using I4s = Nemonuri.ByteChars.Internal.Base1E9Premise;

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

    /// <param name="integerString">Positive signed, little endian integer string.</param>
    public static bool TryParseUnsignedDecimalDigitSpanToIntegerString(ReadOnlySpan<byte> digits, out ImmutableArray<byte> integerString)
    {
        var builder = ImmutableArray.CreateBuilder<byte>();
        ReadOnlySpan<byte> stepDigits = digits;
        Span<byte> tempStorage = stackalloc byte[sizeof(uint)];

        while (stepDigits.Length > 0 && Utf8Parser.TryParse(stepDigits, out uint stepValue, out int stepBytesConsumed))
        {
            stepDigits = stepDigits[stepBytesConsumed..];

            bool wasFinalStep = stepDigits.Length == 0;

            //--- Handle stepStorage ---
            Span<byte> stepStorage = tempStorage;
            stepStorage = BinaryPrimitives
            //---|
        }

/*
        while (stepDigits.Length > 0 && Utf8Parser.TryParse(stepDigits, out uint stepValue, out int stepBytesConsumed))
        {
            BinaryPrimitives.WriteUInt32BigEndian(tempStorage, stepValue);
            Span<byte> trimmed = 
            builder.AddRange(tempStorage);

        }
*/

    }

}
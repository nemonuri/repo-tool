namespace Nemonuri.ByteChars;

using System.Diagnostics;
using B = ByteCharConstants;
using Bp = ByteCharTheory.BytePremise;


public static partial class ByteCharTheory
{
    /**
      Dreived from premises.
    */

    public static bool IsValid(byte byteChar) => Bp.IsValidAll(byteChar);

    public static bool IsLower(byte byteChar) => Bp.IsLowerAll(byteChar);

    public static bool IsUpper(byte byteChar) => Bp.IsUpperAll(byteChar);

    public static bool IsLetter(byte byteChar) => IsLower(byteChar) || IsUpper(byteChar);

    public static bool IsDecimalDigit(byte byteChar) => Bp.IsDecimalDigitAll(byteChar);

    public static bool IsAlphanumeric(byte byteChar) => IsLetter(byteChar) || IsDecimalDigit(byteChar);

    public static bool IsBlank(byte byteChar) => Bp.IsBlankAll(byteChar);

    public static bool IsEqualTo(byte left, byte right) => Bp.IsEqualToConstantAll(left, right);

    public static bool IsInInclusiveRange(byte byteChar, byte min, byte max) => Bp.IsInInclusiveConstantRangeAll(byteChar, min, max);

    public static bool IsWhite(byte byteChar) => 
      IsBlank(byteChar) ||
      IsEqualTo(byteChar, B.AsciiLineFeed) ||
      IsEqualTo(byteChar, B.AsciiVerticalTabulation) ||
      IsEqualTo(byteChar, B.AsciiFormFeed) ||
      IsEqualTo(byteChar, B.AsciiCarriageReturn) ;

    public static bool IsGraphic(byte byteChar) => Bp.IsGraphicAll(byteChar);

    public static bool IsPrint(byte byteChar) => IsGraphic(byteChar) || IsEqualTo(byteChar, B.AsciiSpace);

    public static bool IsControl(byte byteChar) => 
      IsInInclusiveRange(byteChar, B.AsciiNull, 0x1f) || IsEqualTo(byteChar, B.AsciiDelete);
    
  /**
    'Or'이 없는 것은, 실제 구현을 Implicit 하게 할 필요가 없다.
    - Implicit Deduction 의 메소드로 드러내는 것은, 단지 Syntax sugar 일 뿐.
    - 최적화와 반대로, 'Test converage' 는 일찍 고려할수록 좋다.
  */


    public static bool IsInLowerAToF(byte byteChar) => Bp.IsInLowerAToFAll(byteChar);

    public static bool IsInUpperAToF(byte byteChar) => Bp.IsInUpperAToFAll(byteChar);


    public static bool IsHexadecimalDigit(byte byteChar) => IsDecimalDigit(byteChar) || IsInLowerAToF(byteChar) || IsInUpperAToF(byteChar);

    public static byte UncheckedSubtract(byte left, byte right) => Bp.SubtractConstant(left, right);

    public static byte UncheckedDecimalDigitToInteger(byte byteChar) => UncheckedSubtract(byteChar, B.AsciiDigit0);

    public static void UncheckedUpdateDecimalDigitToInteger(ref byte byteChar)
        { byteChar = UncheckedDecimalDigitToInteger(byteChar); }

    public static byte UncheckedAdd(byte left, byte right) => Bp.AddConstant(left, right);

    public static byte UncheckedIntegerToDecimalDigit(byte integer) => UncheckedAdd(integer, B.AsciiDigit0);

    public static byte Modulus(byte left, byte right) => Bp.ModulusConstant(left, right);

    public static byte IntegerToDecimalDigit(byte integer) => UncheckedIntegerToDecimalDigit(Modulus(integer, 10));
    
    public static void UpdateIntegerToDecimalDigit(ref byte integer)
        { integer = IntegerToDecimalDigit(integer); }

    public static bool TryHexadecimalDigitToInteger(byte byteChar, out byte byteInteger)
    {
        if (IsDecimalDigit(byteChar))
        {
            byteInteger = UncheckedDecimalDigitToInteger(byteChar); return true;
        }
        else if (IsInLowerAToF(byteChar))
        {
            byteInteger = UncheckedSubtract(byteChar, B.AsciiLowerA); return true;
        }
        else if (IsInUpperAToF(byteChar))
        {
          byteInteger = UncheckedSubtract(byteChar, B.AsciiUpperA); return true;
        }
        else
        {
            Debug.Assert( !IsHexadecimalDigit(byteChar) );
            byteInteger = default; return false;
        }
    }

    /// <returns>True iff success.</returns>
    public static bool UpdateToIntegerIfHexadecimalDigit(ref byte byteChar)
    {
        if (TryHexadecimalDigitToInteger(byteChar, out byte byteInteger))
        {
            byteChar = byteInteger;
            return true;
        }
        else
        {
            return false;
        }
    }

    private static byte IntegerToHexadecimalDigit_BaseCharRequired(byte integer, byte baseChar)
    {
        int rem = Math.Abs(integer % 16);
        if (rem < 10)
        {
            return UncheckedIntegerToDecimalDigit((byte)rem);
        }
        else
        {
            return Bp.AddConstant((byte)(rem - 10), baseChar);
        }
    }

    private static void UpdateIntegerToHexadecimalDigit_HexBaseCharRequired(ref byte integer, byte hexBaseChar)
    {
        integer = IntegerToHexadecimalDigit_BaseCharRequired(integer, hexBaseChar);
    }

    public static byte ToUpperCase(byte byteChar) =>
        IsLower(byteChar) ? UncheckedSubtract(byteChar, B.AsciiUpperToLowerDistance) : byteChar;

    public static void UpdateToUpperCase(ref byte byteChar) { byteChar = ToUpperCase(byteChar); }
    
    public static byte ToLowerCase(byte byteChar) =>
        IsUpper(byteChar) ? UncheckedAdd(byteChar, B.AsciiUpperToLowerDistance) : byteChar;
    
    public static void UpdateToLowerCase(ref byte byteChar) { byteChar = ToLowerCase(byteChar); }

    public static byte DotNetCharToByteChar(char dotNetChar) => unchecked((byte)dotNetChar);

    /// <exception cref="System.OverflowException" />
    public static byte CheckedDotNetCharToByteChar(char dotNetChar) => checked((byte)dotNetChar);

    public static char ByteCharToDotNetChar(byte byteChar) => (char)byteChar;

}

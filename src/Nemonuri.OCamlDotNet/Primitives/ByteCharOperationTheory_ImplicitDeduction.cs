namespace Nemonuri.OCamlDotNet;

using System.Diagnostics;
using B = ByteCharTheory;
using Bp = ByteCharOperationTheory.BytePremise;


public static partial class ByteCharOperationTheory
{
    /**
      Dreived from premises.
    */

    public static bool IsLower(byte byteChar) => Bp.IsLowerAll(byteChar);

    public static bool IsUpper(byte byteChar) => Bp.IsUpperAll(byteChar);

    public static bool IsLetter(byte byteChar) => IsLower(byteChar) || IsUpper(byteChar);

    public static bool IsAlphanumeric(byte byteChar) => Bp.IsLetterAll(byteChar) || Bp.IsDecimalDigitAll(byteChar);

    public static bool IsWhite(byte byteChar) => 
      Bp.IsBlankAll(byteChar) ||
      Bp.IsEqualToConstantAll(byteChar, B.AsciiLineFeed) ||
      Bp.IsEqualToConstantAll(byteChar, B.AsciiVerticalTabulation) ||
      Bp.IsEqualToConstantAll(byteChar, B.AsciiFormFeed) ||
      Bp.IsEqualToConstantAll(byteChar, B.AsciiCarriageReturn) ;
      
    public static bool IsPrint(byte byteChar) => Bp.IsGraphicAll(byteChar) || Bp.IsEqualToConstantAll(byteChar, B.AsciiSpace);

    public static bool IsControl(byte byteChar) => 
      Bp.IsInInclusiveConstantRangeAll(byteChar, B.AsciiNull, 0x1f) || Bp.IsEqualToConstantAll(byteChar, B.AsciiDelete);
    
  /**
    'Or'이 없는 것은, 실제 구현을 Implicit 하게 할 필요가 없다.
    - Implicit Deduction 의 메소드로 드러내는 것은, 단지 Syntax sugar 일 뿐.
    - 최적화와 반대로, 'Test converage' 는 일찍 고려할수록 좋다.
  */

    public static bool IsDecimalDigit(byte byteChar) => Bp.IsDecimalDigitAll(byteChar);

    public static bool IsInLowerAToF(byte byteChar) => Bp.IsInLowerAToFAll(byteChar);

    public static bool IsInUpperAToF(byte byteChar) => Bp.IsInUpperAToFAll(byteChar);


    public static bool IsHexadecimalDigit(byte byteChar) => IsDecimalDigit(byteChar) || IsInLowerAToF(byteChar) || IsInUpperAToF(byteChar);


    public static byte UncheckedDecimalDigitToInteger(byte byteChar) => Bp.SubtractConstant(byteChar, B.AsciiDigit0);

    public static void UncheckedUpdateDecimalDigitToInteger(ref byte byteChar)
        { byteChar = UncheckedDecimalDigitToInteger(byteChar); }

    public static byte UncheckedIntegerToDecimalDigit(byte integer) => Bp.AddConstant(integer, B.AsciiDigit0);

    public static byte IntegerToDecimalDigit(byte integer) => UncheckedIntegerToDecimalDigit(Bp.ModulusConstant(integer, 10));
    
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
            byteInteger = Bp.SubtractConstant(byteChar, B.AsciiLowerA); return true;
        }
        else if (IsInUpperAToF(byteChar))
        {
          byteInteger = Bp.SubtractConstant(byteChar, B.AsciiUpperA); return true;
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
        IsLower(byteChar) ? Bp.SubtractConstant(byteChar, B.UpperToLowerDistance) : byteChar;

    public static void UpdateToUpperCase(ref byte byteChar) { byteChar = ToUpperCase(byteChar); }
    
    public static byte ToLowerCase(byte byteChar) =>
        IsUpper(byteChar) ? Bp.AddConstant(byteChar, B.UpperToLowerDistance) : byteChar;
    
    public static void UpdateToLowerCase(ref byte byteChar) { byteChar = ToLowerCase(byteChar); }
}

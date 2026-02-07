namespace Nemonuri.OCamlDotNet;
using B = ByteCharTheory;
using Bp = ByteCharOperationTheory.BytePremise;


public static partial class ByteCharOperationTheory
{
    /**
      Dreived from premises.
    */

    public static bool IsLetter(byte byteChar) => Bp.IsLowerAll(byteChar) || Bp.IsUpperAll(byteChar);

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
    
    public static bool IsHexadecimalDigit(byte byteChar) =>
      Bp.IsDecimalDigitAll(byteChar) || Bp.IsInLowerAToFAll(byteChar) || Bp.IsInUpperAToFAll(byteChar);
    
}

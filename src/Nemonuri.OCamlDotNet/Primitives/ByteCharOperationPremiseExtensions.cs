namespace Nemonuri.OCamlDotNet;

using B = ByteCharTheory;

public static class ByteCharOperationPremiseExtensions
{
    extension<P, O>(P) /* TPremise, TOperand */
        where P : unmanaged, IByteCharOperationPremise<P, O>
        where O : notnull
    {
        public static bool IsInInclusiveRangeAll(O chars, O min, O max)
        {
            P th = new();
            return
                th.LessThanOrEqualAll(min, chars) &&
                th.LessThanOrEqualAll(chars, max);
        }

        public static bool IsInInclusiveConstantRangeAll(O chars, byte min, byte max)
        {
            P th = new();
            return IsInInclusiveRangeAll<P, O>(chars, th.GetConstant(min), th.GetConstant(max));
        }

        public static bool IsEqualToConstantAll(O chars, byte constant)
        {
            P th = new();
            return th.EqualsAll(chars, th.GetConstant(constant));
        }

        public static bool IsEqualToConstantAny(O left, byte right)
        {
            P th = new();
            return th.EqualsAny(left, th.GetConstant(right));
        } 

        public static bool IsValidAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiMinimum, B.AsciiMaximum);

        public static bool IsUpperAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiUpperA, B.AsciiUpperZ);

        public static bool IsLowerAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiLowerA, B.AsciiLowerZ);

        public static bool IsLetterAll(O chars) => IsLowerAll<P, O>(chars) || IsUpperAll<P, O>(chars);

        public static bool IsDecimalDigitAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.Digit0, B.Digit9);

        public static bool IsAlphanumericAll(O chars) => IsLetterAll<P, O>(chars) || IsDecimalDigitAll<P, O>(chars);

        public static bool IsWhiteAll(O chars) => 
            IsBlankAll<P, O>(chars) ||
            IsEqualToConstantAll<P, O>(chars, B.AsciiLineFeed) ||
            IsEqualToConstantAll<P, O>(chars, B.AsciiVerticalTabulation) ||
            IsEqualToConstantAll<P, O>(chars, B.AsciiFormFeed) ||
            IsEqualToConstantAll<P, O>(chars, B.AsciiCarriageReturn) ;

        public static bool IsBlankAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiHorizontalTabulation, B.AsciiSpace);

        public static bool IsGraphicAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiGraphicCharacterMinimum, B.AsciiGraphicCharacterMaximum);

        public static bool IsPrintAll(O chars) => IsGraphicAll<P, O>(chars) || IsEqualToConstantAll<P, O>(chars, B.AsciiSpace);
        
        public static bool IsControlAll(O chars) => 
            IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiNull, 0x1f) ||
            IsEqualToConstantAll<P, O>(chars, B.AsciiDelete);
        
        public static O SubtractConstantAll(O left, byte right)
        {
            P th = new();
            return th.SubtractAll(left, th.GetConstant(right));
        }

        /// <returns>Bytes as unsigned integers.</returns>
        public static O UnsafeDecimalDigitToIntegerAll(O chars) => SubtractConstantAll<P, O>(chars, B.Digit0);

        public static O AddConstantAll(O left, byte right)
        {
            P th = new();
            return th.AddAll(left, th.GetConstant(right));
        }

        public static O ModulusConstantAll(O left, byte right)
        {
            P th = new();
            return th.ModulusAll(left, th.GetConstant(right));
        }

        public static O IntegerToDecimalDigitAll(O ints) => 
            AddConstantAll<P,O>(ModulusConstantAll<P,O>(ints, 10), B.Digit0);
        
        public static bool IsInLowerAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiLowerA, B.AsciiLowerF);

        public static bool IsInUpperAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiUpperA, B.AsciiUpperF);

        public static bool IsHexadecimalDigitAll(O chars) =>
            IsDecimalDigitAll<P,O>(chars) || IsInLowerAToFAll<P,O>(chars) || IsInUpperAToFAll<P,O>(chars);

        /// <returns>Bytes as unsigned integers.</returns>
        /*
        public static bool TryHexadecimalDigitToIntegerAll(O chars, [NotNullWhen(true)] out O? result)
        {
            if (IsDecimalDigitAll<P,O>(chars))
            {
                result = UnsafeDecimalDigitToIntegerAll<P,O>(chars); return true;
            }
            else if (IsInLowerAToFAll<P,O>(chars))
            {
                result = SubtractConstantAll<P,O>(chars, B.AsciiLowerA); return true;
            }
            else if (IsInUpperAToFAll<P,O>(chars))
            {
                result = SubtractConstantAll<P,O>(chars, B.AsciiUpperA); return true;
            }
            else
            {
                res
            }
        }
        */

            
    }
}

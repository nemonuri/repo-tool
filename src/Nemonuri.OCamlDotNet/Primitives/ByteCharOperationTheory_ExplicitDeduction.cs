namespace Nemonuri.OCamlDotNet;

using B = ByteCharTheory;
using Id = ByteCharOperationTheory; // Implicit Deduction
using Bp = ByteCharOperationTheory.BytePremise;

public static unsafe partial class ByteCharOperationTheory
{
    extension<P, O>(P) /* TPremise, TOperand */
        where P : unmanaged, IByteCharOperationPremise<P, O>
        where O : notnull
    {
/**
```
∀x.( ((∀n1.P1(x,n1)) && (∀n2.P2(x,n2))) ⇔ (∀n.(P1(x,n) && P2(x,n))) )
      ------------ ↑ Actual -------------    -------- ↑ Desired -------

    ⇕   ┌ Remove free var 'x'

( (∀n1.P1(n1)) && (∀n2.P2(n2)) ) ⇔ ( ∀n.(P1(n) && P2(n)) )

    ⇕   ┌ To Prenex form

( ∀n1.∀n2.(P1(n1) && P2(n2)) ) ⇔ ( ∀n.(P1(n) && P2(n)) )

```

- [Rules of passage](https://en.wikipedia.org/wiki/Rules_of_passage) 라고, 이미 증명되어 있었구나...
  - '⇒' 가 아니라, '⇔' 이구나!
*/
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

        public static bool UnsafeAll(O chars, delegate*<byte, bool> predicate)
        {
            P th = new();
            if (th.TryUnsafeDecomposeToByteSpan(chars, out var unsafeBytes))
            {
                foreach (byte b in unsafeBytes)
                    { if (predicate(b)) {return false;} }

                return true;
            }
            else if (chars is byte sgt)
            {
                return predicate(sgt);
            }
            else
            {
                // Assume: 'chars' is empty set.
                return true;
            }
        }

        public static O SubtractConstant(O left, byte right)
        {
            P th = new();
            return th.Subtract(left, th.GetConstant(right));
        }

        public static O AddConstant(O left, byte right)
        {
            P th = new();
            return th.Add(left, th.GetConstant(right));
        }

        public static O ModulusConstant(O left, byte right)
        {
            P th = new();
            return th.Modulus(left, th.GetConstant(right));
        }



        public static bool IsValidAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiMinimum, B.AsciiMaximum);

        public static bool IsUpperAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiUpperA, B.AsciiUpperZ);

        public static bool IsLowerAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiLowerA, B.AsciiLowerZ);

        public static bool IsDecimalDigitAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.Digit0, B.Digit9);

        public static bool IsBlankAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiHorizontalTabulation, B.AsciiSpace);

        public static bool IsGraphicAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiGraphicCharacterMinimum, B.AsciiGraphicCharacterMaximum);



        public static bool IsLetterAll(O chars) => chars is byte b ? Id.IsLetter(b) : UnsafeAll<P,O>(chars, &Id.IsLetter);

        public static bool IsAlphanumericAll(O chars) => chars is byte b ? Id.IsAlphanumeric(b) : UnsafeAll<P,O>(chars, &Id.IsAlphanumeric);

        public static bool IsWhiteAll(O chars) => chars is byte b ? Id.IsWhite(b) : UnsafeAll<P,O>(chars, &Id.IsWhite);

        public static bool IsPrintAll(O chars) => chars is byte b ? Id.IsPrint(b) : UnsafeAll<P,O>(chars, &Id.IsPrint);
        
        public static bool IsControlAll(O chars) => chars is byte b ? Id.IsControl(b) : UnsafeAll<P,O>(chars, &Id.IsControl);
        


        /// <returns>Bytes as unsigned integers.</returns>
        public static O UnsafeDecimalDigitToInteger(O chars) => SubtractConstant<P, O>(chars, B.Digit0);

        public static O IntegerToDecimalDigit(O ints) => AddConstant<P,O>(ModulusConstant<P,O>(ints, 10), B.Digit0);
        
        public static bool IsInLowerAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiLowerA, B.AsciiLowerF);

        public static bool IsInUpperAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiUpperA, B.AsciiUpperF);

        public static bool IsHexadecimalDigitAll(O chars) => chars is byte b ? Id.IsHexadecimalDigit(b) : UnsafeAll<P,O>(chars, &Id.IsHexadecimalDigit);

        

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

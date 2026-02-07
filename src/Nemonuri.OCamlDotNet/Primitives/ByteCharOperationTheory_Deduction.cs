namespace Nemonuri.OCamlDotNet;

using B = ByteCharTheory;
using Bo = ByteCharOperationTheory;
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

        public static unsafe bool UnsafeAll(O chars, delegate*<byte, bool> predicate)
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

        public static bool IsValidAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiMinimum, B.AsciiMaximum);

        public static bool IsUpperAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiUpperA, B.AsciiUpperZ);

        public static bool IsLowerAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiLowerA, B.AsciiLowerZ);

        public static bool IsDecimalDigitAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.Digit0, B.Digit9);

        public static bool IsBlankAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiHorizontalTabulation, B.AsciiSpace);

        public static bool IsGraphicAll(O chars) => IsInInclusiveConstantRangeAll<P, O>(chars, B.AsciiGraphicCharacterMinimum, B.AsciiGraphicCharacterMaximum);



        public static bool IsLetterAll(O chars) => chars is byte b ? Bo.IsLetter(b) : UnsafeAll<P,O>(chars, &Bo.IsLetter);

        public static bool IsAlphanumericAll(O chars) => chars is byte b ? Bo.IsAlphanumeric(b) : UnsafeAll<P,O>(chars, &Bo.IsAlphanumeric);

        public static bool IsWhiteAll(O chars) => chars is byte b ? Bo.IsWhite(b) : UnsafeAll<P,O>(chars, &Bo.IsWhite);



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

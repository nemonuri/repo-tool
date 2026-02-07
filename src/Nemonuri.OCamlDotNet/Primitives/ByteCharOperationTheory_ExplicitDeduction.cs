namespace Nemonuri.OCamlDotNet;

using System.Diagnostics.CodeAnalysis;
using B = ByteCharTheory;
using Id = ByteCharOperationTheory; // Implicit Deduction
using Bp = ByteCharOperationTheory.BytePremise;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;

public static unsafe partial class ByteCharOperationTheory
{
    extension<P, O>(P) /* TPremise, TOperand */
        where P : unmanaged, IByteCharOperationPremise<P, O>
        where O : notnull
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
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

        public static void UnsafeUpdateWithAux<TAux>(ref O chars, TAux aux, delegate*<ref byte, TAux, void> updater)
        {
            P th = new();
            if (th.TryUnsafeDecomposeToByteSpan(chars, out Span<byte> unsafeBytes))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b, aux); }
            }
            else if (chars is byte)
            {
                updater(ref Unsafe.As<O, byte>(ref chars), aux);
            }
            else
            {
                // Assume: 'chars' is empty set.
                return;
            }
        }

        public static int UnsafeUpdateWhileSuccess(ref O chars, delegate*<ref byte, bool> updater)
        {
            P th = new();
            if (th.TryUnsafeDecomposeToByteSpan(chars, out Span<byte> unsafeBytes))
            {
                int i = 0;
                for (; i < unsafeBytes.Length; i++)
                {
                    if (!updater(ref unsafeBytes[i]))
                    {
                        break;
                    }
                }
                return i;
            }
            else if (chars is byte)
            {
                return updater(ref Unsafe.As<O, byte>(ref chars)) ? 1 : 0;
            }
            else
            {
                // Assume: 'chars' is empty set.
                return 0;
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

        private static O IntegerToDecimalDigit_AssumeLessThan10(O rem) => AddConstant<P,O>(rem, B.Digit0);

        public static O IntegerToDecimalDigit(O ints) => IntegerToDecimalDigit_AssumeLessThan10<P,O>(ModulusConstant<P,O>(ints, 10));
        
        public static bool IsInLowerAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiLowerA, B.AsciiLowerF);

        public static bool IsInUpperAToFAll(O chars) => IsInInclusiveConstantRangeAll<P,O>(chars, B.AsciiUpperA, B.AsciiUpperF);

        public static bool IsHexadecimalDigitAll(O chars) => chars is byte b ? Id.IsHexadecimalDigit(b) : UnsafeAll<P,O>(chars, &Id.IsHexadecimalDigit);



        /// <returns>Success span length.</returns>
        public static int UnsafeHexadecimalDigitToInteger(ref O chars) =>
            (chars is byte) ? 
                (Id.UpdateToIntegerIfHexadecimalDigit(ref Unsafe.As<O, byte>(ref chars)) ? 1 : 0) :
                UnsafeUpdateWhileSuccess<P,O>(ref chars, &Id.UpdateToIntegerIfHexadecimalDigit);

        private static void UnsafeIntegerToHexadecimalDigit_BaseCharRequired(ref O chars, byte baseChar)
        {
            if (chars is byte)
            { 
                Id.UpdateIntegerToHexadecimalDigit_BaseCharRequired(ref Unsafe.As<O, byte>(ref chars), baseChar); 
            }
            else
            { 
                UnsafeUpdateWithAux<P,O,byte>(ref chars, baseChar, &Id.UpdateIntegerToHexadecimalDigit_BaseCharRequired); 
            }
        }

        public static O IntegerToLowerHexadecimalDigit()
        {
            
        }
            
/*
        {
            if (chars is byte)
            {
                if (Id.ConvertToIntegerIfHexadecimalDigit(ref Unsafe.As<O, byte>(ref chars)))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (new P().TryUnsafeDecomposeToByteSpan(chars, out Span<byte> bytes))
            {
                int i = 0;
                for (; i < bytes.Length; i++)
                {
                    if (!Id.ConvertToIntegerIfHexadecimalDigit(ref bytes[i]))
                    {
                        break;
                    }
                }
                return i;
            }
            else
            {
                return 0;
            }
        }
    */

        

            
    }
}

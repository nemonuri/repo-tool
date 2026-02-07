namespace Nemonuri.OCamlDotNet;


using B = ByteCharTheory;
using Id = ByteCharOperationTheory; // Implicit Deduction
using System.Runtime.CompilerServices;

public static unsafe partial class ByteCharOperationTheory
{
    private static ref byte AsByteRef<TOperand>(ref TOperand sourceRef) 
        where TOperand : notnull 
#if NET9_0_OR_GREATER
        // , allows ref struct
#endif
        => 
        ref Unsafe.As<TOperand, byte>(ref sourceRef);
        

    extension<TPremise, TOperand>(TPremise) /* TPremise, TOperand */
        where TPremise : unmanaged, IByteCharOperationPremise<TPremise, TOperand>
        where TOperand : notnull
#if NET9_0_OR_GREATER
        // , allows ref struct
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
        public static bool IsInInclusiveRangeAll(TOperand chars, TOperand min, TOperand max)
        {
            TPremise th = new();
            return
                th.LessThanOrEqualAll(min, chars) &&
                th.LessThanOrEqualAll(chars, max);
        }

        public static bool IsInInclusiveConstantRangeAll(TOperand chars, byte min, byte max)
        {
            TPremise th = new();
            return IsInInclusiveRangeAll<TPremise, TOperand>(chars, th.GetConstant(min), th.GetConstant(max));
        }


        public static bool IsEqualToConstantAll(TOperand chars, byte constant)
        {
            TPremise th = new();
            return th.EqualsAll(chars, th.GetConstant(constant));
        }

        public static bool UnsafeAll(TOperand chars, delegate*<byte, bool> predicate)
        {
            TPremise th = new();
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

        public static void UnsafeUpdate(ref TOperand chars, delegate*<ref byte, void> updater)
        {
            TPremise th = new();
            if (th.TryUnsafeDecomposeToByteSpan(chars, out Span<byte> unsafeBytes))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b); }
            }
            else if (chars is byte)
            {
                updater(ref AsByteRef(ref chars));
            }
            else
            {
                // Assume: 'chars' is empty set.
                return;
            }
        }        

        public static void UnsafeUpdateWithAux<TAux>(ref TOperand chars, TAux aux, delegate*<ref byte, TAux, void> updater)
        {
            TPremise th = new();
            if (th.TryUnsafeDecomposeToByteSpan(chars, out Span<byte> unsafeBytes))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b, aux); }
            }
            else if (chars is byte)
            {
                updater(ref AsByteRef(ref chars), aux);
            }
            else
            {
                // Assume: 'chars' is empty set.
                return;
            }
        }

        public static int UnsafeUpdateWhileSuccess(ref TOperand chars, delegate*<ref byte, bool> updater)
        {
            TPremise th = new();
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
                return updater(ref AsByteRef(ref chars)) ? 1 : 0;
            }
            else
            {
                // Assume: 'chars' is empty set.
                return 0;
            }
        }

        public static TOperand SubtractConstant(TOperand left, byte right)
        {
            TPremise th = new();
            return th.Subtract(left, th.GetConstant(right));
        }

        public static TOperand AddConstant(TOperand left, byte right)
        {
            TPremise th = new();
            return th.Add(left, th.GetConstant(right));
        }

        public static TOperand ModulusConstant(TOperand left, byte right)
        {
            TPremise th = new();
            return th.Modulus(left, th.GetConstant(right));
        }



        public static bool IsValidAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiMinimum, B.AsciiMaximum);

        public static bool IsUpperAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiUpperA, B.AsciiUpperZ);

        public static bool IsLowerAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiLowerA, B.AsciiLowerZ);

        public static bool IsDecimalDigitAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiDigit0, B.AsciiDigit9);

        public static bool IsBlankAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiHorizontalTabulation, B.AsciiSpace);

        public static bool IsGraphicAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise, TOperand>(chars, B.AsciiGraphicCharacterMinimum, B.AsciiGraphicCharacterMaximum);



        public static bool IsLetterAll(TOperand chars) => chars is byte b ? Id.IsLetter(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsLetter);

        public static bool IsAlphanumericAll(TOperand chars) => chars is byte b ? Id.IsAlphanumeric(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsAlphanumeric);

        public static bool IsWhiteAll(TOperand chars) => chars is byte b ? Id.IsWhite(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsWhite);

        public static bool IsPrintAll(TOperand chars) => chars is byte b ? Id.IsPrint(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsPrint);
        
        public static bool IsControlAll(TOperand chars) => chars is byte b ? Id.IsControl(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsControl);
        


        /// <returns>Bytes as unsigned integers.</returns>
        public static void UncheckedUpdateDecimalDigitToInteger(ref TOperand chars)
        {
            if (chars is byte)
            {
                Id.UncheckedUpdateDecimalDigitToInteger(ref AsByteRef(ref chars));
            }
            else
            {
                UnsafeUpdate<TPremise,TOperand>(ref chars, &Id.UncheckedUpdateDecimalDigitToInteger);
            }
        }

        public static void UpdateIntegerToDecimalDigit(ref TOperand ints)
        {
            if (ints is byte)
            {
                Id.UpdateIntegerToDecimalDigit(ref AsByteRef(ref ints));
            }
            else
            {
                UnsafeUpdate<TPremise,TOperand>(ref ints, &Id.UpdateIntegerToDecimalDigit);
            }
        }
        
        public static bool IsInLowerAToFAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise,TOperand>(chars, B.AsciiLowerA, B.AsciiLowerF);

        public static bool IsInUpperAToFAll(TOperand chars) => IsInInclusiveConstantRangeAll<TPremise,TOperand>(chars, B.AsciiUpperA, B.AsciiUpperF);

        public static bool IsHexadecimalDigitAll(TOperand chars) => chars is byte b ? Id.IsHexadecimalDigit(b) : UnsafeAll<TPremise,TOperand>(chars, &Id.IsHexadecimalDigit);



        /// <returns>Success span length.</returns>
        public static int UpdateHexadecimalDigitToIntegerWhileSuccess(ref TOperand chars) =>
            (chars is byte) ? 
                (Id.UpdateToIntegerIfHexadecimalDigit(ref AsByteRef(ref chars)) ? 1 : 0) :
                UnsafeUpdateWhileSuccess<TPremise,TOperand>(ref chars, &Id.UpdateToIntegerIfHexadecimalDigit);

        private static void UpdateIntegerToHexadecimalDigit_HexBaseCharRequired(ref TOperand chars, byte hexBaseChar)
        {
            if (chars is byte)
            { 
                Id.UpdateIntegerToHexadecimalDigit_HexBaseCharRequired(ref AsByteRef(ref chars), hexBaseChar); 
            }
            else
            { 
                UnsafeUpdateWithAux<TPremise,TOperand,byte>(ref chars, hexBaseChar, &Id.UpdateIntegerToHexadecimalDigit_HexBaseCharRequired); 
            }
        }

        public static void UpdateIntegerToLowerHexadecimalDigit(ref TOperand chars) => UpdateIntegerToHexadecimalDigit_HexBaseCharRequired<TPremise,TOperand>(ref chars, B.AsciiLowerA);

        public static void UpdateIntegerToUpperHexadecimalDigit(ref TOperand chars) => UpdateIntegerToHexadecimalDigit_HexBaseCharRequired<TPremise,TOperand>(ref chars, B.AsciiUpperA);
            
        public static void UpdateToUpperCase(ref TOperand chars)
        {
            if (chars is byte)
                { Id.UpdateToUpperCase(ref AsByteRef(ref chars)); }
            else
                { UnsafeUpdate<TPremise,TOperand>(ref chars, &Id.UpdateToUpperCase); }
        }
        
        public static void UpdateToLowerCase(ref TOperand chars)
        {
            if (chars is byte)
                { Id.UpdateToLowerCase(ref AsByteRef(ref chars)); }
            else
                { UnsafeUpdate<TPremise,TOperand>(ref chars, &Id.UpdateToLowerCase); }
        }
            
    }
}

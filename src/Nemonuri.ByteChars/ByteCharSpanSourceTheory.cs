using Nemonuri.ByteChars.ByteSpans;
using System.Runtime.CompilerServices;

namespace Nemonuri.ByteChars;

using Bc = ByteCharConstants;
using Bt = ByteCharTheory;


public static unsafe class ByteCharSpanSourceTheory
{
    private static ref byte AsByteRef<TSource>(ref TSource sourceRef) 
        where TSource : notnull 
#if NET9_0_OR_GREATER
        // , allows ref struct
#endif
        => 
        ref Unsafe.As<TSource, byte>(ref sourceRef);
        

    extension<TPremise, TSource>(TPremise) /* TPremise, TOperand */
        where TPremise : unmanaged, IByteSpanSourcePremise<TSource>
        where TSource : notnull
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
        public static bool IsInInclusiveConstantRangeAll(TSource chars, byte min, byte max)
        {
            TPremise th = new();
            return
                th.LessThanOrEqualAll(th.GetTemporaryConstant(min), chars) &&
                th.LessThanOrEqualAll(chars, th.GetTemporaryConstant(max));
        }


        public static bool IsEqualToConstantAll(TSource chars, byte constant)
        {
            TPremise th = new();
            return th.EqualsAll(chars, th.GetTemporaryConstant(constant));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsComposeFromByteSpanRequired() => (new TPremise()).ComposeFromByteSpan != null;

        internal static void ComposeFromByteSpanIfRequired(Span<byte> byteSpan, object? aux, ref TSource dest)
        {
            if (!IsComposeFromByteSpanRequired<TPremise, TSource>()) { return; }

            dest = (new TPremise()).ComposeFromByteSpan(byteSpan, aux);
        }

        private static bool UnsafeAll(TSource chars, delegate*<byte, bool> predicate)
        {
            TPremise th = new();
            if (th.TryDecomposeToReadOnlyByteSpan(chars, out var unsafeBytes))
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

        private static void UnsafeUpdate(ref TSource chars, delegate*<ref byte, void> updater)
        {
            TPremise th = new();
            if (th.TryDecomposeToByteSpan(chars, out Span<byte> unsafeBytes, out var aux))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b); }
                ComposeFromByteSpanIfRequired<TPremise, TSource>(unsafeBytes, aux, ref chars);
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

        private static void UnsafeUpdateWithAux<TAux>(ref TSource chars, TAux aux, delegate*<ref byte, TAux, void> updater)
        {
            TPremise th = new();
            if (th.TryDecomposeToByteSpan(chars, out Span<byte> unsafeBytes, out var aux0))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b, aux); }
                ComposeFromByteSpanIfRequired<TPremise, TSource>(unsafeBytes, aux0, ref chars);
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

        private static void UnsafeUpdateWithState<TState>(ref TSource chars, ref TState state, delegate*<ref byte, ref TState, void> updater)
        {
            TPremise th = new();
            if (th.TryDecomposeToByteSpan(chars, out Span<byte> unsafeBytes, out var aux0))
            {
                foreach (ref byte b in unsafeBytes) { updater(ref b, ref state); }
                ComposeFromByteSpanIfRequired<TPremise, TSource>(unsafeBytes, aux0, ref chars);
            }
            else if (chars is byte)
            {
                updater(ref AsByteRef(ref chars), ref state);
            }
            else
            {
                // Assume: 'chars' is empty set.
                return;
            }
        }

        private static int UnsafeUpdateWhileSuccess(ref TSource chars, delegate*<ref byte, bool> updater)
        {
            TPremise th = new();
            if (th.TryDecomposeToByteSpan(chars, out Span<byte> unsafeBytes, out var aux))
            {
                int i = 0;
                for (; i < unsafeBytes.Length; i++)
                {
                    if (!updater(ref unsafeBytes[i]))
                    {
                        break;
                    }
                }
                ComposeFromByteSpanIfRequired<TPremise, TSource>(unsafeBytes, aux, ref chars);
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

        public static TSource SubtractConstant(TSource left, byte right)
        {
            TPremise th = new();
            return th.Subtract(left, th.GetTemporaryConstant(right));
        }

        public static TSource AddConstant(TSource left, byte right)
        {
            TPremise th = new();
            return th.Add(left, th.GetTemporaryConstant(right));
        }

        public static TSource ModulusConstant(TSource left, byte right)
        {
            TPremise th = new();
            return th.Modulus(left, th.GetTemporaryConstant(right));
        }



        public static bool IsValidAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiMinimum, Bc.AsciiMaximum);

        public static bool IsUpperAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiUpperA, Bc.AsciiUpperZ);

        public static bool IsLowerAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiLowerA, Bc.AsciiLowerZ);

        public static bool IsDecimalDigitAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiDigit0, Bc.AsciiDigit9);

        public static bool IsBlankAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiHorizontalTabulation, Bc.AsciiSpace);

        public static bool IsGraphicAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise, TSource>(chars, Bc.AsciiGraphicCharacterMinimum, Bc.AsciiGraphicCharacterMaximum);



        public static bool IsLetterAll(TSource chars) => chars is byte b ? Bt.IsLetter(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsLetter);

        public static bool IsAlphanumericAll(TSource chars) => chars is byte b ? Bt.IsAlphanumeric(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsAlphanumeric);

        public static bool IsWhiteAll(TSource chars) => chars is byte b ? Bt.IsWhite(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsWhite);

        public static bool IsPrintAll(TSource chars) => chars is byte b ? Bt.IsPrint(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsPrint);
        
        public static bool IsControlAll(TSource chars) => chars is byte b ? Bt.IsControl(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsControl);
        


        /// <returns>Bytes as unsigned integers.</returns>
        public static void UncheckedUpdateDecimalDigitToInteger(ref TSource chars)
        {
            if (chars is byte)
            {
                Bt.UncheckedUpdateDecimalDigitToInteger(ref AsByteRef(ref chars));
            }
            else
            {
                UnsafeUpdate<TPremise,TSource>(ref chars, &Bt.UncheckedUpdateDecimalDigitToInteger);
            }
        }

        public static void UpdateIntegerToDecimalDigit(ref TSource ints)
        {
            if (ints is byte)
            {
                Bt.UpdateIntegerToDecimalDigit(ref AsByteRef(ref ints));
            }
            else
            {
                UnsafeUpdate<TPremise,TSource>(ref ints, &Bt.UpdateIntegerToDecimalDigit);
            }
        }
        
        public static bool IsInLowerAToFAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise,TSource>(chars, Bc.AsciiLowerA, Bc.AsciiLowerF);

        public static bool IsInUpperAToFAll(TSource chars) => IsInInclusiveConstantRangeAll<TPremise,TSource>(chars, Bc.AsciiUpperA, Bc.AsciiUpperF);

        public static bool IsHexadecimalDigitAll(TSource chars) => chars is byte b ? Bt.IsHexadecimalDigit(b) : UnsafeAll<TPremise,TSource>(chars, &Bt.IsHexadecimalDigit);



        /// <returns>Success span length.</returns>
        public static int UpdateHexadecimalDigitToIntegerWhileSuccess(ref TSource chars) =>
            (chars is byte) ? 
                (Bt.UpdateToIntegerIfHexadecimalDigit(ref AsByteRef(ref chars)) ? 1 : 0) :
                UnsafeUpdateWhileSuccess<TPremise,TSource>(ref chars, &Bt.UpdateToIntegerIfHexadecimalDigit);

        private static void UpdateIntegerToHexadecimalDigit_HexBaseCharRequired(ref TSource chars, byte hexBaseChar)
        {
            if (chars is byte)
            { 
                Bt.UpdateIntegerToHexadecimalDigit_HexBaseCharRequired(ref AsByteRef(ref chars), hexBaseChar); 
            }
            else
            { 
                UnsafeUpdateWithAux<TPremise,TSource,byte>(ref chars, hexBaseChar, &Bt.UpdateIntegerToHexadecimalDigit_HexBaseCharRequired); 
            }
        }

        public static void UpdateIntegerToLowerHexadecimalDigit(ref TSource chars) => UpdateIntegerToHexadecimalDigit_HexBaseCharRequired<TPremise,TSource>(ref chars, Bc.AsciiLowerA);

        public static void UpdateIntegerToUpperHexadecimalDigit(ref TSource chars) => UpdateIntegerToHexadecimalDigit_HexBaseCharRequired<TPremise,TSource>(ref chars, Bc.AsciiUpperA);
            
        public static void UpdateToUpperCase(ref TSource chars)
        {
            if (chars is byte)
                { Bt.UpdateToUpperCase(ref AsByteRef(ref chars)); }
            else
                { UnsafeUpdate<TPremise,TSource>(ref chars, &Bt.UpdateToUpperCase); }
        }
        
        public static void UpdateToLowerCase(ref TSource chars)
        {
            if (chars is byte)
                { Bt.UpdateToLowerCase(ref AsByteRef(ref chars)); }
            else
                { UnsafeUpdate<TPremise,TSource>(ref chars, &Bt.UpdateToLowerCase); }
        }

        public static void UpdateFirstAscii(ref TSource chars, delegate*<ref byte, void> charUpdater)
        {
            CharUpdater cu = new(charUpdater);
            if (chars is byte)
            { 
                cu.Invoke(ref AsByteRef(ref chars));
            }
            else
            {
                (bool, CharUpdater) state = (false, cu);
                UnsafeUpdateWithState<TPremise,TSource,(bool, CharUpdater)>(ref chars, ref state, &UpdateFirstAscii_UpdaterImpl);
            }
        }

        public static void UpdateToCapitalizdAscii(ref TSource chars) => UpdateFirstAscii<TPremise,TSource>(ref chars, &Bt.UpdateToUpperCase);

        public static void UpdateToUncapitalizdAscii(ref TSource chars) => UpdateFirstAscii<TPremise,TSource>(ref chars, &Bt.UpdateToLowerCase);
    }

    private static void UpdateFirstAscii_UpdaterImpl(ref byte currentChar, ref (bool PrevWasLetter, CharUpdater CharUpdater) state)
    {
        if (!Bt.IsLetter(currentChar)) { state.PrevWasLetter = false; return; }
        if (state.PrevWasLetter) { return; }

        // Current char is first letter.
        state.CharUpdater.Invoke(ref currentChar);
        state.PrevWasLetter = true;
    }

    private readonly struct CharUpdater(delegate*<ref byte, void> func)
    {
        public readonly delegate*<ref byte, void> Func = func;

        public void Invoke(ref byte @byte) => Func(ref @byte);
    }
}

// Reference: https://ocaml.org/manual/5.4/api/Char.html
namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.ByteChars
open Nemonuri.ByteChars.ByteSpans
open Nemonuri.OCamlDotNet.Primitives
open Microsoft.FSharp.NativeInterop
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Cop = Microsoft.FSharp.Core.Operators

module Char =

    type t = OCamlChar

    // reference: https://ocaml.org/manual/5.4/api/Char.Ascii.html
    module Ascii =

        open type Nemonuri.ByteChars.ByteCharTheory

        let min = ByteCharConstants.AsciiMinimum

        let max = ByteCharConstants.AsciiMaximum

        let is_valid  c = IsValid c

        let is_upper c = IsUpper c

        let is_lower  c = IsLower c

        let is_letter c = IsLetter c

        let is_alphanum  c = IsAlphanumeric c

        let is_white c = IsWhite c

        let is_blank  c = IsBlank c

        let is_graphic  c = IsGraphic c

        let is_print c = IsPrint c

        let is_control c = IsControl c

        let is_digit c = IsDecimalDigit c

        let digit_to_int c = 
            match is_digit c with
            | true -> UncheckedDecimalDigitToInteger c
            | false -> Exceptions.invalid_arg (Obs.Unsafe.stringOfArray "Char.Ascii.is_digit c is false."B)
        
        let digit_of_int (n: OCamlInt) = IntegerToDecimalDigit (Cop.abs (n % 10) |> Cop.byte)

        let is_hex_digit c = IsHexadecimalDigit c

        let hex_digit_to_int c =
            let ok, n = TryHexadecimalDigitToInteger(c)
            if not ok then Exceptions.invalid_arg (Obs.Unsafe.stringOfArray "Char.Ascii.is_hex_digit c is false."B)
            n |> Cop.int
        
        let lower_hex_digit_of_int (n: OCamlInt) = IntegerToLowerHexadecimalDigit (Cop.byte n)

        let upper_hex_digit_of_int (n: OCamlInt) = IntegerToUpperHexadecimalDigit (Cop.byte n)

        let uppercase  c = ToUpperCase c

        let lowercase  c = ToLowerCase c


    let code (c: OCamlChar) : int = c |> Cop.int

    let chr (i: int) : OCamlChar = 
        try
            System.Convert.ToByte i
        with
            | :? System.OverflowException as oe -> Exceptions.invalid_arg (Obs.stringOfDotNetString oe.Message)

    let escaped a = 
        let bs = DotNetSpans.NativePtrToSpan(NativePtr.stackalloc<OCamlChar> 1,1)
        bs[0] <- a
        ByteSpans.escaped(bs) |> Obs.Unsafe.stringOfArraySegment
    
    let compare (l: t) (r: t) = l.CompareTo(r)

    let equal (l: t) (r: t) = l.Equals(r)

    let lowercase_ascii c = Ascii.lowercase

    let uppercase_ascii c = Ascii.uppercase

    let hash (c: t) = c.GetHashCode()

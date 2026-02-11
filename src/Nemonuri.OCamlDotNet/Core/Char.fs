#nowarn "62" // ML compatibility

/// - Reference: https://ocaml.org/manual/5.4/api/Char.html
module Nemonuri.OCamlDotNet.Char
open Nemonuri.OCamlDotNet

type private Cc = Nemonuri.ByteChars.ByteCharConstants
type private Cth = Nemonuri.ByteChars.ByteCharTheory
type private Ia = System.Collections.Immutable.ImmutableArray

//// <category name="Characters">

/// An alias for the type of characters.
type t = char

/// Return the integer code of the argument.
let code (c: char) : int = c |> Operators.int

/// Return the character with the given integer code.
/// 
/// Raises `Invalid_argument` if the argument is outside the range \[`0x00`;`0xFF`\].
let chr (i: int) : char = 
    try
        System.Convert.ToByte i
    with
        | :? System.OverflowException as oe -> Forward.invalid_arg oe.Message

/// Return a string representing the given character, with special characters escaped following the lexical conventions of OCaml. 
/// All characters outside the ASCII printable range \[`0x20`;`0x7E`\] are escaped, as well as backslash, double-quote, and single-quote.
let escaped (c: char) : string =
    let bs = Cc.AsciiBackslash
    let inline divRem10 n = let d = 10uy in n / d, Cth.Modulus(n, d) |> Cth.UncheckedIntegerToDecimalDigit
    match c with
    | Cc.AsciiBackslash | Cc.AsciiDoubleQuote | Cc.AsciiSingleQuote -> Ia.Create(bs, c)
    | Cc.AsciiLineFeed -> Ia.Create(bs, byte 'n')
    | Cc.AsciiCarriageReturn -> Ia.Create(bs, byte 'r')
    | Cc.AsciiHorizontalTabulation -> Ia.Create(bs, byte 't')
    | Cc.AsciiBackspace -> Ia.Create(bs, byte 'b')
    | Cc.AsciiSpace -> Ia.Create(bs, byte ' ')
    | c0 when Cc.AsciiPrintableMinimum <= c0 && c0 <= Cc.AsciiPrintableMaximum -> Ia.Create(c0)
    | _ -> 
        let q2, r2 = divRem10 c
        let q1, r1 = divRem10 q2
        let _, r0 = divRem10 q1
        Ia.Create(bs, r0, r1, r2)

//// </category>

module rec Ascii =

    //// <category name="Characters">

    /// min is '\x00'.
    let [<Literal>] min : char = Cc.AsciiMinimum

    /// max is '\x7F'.
    let [<Literal>] max : char = Cc.AsciiMaximum

    //// </category>
    
    //// <category name="Predicates">
    
    /// is_valid c is true if and only if c is an ASCII character, that is a byte in the range \[Char.Ascii.min;Char.Ascii.max\].
    let is_valid (c: char) : bool = Cth.IsValid c

    /// is_upper c is true if and only if c is an ASCII uppercase letter 'A' to 'Z', that is a byte in the range \[0x41;0x5A\].
    let is_upper (c: char) : bool = Cth.IsUpper c

    /// is_lower c is true if and only if c is an ASCII lowercase letter 'a' to 'z', that is a byte in the range \[0x61;0x7A\].
    let is_lower (c: char) : bool = Cth.IsLower c

    /// is_letter c is Char.Ascii.is_lower c || Char.Ascii.is_upper c.
    let is_letter (c: char) : bool = Cth.IsLetter c

    /// is_alphanum c is Char.Ascii.is_letter c || Char.Ascii.is_digit c.
    let is_alphanum (c: char) : bool = Cth.IsAlphanumeric c

    /// is_white c is true if and only if c is an ASCII white space character, 
    /// that is one of tab '\t' (0x09), newline '\n' (0x0A), vertical tab (0x0B), form feed (0x0C), carriage return '\r' (0x0D) or space ' ' (0x20),
    let is_white (c: char) : bool = Cth.IsWhite c
    
    /// is_blank c is true if and only if c is an ASCII blank character, that is either space ' ' (0x20) or tab '\t' (0x09).
    let is_blank (c: char) : bool = Cth.IsBlank c
    
    /// is_graphic c is true if and only if c is an ASCII graphic character, that is a byte in the range \[0x21;0x7E\].
    let is_graphic (c: char) : bool = Cth.IsGraphic c

    /// is_print c is Char.Ascii.is_graphic c || c = ' '.
    let is_print (c: char) : bool = Cth.IsPrint c
        
    /// is_control c is true if and only if c is an ASCII control character, that is a byte in the range \[0x00;0x1F\] or 0x7F.
    let is_control (c: char) : bool = Cth.IsControl c


    //// <category name="Decimal digits">
    
    /// is_digit c is true if and only if c is an ASCII decimal digit '0' to '9', that is a byte in the range \[0x30;0x39\].
    let is_digit (c: char) : bool = Cth.IsDecimalDigit c

    /// digit_to_int c is the numerical value of a digit that satisfies Char.Ascii.is_digit. 
    /// Raises Invalid_argument if Char.Ascii.is_digit c is false.
    let digit_to_int (c: char) : int = 
        match is_digit c with
        | true -> Cth.UncheckedDecimalDigitToInteger c |> code
        | false -> Forward.invalid_arg "Char.Ascii.is_digit c is false."

    /// digit_of_int n is an ASCII decimal digit for the decimal value abs (n mod 10).    
    let digit_of_int (n: int) : char = Cth.UncheckedIntegerToDecimalDigit(Operators.byte n)

    //// </category>
    
    //// <category name="Hexadecimal digits">

    /// is_hex_digit c is true if and only if c is an ASCII hexadecimal digit '0' to '9', 'a' to 'f' or 'A' to 'F', 
    /// that is a byte in one of the ranges \[0x30;0x39\], \[0x41;0x46\], \[0x61;0x66\].
    let is_hex_digit (c: char) : bool = Cth.IsHexadecimalDigit c

    /// hex_digit_to_int c is the numerical value of a digit that satisfies Char.Ascii.is_hex_digit. 
    /// Raises Invalid_argument if Char.Ascii.is_hex_digit c is false.
    let hex_digit_to_int (c: char) : int = 
        let success, integer = Cth.TryHexadecimalDigitToInteger c
        if not success then Forward.invalid_arg "Char.Ascii.is_hex_digit c is false." else
        Operators.int integer
    
    //// </category>
    
    //// <category name="Casing transforms">

    /// uppercase c is c with ASCII characters 'a' to 'z' respectively mapped to uppercase characters 'A' to 'Z'. Other characters are left untouched.
    let uppercase (c: char) = Cth.ToUpperCase c

    /// lowercase c is c with ASCII characters 'A' to 'Z' respectively mapped to lowercase characters 'a' to 'z'. Other characters are left untouched.
    let lowercase (c: char) = Cth.ToLowerCase c

    //// </category>


//// <category name="Predicates and comparisons">
//// See also the Char.Ascii module.

/// The comparison function for characters, with the same specification as compare. 
/// Along with the type t, this function compare allows the module Char to be passed as argument to the functors Set.Make and Map.Make.
let compare (left: t) (right: t) : int = left.CompareTo right

/// The equal function for chars.
let equal (left: t) (right: t) : bool = left.Equals right

//// </category>

//// <category name="Hashing">

let hash (c: t) : int = c.GetHashCode()

//// </category>
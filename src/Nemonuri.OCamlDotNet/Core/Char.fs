#nowarn "62" // ML compatibility

/// - Reference: https://ocaml.org/manual/5.4/api/Char.html
module Nemonuri.OCamlDotNet.Core.Char
open Nemonuri.OCamlDotNet.Core

module rec Ascii =

    type private L = Nemonuri.OCamlDotNet.LexicalTheory

    //// <category name="Characters">

    /// min is '\x00'.
    let min : char = char.MinValue

    /// max is '\x7F'.
    let max : char = char.MaxValue

    //// </category>
    
    //// <category name="Predicates">
    
    /// is_valid c is true if and only if c is an ASCII character, that is a byte in the range \[Char.Ascii.min;Char.Ascii.max\].
    let is_valid (c: char) : bool = min <= c && c <= max

    /// is_upper c is true if and only if c is an ASCII uppercase letter 'A' to 'Z', that is a byte in the range \[0x41;0x5A\].
    let is_upper (c: char) : bool = char.UpperA <= c && c <= char.UpperZ

    /// is_lower c is true if and only if c is an ASCII lowercase letter 'a' to 'z', that is a byte in the range \[0x61;0x7A\].
    let is_lower (c: char) : bool = char.LowerA <= c && c <= char.LowerZ

    /// is_letter c is Char.Ascii.is_lower c || Char.Ascii.is_upper c.
    let inline is_letter (c: char) : bool = is_lower c || is_upper c

    /// is_alphanum c is Char.Ascii.is_letter c || Char.Ascii.is_digit c.
    let inline is_alphanum (c: char) : bool = is_letter c || is_digit c

    /// is_white c is true if and only if c is an ASCII white space character, 
    /// that is one of tab '\t' (0x09), newline '\n' (0x0A), vertical tab (0x0B), form feed (0x0C), carriage return '\r' (0x0D) or space ' ' (0x20),
    let is_white (c: char) : bool = 
        match c.Value with
        | L.AsciiHorizontalTabulation | L.AsciiLineFeed | L.AsciiVerticalTabulation
        | L.AsciiFormFeed | L.AsciiCarriageReturn | L.AsciiSpace -> true
        | _ -> false
    
    /// is_blank c is true if and only if c is an ASCII blank character, that is either space ' ' (0x20) or tab '\t' (0x09).
    let is_blank (c: char) : bool =
        match c.Value with
        | L.AsciiSpace | L.AsciiHorizontalTabulation -> true
        | _ -> false
    
    /// is_graphic c is true if and only if c is an ASCII graphic character, that is a byte in the range \[0x21;0x7E\].
    let is_graphic (c: char) : bool = char.GraphicMinValue <= c && c <= char.GraphicMaxValue

    /// is_print c is Char.Ascii.is_graphic c || c = ' '.
    let is_print (c: char) : bool = is_graphic c || c = char.Space
        
    /// is_control c is true if and only if c is an ASCII control character, that is a byte in the range \[0x00;0x1F\] or 0x7F.
    let is_control (c: char) : bool = let v = c.Value in (0x00uy <= v && v <= 0x1fuy) || v = 0x7fuy


    //// <category name="Decimal digits">
    
    /// is_digit c is true if and only if c is an ASCII decimal digit '0' to '9', that is a byte in the range \[0x30;0x39\].
    let is_digit (c: char) : bool = char.Digit0 <= c && c <= char.Digit9

    let private digit_to_int_core (c: char) : int = c.Value - char.Digit0.Value |> Operators.int

    /// digit_to_int c is the numerical value of a digit that satisfies Char.Ascii.is_digit. 
    /// Raises Invalid_argument if Char.Ascii.is_digit c is false.
    let digit_to_int (c: char) : int = 
        match is_digit c with
        | true -> digit_to_int_core c
        | false -> Forward.invalid_arg "Char.Ascii.is_digit c is false." |> raise

    let private digit_of_int_core' (n: int) (baseChar: char) = n + Operators.int baseChar.Value |> Operators.byte |> char

    let inline private digit_of_int_core (n: int) = digit_of_int_core' n char.Digit0

    /// digit_of_int n is an ASCII decimal digit for the decimal value abs (n mod 10).    
    let digit_of_int (n: int) : char = abs (n % 10) |> digit_of_int_core

    //// </category>
    
    //// <category name="Hexadecimal digits">
    
    let private is_lower_hex_digit c = char.LowerA <= c && c <= char.LowerF

    let private is_upper_hex_digit c = char.UpperA <= c && c <= char.UpperF

    /// is_hex_digit c is true if and only if c is an ASCII hexadecimal digit '0' to '9', 'a' to 'f' or 'A' to 'F', 
    /// that is a byte in one of the ranges \[0x30;0x39\], \[0x41;0x46\], \[0x61;0x66\].
    let is_hex_digit (c: char) : bool = is_digit c || is_lower_hex_digit c || is_upper_hex_digit c

    /// hex_digit_to_int c is the numerical value of a digit that satisfies Char.Ascii.is_hex_digit. 
    /// Raises Invalid_argument if Char.Ascii.is_hex_digit c is false.
    let hex_digit_to_int (c: char) : int = 
        match is_digit c with
        | true -> digit_to_int_core c
        | false -> 
        match is_lower_hex_digit c with
        | true -> c.Value - char.LowerA.Value |> Operators.int |> (+) 10
        | false ->
        match is_upper_hex_digit c with
        | true -> c.Value - char.UpperA.Value |> Operators.int |> (+) 10
        | false ->
            assert (not (is_hex_digit c))
            Forward.invalid_arg "Char.Ascii.is_hex_digit c is false." |> raise
    
    let private x_hex_digit_of_int (n: int) (baseChar: char) : char = 
        match abs (n % 16) with
        | v when v < 10 -> v |> digit_of_int_core
        | v -> digit_of_int_core' (v - 10) baseChar

    /// lower_hex_digit_of_int n is a lowercase ASCII hexadecimal digit for the hexadecimal value abs (n mod 16).
    let lower_hex_digit_of_int n = x_hex_digit_of_int n char.LowerA

    /// upper_hex_digit_of_int n is an uppercase ASCII hexadecimal digit for the hexadecimal value abs (n mod 16).
    let upper_hex_digit_of_int n = x_hex_digit_of_int n char.UpperA

    //// </category>
    
    //// <category name="Casing transforms">
    
    let private A_to_a_distance = char.LowerA.Value - char.UpperA.Value // int32 'a' > int32 'A'

    /// uppercase c is c with ASCII characters 'a' to 'z' respectively mapped to uppercase characters 'A' to 'Z'. Other characters are left untouched.
    let uppercase (c: char) =
        match is_lower c with
        | true -> c.Value - A_to_a_distance |> char
        | false -> c

    /// lowercase c is c with ASCII characters 'A' to 'Z' respectively mapped to lowercase characters 'a' to 'z'. Other characters are left untouched.
    let lowercase (c: char) =
        match is_upper c with
        | true -> c.Value + A_to_a_distance |> char
        | false -> c

    //// </category>


//// <category name="Characters">

/// An alias for the type of characters.
type t = char

/// Return the integer code of the argument.
let code (c: char) : int = c.Value |> Operators.int

/// Return the character with the given integer code.
/// 
/// Raises `Invalid_argument` if the argument is outside the range \[`0x00`;`0xFF`\].
let chr (i: int) : char = 
    try
        System.Convert.ToByte i |> char
    with
        | :? System.OverflowException as oe -> raise (Forward.invalid_arg oe.Message)

/// Return a string representing the given character, with special characters escaped following the lexical conventions of OCaml. 
/// All characters outside the ASCII printable range \[`0x20`;`0x7E`\] are escaped, as well as backslash, double-quote, and single-quote.
let escaped (c: char) : string = c.ToEscaped()

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
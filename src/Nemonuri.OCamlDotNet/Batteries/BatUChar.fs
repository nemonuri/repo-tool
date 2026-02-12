/// Unicode characters.
/// This module implements Unicode characters.
/// Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUChar.html
module Nemonuri.OCamlDotNet.Batteries.BatUChar
open Nemonuri.OCamlDotNet

type t = System.Text.Rune

/// Aliases of **type** `t`
type uchar = t

type private latin1Char = byte

exception Out_of_range = System.ArgumentOutOfRangeException

/// `code u` returns the Unicode code number of `u`.
let inline code (u: t) : int = u.Value

/// Alias of `code`
let inline int_of u = code u

/// `chr n` returns the Unicode character with the code number `n`. 
/// If n does not lay in the valid range of Unicode or designates a surrogate character, raises Out_of_range
let inline chr (n: int) : t = System.Text.Rune n

/// Alias of `chr`
let inline of_int n = chr n

/// Equality by code point comparison
let inline eq (left: t) (right: t) : bool = left.Equals right

/// `compare u1 u2` returns, a value > 0 if `u1` has a larger Unicode code number than `u2`, 0 if `u1` and `u2` are the same Unicode character, a value < 0 if `u1` has a smaller Unicode code number than `u2`.
let inline compare (left: t) (right: t) : int = left.CompareTo right

/// **true** if the char is a regular ascii char, i.e. if its code is <= 127
let inline is_ascii (u: t) : bool = u.IsAscii

(**
### Layout

- [ISO/IEC 8859-1](https://en.wikipedia.org/wiki/ISO/IEC_8859-1#Code_page_layout)
- [Latin-1 Supplement](https://en.wikipedia.org/wiki/Latin-1_Supplement#Compact_table)
*)

/// `of_char c` returns the Unicode character of the Latin-1 character `c`
let of_char (c: char) : t = Char.code c |> chr

/// `char_of u` returns the Latin-1 representation of `u`. If `u` can not be represented by Latin-1, raises Out_of_range
let char_of (u: t) : char =
    try
        Char.chr u.Value
    with
        | Stdlib.Invalid_argument -> raise Out_of_range

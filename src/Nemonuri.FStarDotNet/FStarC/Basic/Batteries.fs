namespace Nemonuri.OCamlDotNet

/// Unicode characters.
/// This module implements Unicode characters.
/// Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUChar.html
module BatUChar =

    type t = System.Text.Rune

    /// Aliases of **type** `t`
    type uchar = t

    type private latin1Char = byte

    exception Out_of_range

    /// `code u` returns the Unicode code number of `u`.
    let inline code (u: t) : int = u.Value

    /// Alias of `code`
    let inline int_of u = code u

    /// `chr n` returns the Unicode character with the code number `n`. 
    /// If n does not lay in the valid range of Unicode or designates a surrogate character, raises Out_of_range
    let inline chr (n: int) : t = 
        try System.Text.Rune n with | :? System.ArgumentOutOfRangeException -> raise Out_of_range
    
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
    let of_char (c: latin1Char) : t =
        if c < 128uy then
            int32 c |> chr
        else
            0x0080 + (int32 c - 128) |> chr

    /// `char_of u` returns the Latin-1 representation of `u`. If `u` can not be represented by Latin-1, raises Out_of_range
    let char_of (u: t) : latin1Char =
        let codePoint = uint32 u.Value
        if codePoint < 128u then
            uint8 codePoint
        elif 0x0080u <= codePoint && codePoint < 0x0080u + 128u then
            codePoint - 0x0080u + 128u |> uint8
        else
            raise Out_of_range


/// UTF-8 encoded Unicode strings. The type is normal string.
/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUTF8.html
module BatUTF8 =

    open type System.Text.Encoding
    open type Nemonuri.NetStandards.Text.EncodingExtensions

    /// UTF-8 encoded Unicode strings. The type is normal string.
    type t = System.ReadOnlySpan<byte>

    exception Malformed_code

    /// validate s successes if s is valid UTF-8, otherwise raises Malformed_code. 
    /// Other functions assume strings are valid UTF-8, so it is prudent to test their validity for strings from untrusted origins.
    let validate (str: t) : unit =
        try
            UTF8.GetCharCount str |> ignore
        with
            | :? System.Text.DecoderFallbackException -> raise Malformed_code


#nowarn "9" // Uses of this construct may result in the generation of unverifiable .NET IL code.

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
    open CommunityToolkit.HighPerformance.Buffers
    open FSharp.NativeInterop
    open type System.Buffers.BuffersExtensions

    type private Bw = byte ArrayPoolBufferWriter

    /// UTF-8 encoded Unicode strings. The type is normal string.
    type t = byte array

    exception Malformed_code

    /// `length s` returns the number of Unicode characters contained in s
    let inline length (s: t) : int = UTF8.GetCharCount s

    /// `validate s` successes if s is valid UTF-8, otherwise raises Malformed_code. 
    /// Other functions assume strings are valid UTF-8, so it is prudent to test their validity for strings from untrusted origins.
    let validate (s: t) : unit =
        try
            length s |> ignore
        with
            | :? System.Text.DecoderFallbackException -> raise Malformed_code

    let [<Literal>] private spanSize = 4

    /// `init len f` returns a new string which contains `len` Unicode characters. The i-th Unicode character is initialized by `f i`
    let init (len: int) (f: int -> BatUChar.t) : t =
        use bw = new Bw()

        for i in 0 .. len-1 do
            let newRune = f i
            let span = System.Span<byte>(NativePtr.stackalloc<byte> spanSize |> NativePtr.toVoidPtr, spanSize)
            let writtenLength = newRune.EncodeToUtf8 span
            bw.Write<byte>(span.Slice(0, writtenLength))           
        
        bw.WrittenSpan.ToArray()
    
    /// Positions in the string represented by the number of bytes from the head. The location of the first character is 0
    type index = int

/// String operations.
/// 
/// Given a string s of length l, we call character number in s the index of a character in s. 
/// Indexes start at 0, and we will call a character number valid in s if it falls within the range \[0...l-1\]. 
/// A position is the point between two characters or at the beginning or end of the string. We call a position valid in s if it falls within the range \[0...l\]. 
/// Note that character number n is between positions n and n+1.
/// 
/// Two parameters start and len are said to designate a valid substring of s if len >= 0 and start and start+len are valid positions in s.
/// 
/// This module replaces Stdlib's String module.
/// 
/// If you're going to do a lot of string slicing, BatSubstring might be a useful module to represent slices of strings, as it doesn't allocate new strings on every operation.
/// 
/// - Author(s): Xavier Leroy (base library), Nicolas Cannasse, David Teller, Edgar Friendly
/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatString.html
module BatString =

    type t = string

    let split_on_string (sep: string) (s: string) : string list = 
        let splited = s.Split([|sep|], System.StringSplitOptions.None) |> List.ofArray
        if s.EndsWith sep then splited @ [""] else splited
    
    let inline compare (left: t) (right: t) = left.CompareTo right

    /// `String.concat sep sl` concatenates the list of strings sl, inserting the separator string sep between each.
    let inline concat (sep: string) (sl: string list) : string = t.Join(sep, values = sl)

/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatList.html
module BatList =

    type 'a t = list<'a>

    /// `cons h t` returns the list starting with `h` and continuing as `t`.
    let inline cons h t = List.Cons(h, t)

    let inline append l1 l2 = List.append l1 l2

    let rec concat (ls: 'a list list) : 'a list =
        match ls with
        | [] -> []
        | hd::tl -> append hd (concat tl)

    let inline flatten ls = concat ls

    let inline map f l = List.map f l
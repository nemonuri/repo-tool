
/// - Reference: https://ocaml.org/manual/5.4/api/String.html
module Nemonuri.OCamlDotNet.String
open Nemonuri.OCamlDotNet
open type System.MemoryExtensions

type private Sth = Nemonuri.ByteChars.ByteStringTheory

/// The type for strings.
type t = string

let private is_invalid_length n = n < 0 || n > Sys.max_string_length

let private throwOutOfRange message = Forward.invalid_arg message

let private (|OutOfRange|_|) (e: exn) =
    match e with
    | :? System.ArgumentOutOfRangeException as e0 -> Some e0.Message
    | _ -> None

/// make n c is a string of length n with each index holding the character c.
///
/// Raises Invalid_argument if n < 0 or n > Sys.max_string_length.
let make (n: int) (c: char) : string = 
    try
        Sth.FromInitialValue(n, c)
    with
        | OutOfRange msg -> throwOutOfRange msg

/// init n f is a string of length n with index i holding the character f i (called in increasing index order).
/// 
/// Since 4.02
/// Raises Invalid_argument if n < 0 or n > Sys.max_string_length.
let init (n: int) (f: int -> char) : string =
    try
        Sth.FromInitializer(n, f)
    with
        | OutOfRange msg -> throwOutOfRange msg

/// The empty string.
let empty : string = Sth.Empty

/// length s is the length (number of bytes/characters) of s.
let length (s: string) : int = s.Length

//// <category name="Concatenating">

/// cat s1 s2 concatenates s1 and s2 (s1 ^ s2).
///
/// Since 4.13
/// Raises Invalid_argument if the result is longer than Sys.max_string_length bytes.
let cat (s1: string) (s2: string) : string = 
    try
        Sth.Concat(s1, s2)
    with
        | OutOfRange msg -> throwOutOfRange msg

let (^) s1 s2 : string = cat s1 s2

/// concat sep ss concatenates the list of strings ss, inserting the separator string sep between each.
///
/// Raises Invalid_argument if the result is longer than Sys.max_string_length bytes.
let concat (sep: string) (ss: string list) : string =
    try
        Sth.Join(sep, ss)
    with
        | OutOfRange msg -> throwOutOfRange msg

//// </category>

//// <category name="Predicates and comparisons">

/// equal s0 s1 is true if and only if s0 and s1 are character-wise equal.
///
/// Since 4.03 (4.05 in StringLabels)
let equal (s0: t) (s1: t) : bool = s0.AsSpan().SequenceEqual(s1.AsSpan())

/// compare s0 s1 sorts s0 and s1 in lexicographical order. compare behaves like compare on strings but may be more efficient.
let compare (s0: t) (s1: t) : int = s0.AsSpan().SequenceCompareTo(s1.AsSpan())





//// </category>
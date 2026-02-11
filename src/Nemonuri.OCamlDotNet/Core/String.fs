
/// - Reference: https://ocaml.org/manual/5.4/api/String.html
module Nemonuri.OCamlDotNet.String
open Nemonuri.OCamlDotNet

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


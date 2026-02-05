
/// - Reference: https://ocaml.org/manual/5.4/api/String.html
module Nemonuri.OCamlDotNet.Core.String
open Nemonuri.OCamlDotNet.Core

/// The type for strings.
type t = string

let private is_invalid_length n = n < 0 || n > Sys.max_string_length

let private throwOutOfRange() = Forward.invalid_arg "out of range" |> raise

/// make n c is a string of length n with each index holding the character c.
///
/// Raises Invalid_argument if n < 0 or n > Sys.max_string_length.
let make (n: int) (c: char) : string =
    if is_invalid_length n then throwOutOfRange() else
    Array.create n c |> string

/// init n f is a string of length n with index i holding the character f i (called in increasing index order).
/// 
/// Since 4.02
/// Raises Invalid_argument if n < 0 or n > Sys.max_string_length.
let init (n: int) (f: int -> char) : string =
    if is_invalid_length n then throwOutOfRange() else
    Array.init n f |> string

/// The empty string.
let empty = Array.empty |> string




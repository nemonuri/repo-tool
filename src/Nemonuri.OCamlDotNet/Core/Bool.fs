
/// - Reference: https://ocaml.org/manual/5.4/api/Unit.html
module Nemonuri.OCamlDotNet.Bool
open Nemonuri.OCamlDotNet
open type Nemonuri.ByteChars.ByteStringTheory

/// The type of booleans (truth values).
///
/// The constructors false and true are included here so that they have paths, but they are not intended to be used in user-defined data types.
type t = bool

/// not b is the boolean negation of b.
let inline not (b: bool) = Microsoft.FSharp.Core.Operators.not b

/// logand b1 b2 is true if and only if b1 and b2 are both true.
let inline logand (b1: bool) (b2: bool) : bool = b1 && b2

/// logor b1 b2 is true if and only if either b1 or b2 is true.
let inline logor (b1: bool) (b2: bool) : bool = b1 || b2

/// logxor b1 b2 is true if exactly one of b1 and b2 is true.
let inline logxor (b1: bool) (b2: bool) : bool = b1 <> b2

/// equal b0 b1 is true if and only if b0 and b1 are both true or both false.
let inline equal (b0: bool) (b1: bool) : bool = Forward.equal b0 b1

/// compare b0 b1 is a total order on boolean values. false is smaller than true.
let inline compare (b0: bool) (b1: bool) : int = Forward.compare b0 b1

/// to_string b is "true" if b is true and "false" if b is false.
let to_string (b: bool) : string =
    match b with
    | true -> FromByteSpan "true"B
    | false -> FromByteSpan "false"B

let hash (b: bool) : int = b.GetHashCode()
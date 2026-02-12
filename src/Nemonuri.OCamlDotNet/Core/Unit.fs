
/// - Reference: https://ocaml.org/manual/5.4/api/Unit.html
module Nemonuri.OCamlDotNet.Unit
open Nemonuri.OCamlDotNet

/// The unit type.
///
/// The constructor () is included here so that it has a path, but it is not intended to be used in user-defined data types.
type t = unit

/// equal u1 u2 is true.
let inline equal (u1 : t) (u2 : t) = Forward.equal u1 u2

/// compare u1 u2 is 0.
let inline compare (u1 : t) (u2 : t) = Forward.compare u1 u2

/// to_string b is "()".
let to_string (b: t) : string = !-"()"B

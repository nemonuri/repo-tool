/// - Reference: https://ocaml.org/manual/5.4/api/Int.html

/// Integer values.
///
/// Integers are Sys.int_size bits wide and use two's complement representation. 
/// All operations are taken modulo 2^{Sys.int_size}. They do not fail on overflow.
module Nemonuri.OCamlDotNet.Int
open Nemonuri.OCamlDotNet

/// The type for integer values.
type t = int


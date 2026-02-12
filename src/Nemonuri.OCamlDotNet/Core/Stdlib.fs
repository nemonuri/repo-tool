
/// - Reference: https://ocaml.org/manual/5.4/api/Stdlib.html
module Nemonuri.OCamlDotNet.Stdlib
open Nemonuri.OCamlDotNet

let raise = Microsoft.FSharp.Core.Operators.raise

exception Invalid_argument = Forward.Invalid_argument

let (|Invalid_argument|_|) = Forward.(|Invalid_argument|_|)

let failwith = Microsoft.FSharp.Core.Operators.failwith

let invalid_arg = Forward.invalid_arg

exception Division_by_zero = Forward.Division_by_zero
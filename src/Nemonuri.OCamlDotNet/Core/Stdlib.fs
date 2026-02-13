
/// - Reference: https://ocaml.org/manual/5.4/api/Stdlib.html
module Nemonuri.OCamlDotNet.Stdlib
open Nemonuri.OCamlDotNet

exception Invalid_argument = Forward.Invalid_argument

exception Division_by_zero = Forward.Division_by_zero

exception Failure = Forward.Failure

let raise = Microsoft.FSharp.Core.Operators.raise

let (|Invalid_argument|_|) = Forward.(|Invalid_argument|_|)

let failwith (message: string) = Forward.failwith message

let invalid_arg (message: string) = Forward.invalid_arg message


//// <category name="String conversion functions">

let string_of_bool (b: bool) : string = Bool.to_string b

//// <category/>
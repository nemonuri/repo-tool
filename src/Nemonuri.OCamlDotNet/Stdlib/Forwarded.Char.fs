// Reference: https://ocaml.org/manual/5.4/api/Char.html
namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.Operations

module Char =

    type t = OCamlChar

    let code (c: OCamlChar) : int = c |> Operators.int

    let chr (i: int) : OCamlChar = 
        try
            System.Convert.ToByte i
        with
            | :? System.OverflowException as oe -> Exceptions.invalid_arg (OCamlStrings.ofDotNetString oe.Message)

/// - Reference: https://ocaml.org/manual/5.4/api/Obj.html
module Nemonuri.OCamlDotNet.Obj
open Nemonuri.OCamlDotNet

open System.Runtime.CompilerServices

type t = System.Object

type raw_data = nativeint

let repr (a: 'a) : t = Microsoft.FSharp.Core.Operators.box a

let obj (o: t) : 'a = Microsoft.FSharp.Core.Operators.unbox<'a> o

let magic (a: 'a) : 'b = repr a |> obj


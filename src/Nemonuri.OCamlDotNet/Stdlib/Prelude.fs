namespace Nemonuri.OCamlDotNet.Core

[<AutoOpen>]
module Prelude =

    type char = Nemonuri.OCamlDotNet.Char
    type int = Core.int
    type string = Nemonuri.OCamlDotNet.String
    type bool = Core.bool

module internal Forward =

    exception Invalid_argument = System.ArgumentException

    [<CompiledNameAttribute("GetMessageOfInvalidArgumentOrNone")>]
    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? Invalid_argument as inv -> Some inv.Message
        | _ -> None

    let invalid_arg message = System.ArgumentException message
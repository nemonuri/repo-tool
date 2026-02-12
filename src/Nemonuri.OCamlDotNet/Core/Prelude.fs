namespace Nemonuri.OCamlDotNet

[<AutoOpen>]
module Prelude =

    type char = Microsoft.FSharp.Core.byte
    type int = Microsoft.FSharp.Core.int
    type string = System.Collections.Immutable.ImmutableArray<char>
    type bool = Microsoft.FSharp.Core.bool
    type unit = Microsoft.FSharp.Core.unit

module internal Forward =

    exception Invalid_argument = System.ArgumentException

    [<CompiledNameAttribute("MatchInvalidArgument")>]
    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? Invalid_argument as inv -> Some inv.Message
        | _ -> None

    let invalid_arg message = System.ArgumentException message |> raise

    let inline equal a0 a1 = Microsoft.FSharp.Core.LanguagePrimitives.GenericEquality a0 a1

    let inline compare a0 a1 = Microsoft.FSharp.Core.LanguagePrimitives.GenericComparison a0 a1
namespace Nemonuri.FStarDotNet

[<AutoOpen>]
module Prelude =

(**
- Reference: https://github.com/dotnet/fsharp/blob/c4ccf58cd107213374237e3910288d4b6b183abb/src/FSharp.Core/fslib-extra-pervasives.fs#L333
*)
    [<assembly: AutoOpen("Nemonuri.FStarDotNet")>]
    [<assembly: AutoOpen("Nemonuri.OCamlDotNet")>]
    [<assembly: AutoOpen("Nemonuri.OCamlDotNet.Stdlib")>]
    [<assembly: AutoOpen("Nemonuri.OCamlDotNet.Zarith")>]
    [<assembly: AutoOpen("Nemonuri.OCamlDotNet.Batteries")>]
    do()

namespace Nemonuri.FStarDotNet.Primitives.Operations

open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Primitives.Abstractions

/// reference: https://github.com/fsprojects/FSharpPlus/blob/v1.9.1/src/FSharpPlus/Operators.fs
module Functions =

    let inline flip f x y = f y x

    module Operators =

        let inline (</) x = (|>) x

        let inline (/>) x = flip x
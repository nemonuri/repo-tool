// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Char.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.FStarDotNet.Primitives.FStarKinds

[<RequireQualifiedAccess>]
module Char =

    type char = Prims.Type0<OCamlChar>


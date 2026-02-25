// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Char.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet.Primitives.Abbreviations
module Flv = Nemonuri.FStarDotNet.Primitives.FStarLiftedValues

[<RequireQualifiedAccess>]
module Char =

    type char = Fv<Core.char>


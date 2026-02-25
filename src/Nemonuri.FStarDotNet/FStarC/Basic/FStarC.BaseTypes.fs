// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.BaseTypes.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_BaseTypes.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet.Primitives.Abbreviations
module Flv = Nemonuri.FStarDotNet.Primitives.FStarLiftedValues

[<RequireQualifiedAccess>]
module BaseTypes =

    /// This module aggregates commonly used primitive type constants into
    /// a single module, providing abbreviations for them.

    type char = Fv<Core.char>
    type float = Fv<Core.float32>
    type double = Fv<Core.double>
    type byte = Fv<Core.byte>
    type int8 = Fv<System.SByte>
    type uint8 = Fv<System.Byte>
    type int16 = Fv<System.Int16>
    type uint16 = Fv<System.UInt16>
    type int32 = Fv<System.Int32>
    type uint32 = Fv<System.UInt32>
    type int64 = Fv<System.Int64>
    type uint64 = Fv<System.UInt64>

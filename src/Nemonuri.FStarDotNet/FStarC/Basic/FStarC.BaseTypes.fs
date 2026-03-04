// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.BaseTypes.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_BaseTypes.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet

[<RequireQualifiedAccess>]
module BaseTypes =

    /// This module aggregates commonly used primitive type constants into
    /// a single module, providing abbreviations for them.

    type char = FStar.Char.char
    type float = Prims.Type0<Core.float32>
    type double = Prims.Type0<Core.double>
    type byte = Prims.Type0<Core.byte>
    type int8 = Prims.Type0<System.SByte>
    type uint8 = Prims.Type0<System.Byte>
    type int16 = Prims.Type0<System.Int16>
    type uint16 = Prims.Type0<System.UInt16>
    type int32 = Prims.Type0<System.Int32>
    type uint32 = Prims.Type0<System.UInt32>
    type int64 = Prims.Type0<System.Int64>
    type uint64 = Prims.Type0<System.UInt64>

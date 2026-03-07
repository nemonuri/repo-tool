// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.BaseTypes.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_BaseTypes.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet

module BaseTypes =

    /// This module aggregates commonly used primitive type constants into
    /// a single module, providing abbreviations for them.

    type char = FStar.Char.char
    type float = Core.float32
    type double = Core.double
    type byte = Core.byte
    type int8 = System.SByte
    type uint8 = System.Byte
    type int16 = System.Int16
    type uint16 = System.UInt16
    type int32 = System.Int32
    type uint32 = System.UInt32
    type int64 = System.Int64
    type uint64 = System.UInt64

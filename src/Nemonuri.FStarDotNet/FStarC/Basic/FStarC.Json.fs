// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Json.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Json.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarC.Effect

module Json =

    type json =
    | JsonNull
    | JsonBool of bool
    | JsonInt of Prims.int
    | JsonStr of Prims.string
    | JsonList of list<json>
    | JsonAssoc of list<Prims.string * json>

#if false
    val json_of_string : string -> option json
    val string_of_json : json -> string
#endif

    exception UnsupportedJson

    /// val json_of_string : string -> option json
    // let json_of_string (str: Prims.string)
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Json.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Json.ml

namespace Nemonuri.FStarDotNet.FStarC

open System.Text.Json
open System.Text.Json.Nodes
open System.Collections.Generic
open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.ByteChars.Json
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

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

    exception private UnsupportedJson

#if false
    let private foldJsonElementArray (je: JsonElement) (f: 'acc -> JsonElement -> 'acc) (init: 'acc) =
        let rec aux (je: JsonElement) (f: 'acc -> JsonElement -> 'acc) (len: int) (stepI: int) (stepAcc: 'acc) =
            if len <= stepI then stepAcc else
            let nextAcc = f stepAcc (je[stepI]) in
            aux je f len (stepI+1) nextAcc
        in
        let len = je.GetArrayLength() in
        let i = 0 in
        aux je f len i init
#endif

    let private foldEnumerator (e: #IEnumerator<'elem>) (f: 'acc -> 'elem -> 'acc) (init: 'acc) =
        let mutable me = e in
        let rec aux (f: 'acc -> 'elem -> 'acc) (stepOk: bool) (stepAcc: 'acc) =
            if not stepOk then stepAcc else
            let nextAcc = f stepAcc me.Current in
            aux f (me.MoveNext()) nextAcc
        in
        aux f (me.MoveNext()) init

    let private json_of_jsonElement (je: JsonElement) : option<json> =
        let rec aux (je: JsonElement) =
            match je.ValueKind with
            | JsonValueKind.Null -> JsonNull
            | JsonValueKind.True -> JsonBool true
            | JsonValueKind.False -> JsonBool false
            | JsonValueKind.Number -> 
                let ok, i32 = je.TryGetInt32() in
                if not ok then raise UnsupportedJson else
                JsonInt (Z.of_int i32)
            | JsonValueKind.String -> je.GetString() |> Obs.stringOfDotNetString |> JsonStr
            | JsonValueKind.Array -> 
                foldEnumerator (je.EnumerateArray()) (fun acc child -> (aux child)::acc) [] |> JsonList
            | JsonValueKind.Object -> 
                foldEnumerator (je.EnumerateObject()) (fun acc jp -> (Obs.stringOfDotNetString jp.Name, aux jp.Value)::acc) []
                |> JsonAssoc
            | _ -> raise UnsupportedJson
        in
        try Some (aux je) with UnsupportedJson -> None

    let rec private jsonNode_of_json (js: json) : JsonNode | null =
        match js with
        | JsonNull -> null
        | JsonBool b -> JsonValue.Create b
        | JsonInt i -> JsonValue.Create (Z.to_int i)
        | JsonStr s -> JsonValue.Create (Obs.stringToDotNetString s)
        | JsonList l -> JsonArray(List.map jsonNode_of_json l |> Array.ofList)
        | JsonAssoc a -> 
            JsonObject(Seq.map (fun (str, js) -> KeyValuePair<_,_>(Obs.stringToDotNetString str, jsonNode_of_json js)) a)

    // val json_of_string : string -> option json
    let json_of_string (str: Prims.string) : option<json> =
        try
            let ok, jd = JsonDocumentTheory.TryParseUtf8Span(Obs.stringToReadOnlySpan str) in
            if not ok then None else json_of_jsonElement jd.RootElement
        with
            | :? JsonException -> None
    
    /// val string_of_json : json -> string
    let string_of_json (js: json) : Prims.string =
        match jsonNode_of_json js with
        | Null -> Nemonuri.OCamlDotNet.Forwarded.String.empty
        | NonNull v -> v.ToJsonString() |> Obs.stringOfDotNetString

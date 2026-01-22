namespace Nemonuri.RepoTools.FStar.BuildTasks

open System.IO
open System.Text.Json;
open System.Text.Json.Nodes;

module BoolOperationTheory =

    let inline (|&>) (left: bool) (right: bool) = left && right


module FStarConfigJsonTheory =

    open BoolOperationTheory

    [<Literal>]
    let Suffix = ".fst.config.json"

    [<Literal>]
    let DefaultGeneratorName = "Nemonuri.RepoTools.FStar"    

    let [<Literal>] CommentPropertyName = "_comment"

    let [<Literal>] FStarExePropertyName = "fstar_exe"

    let [<Literal>] OptionsPropertyName = "options"

    let [<Literal>] IncludeDirectoriesPropertyName = "include_dirs"

    let [<Literal>] CommentContentHeader = "This file is auto generated from"

    let [<Literal>] GeneratorDefaultPrefix = "sdk.g"

    let jsonNodeOptions : JsonNodeOptions =
        JsonNodeOptions(
            PropertyNameCaseInsensitive = false
        )

    let jsonDocumentOptions : JsonDocumentOptions =
        JsonDocumentOptions(
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        )

    let getFullPath directoryPath prefix = Path.Combine(directoryPath, prefix + Suffix)

    let isMaybeValidJsonObject (jo: JsonObject) : bool =
        jo.ContainsKey FStarExePropertyName
        |&> match jo[FStarExePropertyName].GetValueKind() with
            | JsonValueKind.String -> true
            | _ -> false
        |&> jo.ContainsKey OptionsPropertyName
        |&> match jo[OptionsPropertyName].GetValueKind() with
            | JsonValueKind.Array -> true
            | _ -> false
        |&> jo.ContainsKey IncludeDirectoriesPropertyName
        |&> match jo[IncludeDirectoriesPropertyName].GetValueKind() with
            | JsonValueKind.Array -> true
            | _ -> false

    let private tryParseToJsonObject (jsonText: string) =
        JsonNode.Parse(jsonText, jsonNodeOptions, jsonDocumentOptions)
        |> function
            | :? JsonObject as jo -> Some jo
            | _ -> None

    let isMaybeValidText jsonText =
        match tryParseToJsonObject jsonText with
        | Some jo -> isMaybeValidJsonObject jo
        | None -> false
    
    let private (|TextOfValidName|_|) (jsonFilePath: string) =
        match jsonFilePath.EndsWith Suffix with
        | true -> Some (File.ReadAllText jsonFilePath)
        | false -> None

    let isMaybeValidFile (jsonFilePath: string) : bool =
        match jsonFilePath with
        | TextOfValidName t -> isMaybeValidText t
        | _ -> false
    
    let isMaybeGeneratedJsonObject jo =
        isMaybeValidJsonObject jo
        |&> jo.ContainsKey CommentPropertyName
        |&> match jo[CommentPropertyName].GetValueKind() with
            | JsonValueKind.String -> true
            | _ -> false
        |&> match jo[CommentPropertyName].AsValue().GetValue<string>() with
            | Null _ -> false
            | NonNull str -> str.Trim().StartsWith CommentContentHeader
    
    let isMaybeGeneratedText jsonText =
        match tryParseToJsonObject jsonText with
        | Some jo -> isMaybeGeneratedJsonObject jo
        | None -> false
    
    let isMaybeGeneratedFile jsonFilePath =
        match jsonFilePath with
        | TextOfValidName t -> isMaybeGeneratedText t
        | _ -> false

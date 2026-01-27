namespace Nemonuri.RepoTools.FStar.BuildTasks

open System.Text.Json.Nodes;
open System.Runtime.ExceptionServices;
open System.Collections.Generic;

type FStarConfigJsonModel = 
    { FStarExe: string; Options: string array; IncludeDirectories: string array; Extra: JsonObject option }

module FStarConfigJsonModelTheory =

    open FStarConfigJsonTheory;
    type Pr = FStarConfigJsonTheory.ParseToJsonObjectResult

    [<RequireQualifiedAccess>]
    type ParseGeneratedToModelResult =
    | Success of FStarConfigJsonModel
    | NotJsonObject of JsonNode
    | JsonException of ExceptionDispatchInfo
    | NotGeneratedFStarConfigJson of JsonObject

    let isExtraProperty (kv: KeyValuePair<string, _>) : bool =
        match kv.Key with
        | FStarExePropertyName | OptionsPropertyName | IncludeDirectoriesPropertyName -> false
        | _ -> true

    let propertiesToJsonObject properties = JsonObject(properties, jsonNodeOptions)

    let parseGeneratedJsonObjectToModel (jo: JsonObject) : ParseGeneratedToModelResult =
        if isMaybeGeneratedJsonObject jo then
            let fstarExe = jo[FStarExePropertyName].AsValue().GetValue<string>()
            let options = jo[OptionsPropertyName].AsArray() |> Seq.map _.AsValue().GetValue<string>()
            let includeDirs = jo[IncludeDirectoriesPropertyName].AsArray() |> Seq.map _.AsValue().GetValue<string>()
            let extra = jo |> Seq.filter isExtraProperty |> propertiesToJsonObject
            ParseGeneratedToModelResult.Success 
                {   FStarExe = fstarExe; 
                    Options = options |> Array.ofSeq; 
                    IncludeDirectories = includeDirs |> Array.ofSeq; 
                    Extra = Some extra }
        else
            ParseGeneratedToModelResult.NotGeneratedFStarConfigJson jo

    let parseGeneratedTextToModel (jsonText: string) : ParseGeneratedToModelResult =
        match parseToJsonObject jsonText with
        | Pr.NotJsonObject jn -> ParseGeneratedToModelResult.NotJsonObject jn
        | Pr.JsonException je -> ParseGeneratedToModelResult.JsonException je
        | Pr.Success jo -> parseGeneratedJsonObjectToModel jo
    
    let toJsonObject (model: FStarConfigJsonModel) : JsonObject =
        let toKv k v  = KeyValuePair<string, JsonNode>(k, v)
        seq { 
            yield toKv FStarExePropertyName (model.FStarExe |> JsonValue.op_Implicit)
            yield toKv OptionsPropertyName (model.Options |> Array.map JsonValue.op_Implicit |> fun items -> JsonArray items)
            yield toKv IncludeDirectoriesPropertyName (model.IncludeDirectories |> Array.map JsonValue.op_Implicit |> fun items -> JsonArray items )
            yield! model.Extra |> Option.map (fun v -> v :> KeyValuePair<string, JsonNode> seq) |> Option.defaultValue Seq.empty
        } |> propertiesToJsonObject
    
    let tryGetComment (model: FStarConfigJsonModel) : string option =
        match model.Extra with
        | None -> None
        | Some jo ->
            let mutable jsonNodeRef = Unchecked.defaultof<JsonNode>
            if jo.TryGetPropertyValue(CommentPropertyName, &jsonNodeRef) then
                jsonNodeRef.AsValue().GetValue<string>() |> Some
            else
                None
    
    let withComment (model: FStarConfigJsonModel) (comment: string) : FStarConfigJsonModel =
        let ext = Option.defaultValue (JsonObject jsonNodeOptions) model.Extra

        let newExt = 
            let clonedExt = ext.DeepClone().AsObject()
            if clonedExt.ContainsKey CommentPropertyName then
                clonedExt[CommentPropertyName] <- JsonValue.op_Implicit comment
            else
                clonedExt.Add(CommentPropertyName, JsonValue.op_Implicit comment)
            clonedExt
            
        {   FStarExe = model.FStarExe; 
            Options = model.Options; 
            IncludeDirectories = model.IncludeDirectories; 
            Extra = Some newExt }

        



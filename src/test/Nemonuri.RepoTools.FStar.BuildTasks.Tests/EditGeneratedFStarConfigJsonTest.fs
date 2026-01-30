// Reference: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/enumerations
#nowarn "104"

namespace Nemonuri.RepoTools.FStar.BuildTasks.Tests

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;
open System.IO;

module rec EditGeneratedFStarConfigJsonTestTheory =

    open TestTheory
    open PathTheory

    module J = FStarConfigJsonTheory
    module Jm = FStarConfigJsonModelTheory

    let fixtureRootPath = 
        temporaryDirectoryRootPath
        |/> nameof EditGeneratedFStarConfigJsonTestTheory
        |/> nameof EditGeneratedFStarConfigJsonTestTheory.Fixture

    type Label =
    | SdkG = 0
    | Empty = 1

    let labels = seq {
        Label.SdkG
        Label.Empty
    }

    let labelToFStarExe (label: Label) =
        match label with
        | Label.SdkG -> GenerateFStarConfigJsonTestTheory.getCanonFStarMockExe.Force()
        | Label.Empty -> ""

    let labelToModel (label: Label) =
        Jm.create (labelToFStarExe label) [||] [||] None
        |> Jm.withFormattedComment J.DefaultGeneratorName
    
    let labelToPrefix (label: Label) =
        match label with
        | Label.SdkG -> "sdk.g"
        | Label.Empty -> ""
    
    let labelToFStarConfigJsonPath (label: Label) = 
        FStarConfigJsonTheory.getFullPath fixtureRootPath (labelToPrefix label)


    type Fixture() =

        let generatedFStarConfigJsonPathTable =
            labels
            |> Seq.map (fun l -> l, labelToFStarConfigJsonPath l)
            |> Map.ofSeq
    
        do
            generatedFStarConfigJsonPathTable
            |> Map.toSeq
            |> Seq.iter (fun (label, path) ->
                labelToModel label |> Jm.toJsonString |> fun contents -> File.WriteAllText(path, contents)
            )

        member _.GeneratedFStarConfigJsonPathTable = generatedFStarConfigJsonPathTable
    
    [<assembly: AssemblyFixture(typeof<EditGeneratedFStarConfigJsonTestTheory.Fixture>)>]
    do()

module M = EditGeneratedFStarConfigJsonTestTheory
open TestTheory
open System.Text.Json.Nodes

type EditGeneratedFStarConfigJsonTest(fixture: M.Fixture, output: ITestOutputHelper) =

    let log fmt = logf output fmt

    member __.Test(label: M.Label, options: string array, includeDirs: string array, expectedJson: string) =
        let jsonPath = fixture.GeneratedFStarConfigJsonPathTable[label]
        log "GeneratedFStarConfigJsonPath = %s" jsonPath

        EditGeneratedFStarConfigJson(
            GeneratedFStarConfigJsonPath = jsonPath,
            Options = (options |> Array.map MSBuildTheory.toTaskItem),
            IncludeDirectories = (includeDirs |> Array.map MSBuildTheory.toTaskItem)
        ).Execute() |> Assert.True

        (JsonNode.Parse(File.ReadAllText jsonPath), JsonNode.Parse expectedJson)
        |> JsonNode.DeepEquals
        |> Assert.True



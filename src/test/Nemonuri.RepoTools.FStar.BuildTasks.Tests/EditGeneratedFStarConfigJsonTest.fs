// Reference: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/enumerations
#nowarn "104"

namespace Nemonuri.RepoTools.FStar.BuildTasks.Tests

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;
open System.IO;
open TestTheory

module rec EditGeneratedFStarConfigJsonTestTheory =

    open PathTheory

    module J = FStarConfigJsonTheory
    module Jm = FStarConfigJsonModelTheory

    exception EmptyFStarConfigJsonPathException of string

    let fixtureRootPath = 
        temporaryDirectoryRootPath
        |/> nameof EditGeneratedFStarConfigJsonTestTheory
        |/> nameof EditGeneratedFStarConfigJsonTestTheory.Fixture

    type Label =
    | SdkG = 0
    | EmptyFStarExe = 1
    | EmptyFStarConfigJsonPath = 2
    | NotExistFStarConfigJsonPath = 3

    let labels = seq {
        Label.SdkG
        Label.EmptyFStarExe
        Label.EmptyFStarConfigJsonPath
        Label.NotExistFStarConfigJsonPath
    }

    let labelToFStarExe (label: Label) =
        match label with
        | Label.SdkG | Label.NotExistFStarConfigJsonPath -> "fstar.exe"
        | Label.EmptyFStarExe | Label.EmptyFStarConfigJsonPath -> ""

    let labelToModel (label: Label) =
        Jm.create (labelToFStarExe label) [||] [||] None
        |> Jm.withFormattedComment J.DefaultGeneratorName
    
    let labelToPrefix (label: Label) =
        match label with
        | Label.SdkG -> "sdk.g"
        | Label.EmptyFStarExe -> ""
        | Label.EmptyFStarConfigJsonPath -> raise <| EmptyFStarConfigJsonPathException (nameof labelToPrefix)
        | Label.NotExistFStarConfigJsonPath -> "notExist"
    
    let labelToFStarConfigJsonPath (label: Label) = 
        match label with
        | Label.EmptyFStarConfigJsonPath -> ""
        | _ -> FStarConfigJsonTheory.getFullPath fixtureRootPath (labelToPrefix label)

    let canLabelWriteModelToDisk (label: Label) =
        match label with
        | Label.SdkG | Label.EmptyFStarExe -> true
        | Label.EmptyFStarConfigJsonPath | Label.NotExistFStarConfigJsonPath -> false


    type Fixture() =

        let generatedFStarConfigJsonPathTable =
            labels
            |> Seq.map (fun l -> l, labelToFStarConfigJsonPath l)
            |> Map.ofSeq
    
        do
            generatedFStarConfigJsonPathTable
            |> Map.toSeq
            |> Seq.filter (fun (label, _) -> canLabelWriteModelToDisk label)
            |> Seq.iter (fun (label, path) ->
                path |> Path.GetDirectoryName |> function | Null -> () | NonNull path -> Directory.CreateDirectory path |> ignore
                labelToModel label |> Jm.toJsonString |> fun contents -> File.WriteAllText(path, contents)
            )

        member _.GeneratedFStarConfigJsonPathTable = generatedFStarConfigJsonPathTable
    
    [<assembly: AssemblyFixture(typeof<EditGeneratedFStarConfigJsonTestTheory.Fixture>)>]
    do()

    let member1: TheoryData<Label, string array, string array, bool, string> = 
        TheoryData<_,_,_,_,_>(seq {
            struct (
                Label.EmptyFStarExe, [||], [||], true,
                """
{
  "fstar_exe": "",
  "options": [],
  "include_dirs": [],
  "_comment": "This file is auto generated from Nemonuri.RepoTools.FStar. Do not edit manually."
}
                """)
            struct (
                Label.SdkG, [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"|], [|"ulib"; "ulib.checked"|], true, 
                """
{
  "fstar_exe": "fstar.exe",
  "options": ["--ext", "optimize_let_vc", "--ext", "fly_deps"],
  "include_dirs": ["ulib", "ulib.checked"],
  "_comment": "This file is auto generated from Nemonuri.RepoTools.FStar. Do not edit manually."
}
                """)
            struct (
                Label.EmptyFStarConfigJsonPath, [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"|], [|"ulib"; "ulib.checked"|], false, ""
            )
            struct (
                Label.NotExistFStarConfigJsonPath, [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"|], [|"ulib"; "ulib.checked"|], false, ""
            )
        })


module M = EditGeneratedFStarConfigJsonTestTheory
open System.Text.Json.Nodes

type EditGeneratedFStarConfigJsonTest(fixture: M.Fixture, output: ITestOutputHelper) =

    let log fmt = logf output fmt

    let toTaskItem (itemSpec: string) = MockTaskItem itemSpec :> Microsoft.Build.Framework.ITaskItem

    static member Member1 = M.member1

    [<Theory>]
    [<MemberData(nameof EditGeneratedFStarConfigJsonTest.Member1)>]
    member __.Test(
        label: M.Label, 
        options: string array, 
        includeDirs: string array, 
        expectedExecuteSuccess: bool, 
        expectedJson: string
        ) =
        let jsonPath = fixture.GeneratedFStarConfigJsonPathTable[label]
        log "GeneratedFStarConfigJsonPath = %s" jsonPath

        let actualExecuteSuccess = 
            EditGeneratedFStarConfigJson(
                BuildEngine = ConsoleWriterMockBuildEngine(),
                GeneratedFStarConfigJsonPath = jsonPath,
                Options = (options |> Array.map toTaskItem),
                IncludeDirectories = (includeDirs |> Array.map toTaskItem)
            ).Execute()
        
        Assert.Equal( expectedExecuteSuccess, actualExecuteSuccess)

        if not actualExecuteSuccess then () else
        
        let actualJson = File.ReadAllText jsonPath
        log "actualJson = \n%s" actualJson
        log "expectedJson = \n%s" expectedJson

        (JsonNode.Parse actualJson, JsonNode.Parse expectedJson)
        |> JsonNode.DeepEquals
        |> Assert.True



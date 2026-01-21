module GeneratePrivateFStarConfigJsonTest

#nowarn "3261"

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;

// Reference: https://github.com/dotnet/fsharp/blob/main/tests/fsharp/tests.fs
// - F# Xunit 은 별도의 TestOutputHelper 같은 게 없나보네
let log = printfn

type Amd = System.Reflection.AssemblyMetadataAttribute

[<Literal>]
let realFStarExePath = "RealFStarExePath"

[<Literal>]
let mockFStarExePath = "MockFStarExePath"

[<RequireQualifiedAccess>]
type private _Dummy = _Dummy

let getAssemblyMetadataAttributes =
    typeof<_Dummy>.Assembly.GetCustomAttributes(typeof<Amd>, false)
    |> System.Linq.Enumerable.OfType<Amd>

let realFStarExePathOrNone =
    getAssemblyMetadataAttributes
    |> Seq.tryFind (fun amd -> amd.Key = realFStarExePath)
    |> function
        | None -> None
        | Some amd -> 
            match amd.Value with
            | StringTheory.NotNullOrWhiteSpace s -> Some s
            | _ -> None

let tryGetMockFStarExePath (starting: string) =
    getAssemblyMetadataAttributes
    |> Seq.filter (fun amd -> amd.Key = mockFStarExePath)
    |> Seq.tryFind (fun amd ->
        match amd.Value with
        | StringTheory.NotNullOrWhiteSpace v -> 
            v.Trim() 
            |> System.IO.Path.GetFileName
            |> fun v -> v <> null && v.StartsWith starting
        | _ -> false
    )
    |> Option.map (fun amd -> MSBuildIntrinsicFunctions.NormalizePath amd.Value )


[<Fact>]
let TestRealFStarExePathIfSome() =
    match realFStarExePathOrNone with
    | None -> log "%s is None. Skip this test." (nameof realFStarExePathOrNone)
    | Some realPath -> 
        GeneratePrivateFStarConfigJson(
            FStarExe = realPath,
            OutDirectory = System.IO.Path.Combine [|System.AppContext.BaseDirectory; "out-dir"|],
            BuildEngine = ConsoleWriterMockBuildEngine()
        ).Execute()
        |> Assert.True


let TestMockFStarExePath_Member : TheoryData<string, bool> =
    TheoryData<_,_>(seq {        
        struct ("directory.fstar", false)
        struct ("not-exist.fstar", false)
        struct ("txt.fstar", false)
        struct ("return1.fstar", false)
        struct ("return0.fstar", false)
        struct ("stderr-version", false)
        struct ("stdout1-version", true)
        struct ("stdout0-version", true) // ← Canon
        struct ("stdin", false)
        struct ("stdin_readline", false)
        struct ("sleep1s-stdout0-version.fstar", true)
        struct ("sleep10s-stdout0-version.fstar", false)
    })

[<Theory>]
[<MemberData(nameof(TestMockFStarExePath_Member))>]
let TestMockFStarExePath (starting: string) (expected: bool) =
    match tryGetMockFStarExePath starting with
    | None -> failwith $"Cannot find mock F*. Starting = {starting}"
    | Some mockPath ->
        GeneratePrivateFStarConfigJson(
            FStarExe = mockPath,
            OutDirectory = System.IO.Path.Combine [|System.AppContext.BaseDirectory; "out-dir"|],
            BuildEngine = ConsoleWriterMockBuildEngine()
        ).Execute()
        |> fun actual -> Assert.Equal(expected, actual)

let getCanonFStarMockExe = 
    let starting = "stdout0-version"
    lazy (    
        match tryGetMockFStarExePath starting with
        | None -> failwith $"Cannot find mock F*. Starting = {starting}"
        | Some mockPath -> mockPath
    )


let AtExpectedFilePath_FStarConfigJsonShouldBeExist =
    ()

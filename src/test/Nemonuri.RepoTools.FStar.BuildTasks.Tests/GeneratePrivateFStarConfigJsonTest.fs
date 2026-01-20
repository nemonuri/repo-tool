module GeneratePrivateFStarConfigJsonTest

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks

// Reference: https://github.com/dotnet/fsharp/blob/main/tests/fsharp/tests.fs
// - F# Xunit 은 별도의 TestOutputHelper 같은 게 없나보네
let log = printfn

type Amd = System.Reflection.AssemblyMetadataAttribute

[<Literal>]
let realFStarExePath = "RealFStarExePath"

[<RequireQualifiedAccess>]
type private _Dummy = _Dummy

let realFStarExePathOrNone =
    typeof<_Dummy>.Assembly.GetCustomAttributes(typeof<Amd>, false)
    |> Seq.tryFind (fun md -> 
        match md with
        | :? Amd as amd -> amd.Key = realFStarExePath
        | _ -> false
    )
    |> function
        | None -> None
        | Some v -> 
            let amd = v :?> Amd
            match amd.Value with
            | StringTheory.NotNullOrWhiteSpace s -> Some s
            | _ -> None

[<Fact>]
let TestRealFStarExePathIfNotNone() =
    match realFStarExePathOrNone with
    | None -> log "%s is None. Skip this test." (nameof realFStarExePathOrNone)
    | Some realPath -> 
        GeneratePrivateFStarConfigJson(
            FStarExe = realPath,
            OutDirectory = System.IO.Path.Combine [|System.AppContext.BaseDirectory; "out-dir"|]
        ).Execute()
        |> Assert.True

    

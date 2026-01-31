namespace Nemonuri.RepoTools.FStar.BuildTasks.Tests

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;

module TestTheory =

    type Amd = System.Reflection.AssemblyMetadataAttribute

    [<RequireQualifiedAccess>]
    type private _Dummy = _Dummy

    let log (testOutputHelper: ITestOutputHelper) message = testOutputHelper.WriteLine message

    let logf testOutputHelper (format: Printf.StringFormat<_,_>) =
        Printf.ksprintf (log testOutputHelper) format

    let getAssemblyMetadataAttributes =
        typeof<_Dummy>.Assembly.GetCustomAttributes(typeof<Amd>, false)
        |> System.Linq.Enumerable.OfType<Amd>

    [<Literal>]
    let RealFStarExePath = "RealFStarExePath"

    [<Literal>]
    let MockFStarExePath = "MockFStarExePath"

    let realFStarExePathOrNone =
        getAssemblyMetadataAttributes
        |> Seq.tryPick (fun amd -> 
            if amd.Key <> RealFStarExePath then None else
            match amd.Value with
            | StringTheory.NotNullOrWhiteSpace s -> Some s
            | _ -> None
        )

    let tryGetMockFStarExePath (starting: string) =
        getAssemblyMetadataAttributes
        |> Seq.tryPick (fun amd ->
            if amd.Key <> MockFStarExePath then None else
            match amd.Value with
            | StringTheory.NotNullOrWhiteSpace v -> 
                v.Trim() 
                |> System.IO.Path.GetFileName
                |> function
                    | Null -> None
                    | NonNull fn -> 
                        if fn.StartsWith starting then Some v else None
            | _ -> None
        )
        |> Option.map (fun amd -> MSBuildIntrinsicFunctions.NormalizePath amd)

    let toTaskItem (itemSpec: string) = MockTaskItem itemSpec :> Microsoft.Build.Framework.ITaskItem

    let testStartTime = System.DateTime.Now

    open PathTheory

    let temporaryDirectoryRootPath = 
        System.AppContext.BaseDirectory
        |/> "temp"
        |/> System.DateTime.Now.ToString "yyyyMMdd_HHmmss_ffff"
        
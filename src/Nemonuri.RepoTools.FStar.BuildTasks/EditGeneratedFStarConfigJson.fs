namespace Nemonuri.RepoTools.FStar.BuildTasks

open Microsoft.Build.Framework
open System.IO
open FStarConfigJsonModelTheory
type Pr = FStarConfigJsonModelTheory.ParseGeneratedToModelResult

type public EditGeneratedFStarConfigJson() =
    inherit Microsoft.Build.Utilities.Task()

    [<Required>]
    member val GeneratedFStarConfigJsonPath: string = "" with get,set

    override __.Execute(): bool =
        try
            

            true
        with e ->
            __.Log.LogErrorFromException e
            __.Log.LogError "Internal stack trace = "
            __.Log.LogError e.StackTrace
            false

namespace Nemonuri.RepoTools.FStar.BuildTasks
open Microsoft.Build.Framework

type public GeneratePrivateFStarConfigJson() as this =
    inherit Microsoft.Build.Utilities.Task()

    [<Required>]
    member val FStarExe: string = "" with get,set

    member val Options: ITaskItem[] = [||] with get,set

    member val IncludeDirs: ITaskItem[] = [||] with get,set

    [<Required>]
    member val OutDirectoryPath: string = "" with get,set

    member val Prefix: string = "" with get,set

    override _.Execute(): bool =
        try
            true
        with e ->
            this.Log.LogErrorFromException(e)
            false


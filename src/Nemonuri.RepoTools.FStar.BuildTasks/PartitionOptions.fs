namespace Nemonuri.RepoTools.FStar.BuildTasks

open Microsoft.Build.Framework
module Fo = FStarOptionTheory
module Mb = MSBuildTheory

type PartitionOptions() =
    inherit Microsoft.Build.Utilities.Task()

    member val SourceOptions: ITaskItem[] = [||] with get,set

    [<Output>]
    member val ResultOptions: ITaskItem[] = [||] with get,set

    [<Output>]
    member val IncludeDirs: ITaskItem[] = [||] with get,set

    override __.Execute(): bool =
        try
            let sourceOptions = __.SourceOptions |> Array.map Mb.getItemSpec

            match 
                sourceOptions |> Fo.partitionIncludeOptions
            with
                | Fo.Success(includes, others) -> 
                    __.IncludeDirs <- includes |> List.map Mb.toTaskItem |> Array.ofList
                    __.ResultOptions <- others |> List.map Mb.toTaskItem |> Array.ofList
                    true
                | _ ->
                    Mb.logErrorAndFalse __ $"Invalid {Fo.IncludeOption}" (nameof __.SourceOptions) (Printf.sprintf "%A" sourceOptions)
        with e ->
            Mb.logExceptionAndFalse __ e
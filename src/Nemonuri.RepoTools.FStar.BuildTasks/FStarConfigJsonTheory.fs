namespace Nemonuri.RepoTools.FStar.BuildTasks

open System.IO

module FStarConfigJsonTheory =

    [<Literal>]
    let Suffix = ".fst.config.json"

    [<Literal>]
    let DefaultGeneratorName = "Nemonuri.RepoTools.FStar"

    let getFullPath directoryPath prefix = Path.Combine(directoryPath, prefix + Suffix)
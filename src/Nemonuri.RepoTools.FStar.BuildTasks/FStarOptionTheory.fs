namespace Nemonuri.RepoTools.FStar.BuildTasks

module FStarOptionTheory =

    let [<Literal>] IncludeOption = "--include"

    type private PartitionIncludeState = { 
        Includes: string list
        Others: string list
        ExpectingDir: bool
    }

    type PartitionIncludeResult =
    | Success of includes: string list * others: string list
    | IncompleteIncludes of includes: string list * others: string list

    let partitionIncludeOptions (options: string seq) : PartitionIncludeResult =
        let initialState = { Includes = []; Others = []; ExpectingDir = false }

        let state =
            (initialState, options)
            ||> Seq.fold (fun (state: PartitionIncludeState) (option: string) -> 
                if state.ExpectingDir then
                    { state with Includes = state.Includes @ [option]; ExpectingDir = false }
                else
                    if option = IncludeOption then
                        { state with ExpectingDir = true }
                    else
                        { state with Others = state.Others @ [option] } )

        if state.ExpectingDir then
            IncompleteIncludes(state.Includes, state.Others)
        else
            Success(state.Includes, state.Others)

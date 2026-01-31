namespace Nemonuri.RepoTools.FStar.BuildTasks.Tests

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;
open TestTheory

module PartitionOptionsTestTheory =

    let member1 : TheoryData<string array, bool, string array, string array> =
        TheoryData<_,_,_,_>(seq {
            struct ( [|"--include"; "ulib"|], true, [||], [|"ulib"|] )
            struct ( [|"--include"; "ulib"; "--lax";"--include"; "ulib.checked"|], true, [|"--lax"|], [|"ulib"; "ulib.checked"|] )
            struct ( 
                [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"; "--include"; "ulib"; "--include"; "ulib.checked"|], true, 
                [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"|], [|"ulib"; "ulib.checked"|] )
            struct ( [|"--ext"; "optimize_let_vc"; "--ext"; "fly_deps"; "--include"; "ulib"; "--include"|], false, [||], [||] )
        })

module M = PartitionOptionsTestTheory

type PartitionOptionsTest(output: ITestOutputHelper) =

    let log fmt = logf output fmt

    static member val Member1 = M.member1

    [<Theory>]
    [<MemberData(nameof PartitionOptionsTest.Member1)>]
    member __.Test 
        (sourceOptions: string array) 
        (expectedSuccess: bool)
        (expectedOthers: string array) 
        (expectedIncludes: string array) =
        let buildTask =
            PartitionOptions(
                BuildEngine = ConsoleWriterMockBuildEngine(),
                SourceOptions = (sourceOptions |> Array.map toTaskItem)
            )
        let actualSuccess = buildTask.Execute()
        Assert.Equal(expectedSuccess, actualSuccess)

        if not actualSuccess then () else

        Assert.Equal<string>(expectedOthers, buildTask.ResultOptions |> Array.map _.ItemSpec)
        Assert.Equal<string>(expectedIncludes, buildTask.ResultIncludeDirectories |> Array.map _.ItemSpec)

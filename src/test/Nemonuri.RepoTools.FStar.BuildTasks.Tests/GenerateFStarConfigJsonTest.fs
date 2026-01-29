namespace Nemonuri.RepoTools.FStar.BuildTasks.Tests

open Xunit
open Nemonuri.RepoTools.FStar.BuildTasks
open Nemonuri.RepoTools.TestRuntime;
open TestTheory

module GenerateFStarConfigJsonTestTheory =

    let members1 : TheoryData<string, bool> =
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

    let getCanonFStarMockExe = 
        let starting = "stdout0-version"
        lazy (    
            match tryGetMockFStarExePath starting with
            | None -> failwith $"Cannot find mock F*. Starting = {starting}"
            | Some mockPath -> mockPath
        )

    let members2 : TheoryData<string> =
        TheoryData<_>( seq {
            FStarConfigJsonTheory.GeneratorDefaultPrefix
            ""
            "Hello"
        })
    
    

module M = GenerateFStarConfigJsonTestTheory
open PathTheory

type GenerateFStarConfigJsonTest(output: ITestOutputHelper) =

    let log fmt = logf output fmt

    let tempPath = temporaryDirectoryRootPath </> nameof GenerateFStarConfigJsonTest

    [<Fact>]
    member __.TestRealFStarExePathIfSome() =
        match realFStarExePathOrNone with
        | None -> log "%s is None. Skip this test." (nameof realFStarExePathOrNone)
        | Some realPath -> 
            GenerateFStarConfigJson(
                FStarExe = realPath,
                OutDirectory = (tempPath </> nameof __.TestRealFStarExePathIfSome),
                BuildEngine = ConsoleWriterMockBuildEngine()
            ).Execute()
            |> Assert.True


    static member val Members1 = M.members1

    [<Theory>]
    [<MemberData(nameof(GenerateFStarConfigJsonTest.Members1))>]
    member __.TestMockFStarExePath (starting: string) (expected: bool) =
        match tryGetMockFStarExePath starting with
        | None -> failwith $"Cannot find mock F*. Starting = {starting}"
        | Some mockPath ->
            GenerateFStarConfigJson(
                FStarExe = mockPath,
                OutDirectory = (tempPath </> nameof __.TestMockFStarExePath),
                BuildEngine = ConsoleWriterMockBuildEngine()
            ).Execute()
            |> fun actual -> Assert.Equal(expected, actual)

    static member val Members2 = M.members2

    [<Theory>]
    [<MemberData(nameof(GenerateFStarConfigJsonTest.Members2))>]
    member __.GeneratedFilePath_FileShouldBeExistAndValid (prefix: string) =
        let outDirectory = tempPath </> nameof __.GeneratedFilePath_FileShouldBeExistAndValid
        let generatedFilePathBox = MockTaskItem ""
        GenerateFStarConfigJson(
            FStarExe = M.getCanonFStarMockExe.Force(),
            OutDirectory = outDirectory,
            BuildEngine = ConsoleWriterMockBuildEngine(),
            Prefix = prefix,
            GeneratedFilePath = generatedFilePathBox
        ).Execute()
        |> Assert.True
        Assert.True (FStarConfigJsonTheory.isMaybeGeneratedFile generatedFilePathBox.ItemSpec)


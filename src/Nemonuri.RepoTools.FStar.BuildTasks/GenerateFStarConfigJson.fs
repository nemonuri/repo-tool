namespace Nemonuri.RepoTools.FStar.BuildTasks

open Microsoft.Build.Framework
open System.IO
open System.Text.RegularExpressions
open FStarConfigJsonTheory

type private F = System.IO.File
type private D = System.IO.Directory
type private Ir = ProcessTheory.InvokeResult
module M = MSBuildTheory

type public GenerateFStarConfigJson() =
    inherit Microsoft.Build.Utilities.Task()

    let [<Literal>] timeOut = 3000

    [<Required>]
    member val FStarExe: string = "" with get,set

    member val Options: ITaskItem[] = [||] with get,set

    member val IncludeDirs: ITaskItem[] = [||] with get,set

    [<Required>]
    member val OutDirectory: string = "" with get,set

    member val Prefix: string = GeneratorDefaultPrefix with get,set

    member val SkipVersionCommand: bool = false with get,set

    member val GeneratorName: string = DefaultGeneratorName with get,set

    [<Output>]
    member val GeneratedFilePath: ITaskItem | null = null with get,set

    override __.Execute(): bool =
        let logError a1 a2 a3 = M.logErrorAndFalse __ a1 a2 a3
        let fe = __.FStarExe
        let fee = nameof __.FStarExe
        let od = __.OutDirectory
        let ode = nameof __.OutDirectory

        let areAllPathCharsValid path pathExpr =
            if PathTheory.isPathContainsInvalidPathChars path then 
                logError "Invalid path" pathExpr path
            else
                true

        let checkPath path pathExpr =
            if System.String.IsNullOrWhiteSpace path then
                logError "Empty path" pathExpr path
            elif
                areAllPathCharsValid path pathExpr |> not then false
            else
                true

        try
            if areAllPathCharsValid __.Prefix (nameof __.Prefix) |> not then false else

            let checkFStarExe =
                if checkPath fee fe |> not then false
                elif F.Exists fe |> not then 
                    match D.Exists fe with
                    | true -> logError "Not file, but directory" fee fe 
                    | false -> logError "File is not exist" fee fe 
                else

                if __.SkipVersionCommand then true else

                __.Log.LogMessage("Invoke version command. Command = {0} {1}", fe, "--version")
                match ProcessTheory.invokeVersionCommand fe timeOut with
                | Ir.TimeOut -> logError $"Timeout {timeOut}" fee fe
                | Ir.StdErr msg -> logError "Failed" "Message" msg
                | Ir.Exception e -> logError "Exception" "Message" e
                | Ir.StdOut msg -> 
                    __.Log.LogMessage msg
                    Regex.Split(msg, """\s+""") 
                    |> Array.tryFind (fun s -> s.Equals("F*", System.StringComparison.OrdinalIgnoreCase))
                    |> Option.isSome
            
            if checkFStarExe = false then false else

            let checkOutDirectory =
                if checkPath od ode |> not then None else
                
                let odInfo =
                    if D.Exists od |> not then 
                        __.Log.LogMessage("New directory created. {0} = {1}", ode, od)
                        D.CreateDirectory od
                    else 
                        DirectoryInfo od
                Some odInfo
            
            if Option.isNone checkOutDirectory then false else

            let odInfo = checkOutDirectory.Value

            let ``, ``= ", "
            let getItemSpec (ti: ITaskItem) = $"\"{ti.ItemSpec}\""


            let fcjContent = 
                { FStarExe = fe;
                    Options = __.Options |> Seq.map getItemSpec |> Array.ofSeq;
                    IncludeDirectories = __.IncludeDirs |> Seq.map getItemSpec |> Array.ofSeq;
                    Extra = None }
                |> FStarConfigJsonModelTheory.withFormattedComment __.GeneratorName
                |> FStarConfigJsonModelTheory.toJsonString
            let fcjFilePath = FStarConfigJsonTheory.getFullPath odInfo.FullName __.Prefix
            F.WriteAllText (fcjFilePath, fcjContent)

            __.Log.LogMessage("File written. Path = {0}", fcjFilePath)

            match __.GeneratedFilePath with
            | Null _ -> __.GeneratedFilePath <- Microsoft.Build.Utilities.TaskItem fcjFilePath
            | NonNull v -> v.ItemSpec <- fcjFilePath

            true
            
        with e ->
            M.logExceptionAndFalse __ e


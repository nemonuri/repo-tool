namespace Nemonuri.RepoTools.FStar.BuildTasks

open Microsoft.Build.Framework
open System.IO
open System.Text
open System.Text.RegularExpressions

type private F = System.IO.File
type private D = System.IO.Directory
type private Ir = ProcessTheory.InvokeResult

module FStarConfigJson =

    [<Literal>]
    let suffix = ".fst.config.json"

    [<Literal>]
    let defaultGeneratorName = "Nemonuri.RepoTools.FStar"

    let getFullPath directoryPath prefix = Path.Combine(directoryPath, prefix + suffix)

type public GeneratePrivateFStarConfigJson() =
    inherit Microsoft.Build.Utilities.Task()

    [<Required>]
    member val FStarExe: string = "" with get,set

    member val Options: ITaskItem[] = [||] with get,set

    member val IncludeDirs: ITaskItem[] = [||] with get,set

    [<Required>]
    member val OutDirectory: string = "" with get,set

    member val Prefix: string = "" with get,set

    member val SkipVersionCommand: bool = false with get,set

    member val GeneratorName: string = "" with get,set

    override __.Execute(): bool =
        let logError message valueExpr value = __.Log.LogError("{0}. {1} = {2}", message, valueExpr, value); false
        let fe = __.FStarExe
        let fee = nameof __.FStarExe
        let timeOut = 5000
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
            else
            if areAllPathCharsValid path pathExpr |> not then false
            else
            true

        try
            if areAllPathCharsValid __.Prefix (nameof __.Prefix) |> not then false
            else

            let checkFStarExe =
                if checkPath fee fe |> not then false
                else

                if F.Exists fe |> not then 
                    logError "File is not exist" fee fe
                else

                // if F.GetAttributes fe |> FileTheory.isMaybeExecutable |> not then
                //    __.Log.LogMessage("{0}", F.GetAttributes fe)
                //    logError "File is not executable" fee fe
                // else

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
                if checkPath od ode |> not then None
                else
                let odInfo =
                    if D.Exists od |> not then 
                        __.Log.LogMessage("New directory created. {0} = {1}", ode, od)
                        D.CreateDirectory od
                    else DirectoryInfo od
                Some odInfo
            
            if Option.isNone checkOutDirectory then false else

            let odInfo = checkOutDirectory.Value

            // build json string
            let sb = StringBuilder()
            let append (text : string) = sb.AppendLine text |> ignore
            let ``, ``= ", "
            let getItemSpec (ti: ITaskItem) = ti.ItemSpec
            let generatorName =
                match __.GeneratorName with
                | StringTheory.NotNullOrWhiteSpace v -> v
                | _ -> FStarConfigJson.defaultGeneratorName

            append "{"
            append $$"""  "_comment": "This file is auto generated from {{generatorName}}. Do not edit manually.", """
            append $$"""  "options": [{{__.Options |> Array.map getItemSpec |> String.concat ``, ``}}], """
            append $$"""  "include_dirs": [{{__.IncludeDirs |> Array.map getItemSpec |> String.concat ``, ``}}] """
            append "}"

            let fcjContent = sb.ToString()
            let fcjFilePath = FStarConfigJson.getFullPath odInfo.FullName __.Prefix
            F.WriteAllText (fcjFilePath, fcjContent)

            __.Log.LogMessage("File written. Path = {0}", fcjFilePath)

            true
            
        with e ->
            __.Log.LogErrorFromException(e)
            false


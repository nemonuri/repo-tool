namespace Nemonuri.RepoTools.FStar.BuildTasks

open System.IO
open Microsoft.FSharp.Collections

module PathTheory =

    let private invalidPathCharsCache = Path.GetInvalidPathChars()

    let isPathContainsInvalidPathChars path =
        let isInvalidChar c = Array.contains c invalidPathCharsCache
        String.exists isInvalidChar path


module FileTheory =

    let isNoneOrNormal (fileAttr: FileAttributes) =
        match fileAttr with
        | FileAttributes.Normal -> true
        | _ -> fileAttr = LanguagePrimitives.EnumOfValue 0

module ProcessTheory =

    open System.Diagnostics

    type InvokeResult =
    | StdOut of message: string
    | StdErr of message: string
    | Exception of exn
    | TimeOut

    [<Literal>]
    let versionCommandArg = "--version"

    let invokeVersionCommand path (millisecondsTimeout : int) : InvokeResult =
        let startInfo = ProcessStartInfo(
            FileName = path,
            Arguments = versionCommandArg,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        )
        let proc = new Process(StartInfo = startInfo)
        let mutable errorMessage = ""
        proc.ErrorDataReceived.Add(fun evArg -> errorMessage <- evArg.Data) 
        proc.Start() |> ignore
        proc.BeginErrorReadLine()
        try
            try
                Async.RunSynchronously(proc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask, millisecondsTimeout) 
                    |> fun s -> 
                        match System.String.IsNullOrWhiteSpace s with
                        | false -> StdOut s
                        | true -> StdErr errorMessage
            with e ->
                match e with
                | :? System.TimeoutException -> TimeOut
                | _ -> Exception e
        finally
            proc.Kill()

module StringTheory =

    [<CompiledNameAttribute("TryGetNotNullOrWhiteSpace")>]
    let (|NotNullOrWhiteSpace|_|) (value: string | null) : string option =
        if System.String.IsNullOrWhiteSpace value then None else Some value
    

namespace Nemonuri.RepoTools.FStar.BuildTasks

open System.IO
open Microsoft.FSharp.Collections

module PathTheory =

    let private invalidPathCharsCache = Path.GetInvalidPathChars()

    let isPathContainsInvalidPathChars path =
        let isInvalidChar c = Array.contains c invalidPathCharsCache
        String.exists isInvalidChar path

    let (</>) a b = Path.Combine [|a;b|]

    let (|/>) a b = a </> b


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
            RedirectStandardError = true,
            RedirectStandardInput = true
        )
        let proc = new Process(StartInfo = startInfo)
        let mutable errorMessage = ""
        proc.ErrorDataReceived.Add(fun evArg -> errorMessage <- errorMessage + evArg.Data) 
        let mutable stdoutMessage = ""
        proc.OutputDataReceived.Add(fun evArg -> stdoutMessage <- stdoutMessage + evArg.Data)
        proc.Start() |> ignore
        proc.BeginErrorReadLine()
        proc.BeginOutputReadLine()
        try
            try
                match proc.WaitForExit millisecondsTimeout with
                | false -> TimeOut
                | true -> 
                    if System.String.IsNullOrWhiteSpace stdoutMessage then
                        StdErr errorMessage
                    else
                        StdOut stdoutMessage
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


namespace Nemonuri.OCamlDotNet.Forwarded

open System
open Nemonuri.OCamlDotNet.Primitives
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Obsu = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources.Unsafe

/// reference: https://ocaml.org/manual/5.4/api/Sys.html
module Sys =

    let argv: array<OCamlString> = Environment.GetCommandLineArgs() |> Array.map Obs.stringOfDotNetString
    
    type signal = int 

    type signal_behavior = 
    | Signal_default
    | Signal_ignore
    | Signal_handle of (signal -> unit)

    // let signal (s: signal)

    let os_type : OCamlString = 
        match Environment.OSVersion.Platform with
        | PlatformID.Win32NT -> "Win32"B
        | _ -> "Unix"B
        |> Obsu.stringOfArray
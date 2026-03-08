namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.OCamlDotNet.Primitives

/// reference: https://ocaml.org/manual/5.4/api/Sys.html
module Sys =

    let argv: array<OCamlString> = System.Environment.GetCommandLineArgs() |> Collections.Array.map OCamlByteSpanSources.stringOfDotNetString
    
    type signal = int 

    type signal_behavior = 
    | Signal_default
    | Signal_ignore
    | Signal_handle of (signal -> unit)

    // let signal (s: signal)
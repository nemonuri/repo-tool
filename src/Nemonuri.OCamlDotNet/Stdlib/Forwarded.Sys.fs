namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.Operations

/// reference: https://ocaml.org/manual/5.4/api/Sys.html
module Sys =

    let argv: array<OCamlString> = System.Environment.GetCommandLineArgs() |> Collections.Array.map OCamlByteSpanSources.stringOfDotNetString
    
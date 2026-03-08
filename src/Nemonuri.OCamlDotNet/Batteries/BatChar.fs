namespace Nemonuri.OCamlDotNet.Batteries

open System
open type System.MemoryExtensions
open Nemonuri.OCamlDotNet.Primitives

/// reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatChar.html
module BatChar =

    let private symbols = [|'!'B; '%'B; '&'B; '$'B; '#'B; '+'B; '-'B; '/'B; ':'B; '<'B; '='B; '>'B; '?'B; '@'B; '\\'B; '~'B; '^'B; '|'B; '*'B|]

    let is_symbol (c: OCamlChar) = symbols.AsSpan().IndexOf(c) >= 0
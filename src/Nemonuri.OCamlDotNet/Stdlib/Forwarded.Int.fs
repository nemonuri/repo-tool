namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.ByteChars.Numerics
open Nemonuri.OCamlDotNet.Primitives
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

/// reference: https://ocaml.org/manual/5.4/api/Int.html
module Int =

    type t = OCamlInt

    let to_string (x: OCamlInt) = Int32Theory.ToMutableByteString(x) |> Obs.Unsafe.stringOfArraySegment

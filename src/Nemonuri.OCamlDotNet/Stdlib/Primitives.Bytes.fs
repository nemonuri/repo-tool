namespace Nemonuri.OCamlDotNet.Primitives

open Nemonuri.OCamlDotNet.Primitives.Operations
module Obs = OCamlByteSequenceSources
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/Bytes.html
module Bytes =

    module Unsafe =

        let ofSource (source: OCamlByteSequenceSource) = Unsafe.sourceToBytes source

        let toSource (s: OCamlBytes) = Unsafe.sourceOfBytes s

        let ofArray (source: OCamlChar array) =
            source
            |> Obs.ofArray
            |> ofSource

    module U = Unsafe

    let toSpan (s: OCamlBytes) = Obs.unsafeToSpan (U.toSource s) 


    let length (s: OCamlBytes) = Bs.length (toSpan s)

    let get (s: OCamlBytes) (n: OCamlInt) : OCamlChar = Bs.get (toSpan s) n

    let set (s: OCamlBytes) n c = Bs.set (toSpan s) n c

    let create (n: OCamlInt) : OCamlBytes = 
#if NET8_0_OR_GREATER
        System.GC.AllocateUninitializedArray(length = n, pinned = false)
#else
        Array.create n 0uy 
#endif
        |> U.ofArray
    
    let make (n: OCamlInt) (c: OCamlChar) = Array.create n c |> U.ofArray

    let init (n: OCamlInt) f = Array.init n f |> U.ofArray

    let empty: OCamlBytes = Obs.empty |> U.ofSource

    let copy (s: OCamlBytes) = (toSpan s).ToArray() |> U.ofArray

    let of_string (s: OCamlString) : OCamlBytes = Unsafe.sourceOfString s |> Obs.clone |> U.ofSource

    let to_string (s: OCamlBytes) : OCamlString = U.toSource s |> Obs.clone |> Unsafe.sourceToString


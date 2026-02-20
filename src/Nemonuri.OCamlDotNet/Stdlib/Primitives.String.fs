namespace Nemonuri.OCamlDotNet.Primitives

open Nemonuri.OCamlDotNet.Primitives.Operations
module Obs = OCamlByteSequenceSources
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/String.html
module String =

    module Unsafe =

        let internal ofSource (source: OCamlByteSequenceSource) = Unsafe.sourceToString source

        let internal toSource (s: OCamlString) = Unsafe.sourceOfString s

        let ofArray (source: OCamlChar array) =
            source
            |> Obs.ofArray
            |> ofSource

    module U = Unsafe
    
    let toSpan (s: OCamlString) = Obs.toSpan (U.toSource s)


    let make (n: OCamlInt) (c: OCamlChar) : OCamlString = Array.create n c |> U.ofArray
    
    let init (n: OCamlInt) (f: OCamlInt -> OCamlChar) : OCamlString = Array.init n f |> U.ofArray
        
    
    let empty: OCamlString = Obs.empty |> U.ofSource

    let length (s: OCamlString) = Bs.length (toSpan s)

    let get (s: OCamlString) (i: OCamlInt) : OCamlChar = Bs.get (toSpan s) i

    //let ofSource (source: OCamlByteSequenceSource) = Obs.clone source |> U.ofSource

    //let toSource (s: OCamlString) = s |> U.toSource |> Obs.clone






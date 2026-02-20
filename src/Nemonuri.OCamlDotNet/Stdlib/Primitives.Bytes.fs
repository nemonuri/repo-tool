namespace Nemonuri.OCamlDotNet.Primitives

open System;
open Nemonuri.OCamlDotNet.Primitives.Operations
open type System.MemoryExtensions
module Obs = OCamlByteSequenceSources
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/Bytes.html
module Bytes =

    module Unsafe =

        let ofSource (source: OCamlByteSequenceSource) = Unsafe.sourceToBytes source

        let toSource (s: OCamlBytes) = Unsafe.sourceOfBytes s

        let ofArraySegemnt (source: ArraySegment<OCamlChar>) =
            source
            |> Obs.ofArraySegemnt
            |> ofSource

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
        DotNetArrays.createUninitialized<OCamlChar> n
        |> U.ofArray
    
    let make (n: OCamlInt) (c: OCamlChar) = Array.create n c |> U.ofArray

    let init (n: OCamlInt) f = Array.init n f |> U.ofArray

    let empty: OCamlBytes = Obs.empty |> U.ofSource

    let copy (s: OCamlBytes) = (toSpan s).ToArray() |> U.ofArray

    let of_string (s: OCamlString) : OCamlBytes = Unsafe.sourceOfString s |> Obs.clone |> U.ofSource

    let to_string (s: OCamlBytes) : OCamlString = U.toSource s |> Obs.clone |> Unsafe.sourceToString

    let sub (s: OCamlBytes) pos len = U.toSource s |> _.Slice(pos, len) |> U.ofSource

    let sub_string (s: OCamlBytes) pos len = U.toSource s |> _.Slice(pos, len) |> U.sourceToString

    let extend (s: OCamlBytes) (left: OCamlInt) (right: OCamlInt) =
        ByteSpans.extend (toSpan s) left right
        |> U.ofArray
    
    let fill (s: OCamlBytes) pos len c = ByteSpans.fill (toSpan s) pos len c

    let blit (src: OCamlBytes) src_pos dst dst_pos len =
        ByteSpans.blit (toSpan src) src_pos dst dst_pos len

    let blit_string (src: OCamlString) src_pos dst dst_pos len =
        let rbs = Obs.toSpan (Unsafe.sourceOfString src)
        ByteSpans.blit rbs src_pos dst dst_pos len
    
    let concat (sep: OCamlBytes) (sl: OCamlBytes list) = ByteSpans.concat sep sl |> U.ofArraySegemnt

    let cat (s1: OCamlBytes) (s2: OCamlBytes) = ByteSpans.cat s1 s2 |> U.ofArraySegemnt
        
    
        

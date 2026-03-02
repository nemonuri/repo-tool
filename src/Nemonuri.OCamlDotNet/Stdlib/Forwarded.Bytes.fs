namespace Nemonuri.OCamlDotNet.Forwarded

open System;
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.Operations
open type System.MemoryExtensions
module Obs = OCamlByteSequenceSources
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/Bytes.html
module Bytes =

    module Unsafe =

        let internal ofSource (source: OCamlByteSequenceSource) = Obs.Unsafe.sourceToBytes source

        let internal toSource (s: OCamlBytes) = Obs.Unsafe.sourceOfBytes s

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

    let of_string (s: OCamlString) : OCamlBytes = Obs.Unsafe.sourceOfString s |> Obs.clone |> U.ofSource

    let to_string (s: OCamlBytes) : OCamlString = U.toSource s |> Obs.clone |> Obs.Unsafe.sourceToString

    let sub (s: OCamlBytes) pos len = U.toSource s |> _.Slice(pos, len) |> U.ofSource

    let sub_string (s: OCamlBytes) pos len = U.toSource s |> _.Slice(pos, len) |> Obs.Unsafe.sourceToString

    let extend (s: OCamlBytes) (left: OCamlInt) (right: OCamlInt) =
        Bs.extend (toSpan s) left right
        |> U.ofArray
    
    let fill (s: OCamlBytes) pos len c = Bs.fill (toSpan s) pos len c

    let blit (src: OCamlBytes) src_pos dst dst_pos len =
        Bs.blit (toSpan src) src_pos dst dst_pos len

    let blit_string (src: OCamlString) src_pos dst dst_pos len =
        let rbs = Obs.toSpan (Obs.Unsafe.sourceOfString src)
        Bs.blit rbs src_pos dst dst_pos len
    
    let concat (sep: OCamlBytes) (sl: OCamlBytes list) = Bs.concat sep sl |> U.ofArraySegemnt

    let cat (s1: OCamlBytes) (s2: OCamlBytes) = Bs.cat s1 s2 |> U.ofArraySegemnt

    let iter f (s: OCamlBytes) = Bs.iter f (toSpan s)

    let iteri f (s: OCamlBytes) = Bs.iteri f (toSpan s)

    let map f (s: OCamlBytes) = Bs.map f (toSpan s) |> U.ofArray

    let mapi f (s: OCamlBytes) = Bs.mapi f (toSpan s) |> U.ofArray
    
    let fold_left f (x: 'acc) (s: OCamlBytes) = Bs.fold_left f x (toSpan s)

    let fold_right f (s: OCamlBytes) (x: 'acc) = Bs.fold_right f (toSpan s) x

    let for_all f (s: OCamlBytes) = Bs.for_all f (toSpan s)

    let exists  p (s: OCamlBytes) = Bs.exists p (toSpan s)

    let trim (s: OCamlBytes) = (Bs.trim (toSpan s)).ToArray() |> U.ofArray

    let escaped (s: OCamlBytes) = Bs.escaped (toSpan s) |> U.ofArraySegemnt

    let index (s: OCamlBytes) c = Bs.checked_index (toSpan s) c

    let index_opt (s: OCamlBytes) c = Bs.index_opt (toSpan s) c

    let rindex (s: OCamlBytes) c = Bs.checked_rindex (toSpan s) c

    let rindex_opt (s: OCamlBytes) c = Bs.rindex_opt (toSpan s) c

    let index_from (s: OCamlBytes) i c = Bs.checked_index_from (toSpan s) i c

    let index_from_opt (s: OCamlBytes) i c = Bs.index_from_opt (toSpan s) i c

    let rindex_from (s: OCamlBytes) i c = Bs.checked_rindex_from (toSpan s) i c

    let rindex_from_opt (s: OCamlBytes) i c = Bs.rindex_from_opt (toSpan s) i c

    let contains (s: OCamlBytes) c = Bs.contains (toSpan s) c

    let contains_from (s: OCamlBytes) start c = Bs.contains_from (toSpan s) start c

    let rcontains_from (s: OCamlBytes) stop c = Bs.rcontains_from (toSpan s) stop c

    let uppercase_ascii (s: OCamlBytes) = Bs.uppercase_ascii (toSpan s) |> U.ofArraySegemnt

    let lowercase_ascii (s: OCamlBytes) = Bs.lowercase_ascii (toSpan s) |> U.ofArraySegemnt

    let capitalize_ascii (s: OCamlBytes) = Bs.capitalize_ascii (toSpan s) |> U.ofArraySegemnt

    let uncapitalize_ascii (s: OCamlBytes) = Bs.uncapitalize_ascii (toSpan s) |> U.ofArraySegemnt

    type t = OCamlBytes

    let compare (l: t) (r: t) = LanguagePrimitives.GenericComparison l r

    let equal (l: t) (r: t) = l = r

    let starts_with (prefix: OCamlBytes) (s: OCamlBytes) = Bs.starts_with (toSpan prefix) (toSpan s)

    let ends_with (suffix: OCamlBytes) (s: OCamlBytes) = Bs.ends_with (toSpan suffix) (toSpan s)

    let unsafe_to_string s = U.toSource s |> Obs.Unsafe.sourceToString

    let unsafe_of_string s = Obs.Unsafe.sourceOfString s |> U.ofSource
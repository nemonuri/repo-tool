namespace Nemonuri.OCamlDotNet.Forwarded

open System;
open Nemonuri.OCamlDotNet.Primitives
open type System.MemoryExtensions
module O = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ou = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources.Unsafe
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/Bytes.html
module Bytes =

    let private toSpan (s: OCamlBytes) = Ou.bytesToSpan s


    let length (s: OCamlBytes) = Bs.length (toSpan s)

    let get (s: OCamlBytes) (n: OCamlInt) : OCamlChar = Bs.get (toSpan s) n

    let set (s: OCamlBytes) n c = Bs.set (toSpan s) n c

    let create (n: OCamlInt) : OCamlBytes = 
        DotNetArrays.createUninitialized<OCamlChar> n
        |> Ou.bytesOfArray
    
    let make (n: OCamlInt) (c: OCamlChar) = Array.create n c |> Ou.bytesOfArray

    let init (n: OCamlInt) f = Array.init n f |> Ou.bytesOfArray

    let empty: OCamlBytes = O.emptyBytes

    let copy (s: OCamlBytes) = (toSpan s).ToArray() |> Ou.bytesOfArray

    let of_string (s: OCamlString) : OCamlBytes = s |> O.cloneString |> Ou.stringToBytes

    let to_string (s: OCamlBytes) : OCamlString = s |> O.cloneBytes |> Ou.bytesToString

    let sub (s: OCamlBytes) pos len = O.bytesSlice s pos len

    let sub_string (s: OCamlBytes) pos len = (Ou.bytesToString >> O.stringSlice) s pos len

    let extend (s: OCamlBytes) (left: OCamlInt) (right: OCamlInt) =
        Bs.extend (toSpan s) left right
        |> Ou.bytesOfArray
    
    let fill (s: OCamlBytes) pos len c = Bs.fill (toSpan s) pos len c

    let blit (src: OCamlBytes) src_pos dst dst_pos len =
        Bs.blit (toSpan src) src_pos dst dst_pos len

    let blit_string (src: OCamlString) src_pos dst dst_pos len =
        let rbs = O.stringToReadOnlySpan src
        Bs.blit rbs src_pos dst dst_pos len
    
    let concat (sep: OCamlBytes) (sl: OCamlBytes list) = Bs.concat sep sl |> Ou.bytesOfArraySegment

    let cat (s1: OCamlBytes) (s2: OCamlBytes) = Bs.cat s1 s2 |> Ou.bytesOfArraySegment

    let iter f (s: OCamlBytes) = Bs.iter f (toSpan s)

    let iteri f (s: OCamlBytes) = Bs.iteri f (toSpan s)

    let map f (s: OCamlBytes) = Bs.map f (toSpan s) |> Ou.bytesOfArray

    let mapi f (s: OCamlBytes) = Bs.mapi f (toSpan s) |> Ou.bytesOfArray
    
    let fold_left f (x: 'acc) (s: OCamlBytes) = Bs.fold_left f x (toSpan s)

    let fold_right f (s: OCamlBytes) (x: 'acc) = Bs.fold_right f (toSpan s) x

    let for_all f (s: OCamlBytes) = Bs.for_all f (toSpan s)

    let exists  p (s: OCamlBytes) = Bs.exists p (toSpan s)

    let trim (s: OCamlBytes) = (Bs.trim (toSpan s)).ToArray() |> Ou.bytesOfArray

    let escaped (s: OCamlBytes) = Bs.escaped (toSpan s) |> Ou.bytesOfArraySegment

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

    let uppercase_ascii (s: OCamlBytes) = Bs.uppercase_ascii (toSpan s) |> Ou.bytesOfArraySegment

    let lowercase_ascii (s: OCamlBytes) = Bs.lowercase_ascii (toSpan s) |> Ou.bytesOfArraySegment

    let capitalize_ascii (s: OCamlBytes) = Bs.capitalize_ascii (toSpan s) |> Ou.bytesOfArraySegment

    let uncapitalize_ascii (s: OCamlBytes) = Bs.uncapitalize_ascii (toSpan s) |> Ou.bytesOfArraySegment

    type t = OCamlBytes

    let compare (l: t) (r: t) = O.bytesCompare l r

    let equal (l: t) (r: t) = O.bytesEqual l r

    let starts_with (prefix: OCamlBytes) (s: OCamlBytes) = Bs.starts_with (toSpan prefix) (toSpan s)

    let ends_with (suffix: OCamlBytes) (s: OCamlBytes) = Bs.ends_with (toSpan suffix) (toSpan s)

    let unsafe_to_string s = Ou.bytesToString s

    let unsafe_of_string s = Ou.stringToBytes s

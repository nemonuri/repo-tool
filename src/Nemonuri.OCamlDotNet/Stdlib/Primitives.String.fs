namespace Nemonuri.OCamlDotNet.Primitives

open System;
open Nemonuri.OCamlDotNet.Primitives.Operations
open type System.MemoryExtensions
module Obs = OCamlByteSequenceSources
module B = Nemonuri.OCamlDotNet.Primitives.Bytes
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/String.html
module String =

    type internal Monad =
        struct
            member inline this.Bind(t: OCamlString, sf: OCamlBytes -> 'a) = Bytes.unsafe_of_string t |> sf
            member inline this.Return(s: OCamlBytes) = Bytes.unsafe_to_string s
            member inline this.ReturnFrom(s: 's) = s
        end

    module Unsafe =

        let internal ofSource (source: OCamlByteSequenceSource) = Unsafe.sourceToString source

        let internal toSource (s: OCamlString) = Unsafe.sourceOfString s

        let ofArraySegemnt (source: ArraySegment<OCamlChar>) =
            source
            |> Obs.ofArraySegemnt
            |> ofSource

        let ofArray (source: OCamlChar array) =
            source
            |> Obs.ofArray
            |> ofSource

    module U = Unsafe
    
    let private mnd = Monad()

    let toSpan (s: OCamlString) = Obs.toSpan (U.toSource s)

    let make (n: OCamlInt) (c: OCamlChar) : OCamlString = mnd { return B.make n c }
    
    let init (n: OCamlInt) (f: OCamlInt -> OCamlChar) : OCamlString = mnd { return B.init n f }
    
    let empty: OCamlString = mnd { return B.empty }

    let length (s: OCamlString) = mnd { let! b = s in return! B.length b }

    let get (s: OCamlString) (i: OCamlInt) : OCamlChar = mnd { let! b = s in return! B.get b i }

    let of_bytes (s: OCamlBytes) : OCamlString = Unsafe.sourceOfBytes s |> Obs.clone |> U.ofSource

    let to_bytes (s: OCamlString) : OCamlBytes = U.toSource s |> Obs.clone |> Unsafe.sourceToBytes

    let blit src src_pos dst dst_pos len = B.blit_string src src_pos dst dst_pos len

    let concat (sep: OCamlString) (sl: OCamlString list) = Bs.concat sep sl |> U.ofArraySegemnt

    let cat s1 s2 = mnd { let! t1 = s1 in let! t2 = s2 in return B.cat t1 t2 }

    let iter f s = mnd { let! t = s in return! B.iter f t }

    let iteri f s = mnd { let! t = s in return! B.iteri f t }

    let map f s = mnd { let! t = s in return B.map f t }

    let mapi f s = mnd { let! t = s in return B.mapi f t }

    let fold_left f x s = mnd { let! t = s in return B.fold_left f x t }

    let fold_right  f x s = mnd { let! t = s in return B.fold_right f x t }

    type t = OCamlString

    let compare (l: t) (r: t) = LanguagePrimitives.GenericComparison l r

    let equal (l: t) (r: t) = l = r
    
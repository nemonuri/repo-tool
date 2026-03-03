namespace Nemonuri.OCamlDotNet.Forwarded

open System;
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.Operations
open type System.MemoryExtensions
module O = Nemonuri.OCamlDotNet.Primitives.Operations.OCamlByteSpanSources
module Ou = Nemonuri.OCamlDotNet.Primitives.Operations.OCamlByteSpanSources.Unsafe
module B = Nemonuri.OCamlDotNet.Forwarded.Bytes
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/String.html
module String =
    
    let private mnd = TargetToSourceMonad<OCamlBytes, OCamlString>(B.unsafe_to_string, B.unsafe_of_string)


    let make (n: OCamlInt) (c: OCamlChar) : OCamlString = mnd { return B.make n c }
    
    let init (n: OCamlInt) (f: OCamlInt -> OCamlChar) : OCamlString = mnd { return B.init n f }
    
    let empty: OCamlString = mnd { return B.empty }

    let length (s: OCamlString) = mnd { let! b = s in return! B.length b }

    let get (s: OCamlString) (i: OCamlInt) : OCamlChar = mnd { let! b = s in return! B.get b i }

    let of_bytes (s: OCamlBytes) : OCamlString = B.to_string s

    let to_bytes (s: OCamlString) : OCamlBytes = B.of_string s

    let blit src src_pos dst dst_pos len = B.blit_string src src_pos dst dst_pos len

    let concat (sep: OCamlString) (sl: OCamlString list) = Bs.concat sep sl |> Ou.stringOfArraySegment

    let cat s1 s2 = mnd { let! t1 = s1 in let! t2 = s2 in return B.cat t1 t2 }

    let iter f s = mnd { let! t = s in return! B.iter f t }

    let iteri f s = mnd { let! t = s in return! B.iteri f t }

    let map f s = mnd { let! t = s in return B.map f t }

    let mapi f s = mnd { let! t = s in return B.mapi f t }

    let fold_left f x s = mnd { let! t = s in return B.fold_left f x t }

    let fold_right  f x s = mnd { let! t = s in return B.fold_right f x t }

    type t = OCamlString

    let compare (l: t) (r: t) = O.stringCompare l r

    let equal (l: t) (r: t) = O.stringEqual l r

    let sub s pos len = mnd { let! t = s in return B.sub t pos len }
    
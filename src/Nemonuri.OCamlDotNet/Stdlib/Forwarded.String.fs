namespace Nemonuri.OCamlDotNet.Forwarded

open System;
open Nemonuri.OCamlDotNet.Primitives
open type System.MemoryExtensions
module O = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ou = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources.Unsafe
module B = Nemonuri.OCamlDotNet.Forwarded.Bytes
module Bs = ByteSpans

/// https://ocaml.org/manual/5.4/api/String.html
module String =
    
    let internal mnd = TargetToSourceMonad<OCamlBytes, OCamlString>(B.unsafe_to_string, B.unsafe_of_string)


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

    let equal (l: t) (r: t) = O.stringEqual l r

    let compare (l: t) (r: t) = O.stringCompare l r

    let starts_with prefix s  = mnd { let! prefix' = prefix in let! s' = s in return! B.starts_with prefix' s' }

    let ends_with prefix s  = mnd { let! prefix' = prefix in let! s' = s in return! B.ends_with prefix' s' }
    

    let sub s pos len = mnd { let! t = s in return B.sub t pos len }
    
    let uppercase_ascii s = mnd { let! s' = s in return B.uppercase_ascii s' }

    let lowercase_ascii s = mnd { let! s' = s in return B.lowercase_ascii s' }

    let escaped s = mnd { let! s' = s in return B.escaped s' }

    let index s c = mnd { let! s' = s in return! B.index s' c }

    let index_opt s c = mnd { let! s' = s in return! B.index_opt s' c }

    let rindex s c = mnd { let! s' = s in return! B.rindex s' c }

    let rindex_opt s c = mnd { let! s' = s in return! B.rindex_opt s' c }

    let index_from s c = mnd { let! s' = s in return! B.index_from s' c }

    let index_from_opt s c = mnd { let! s' = s in return! B.index_from_opt s' c }

    let rindex_from s c = mnd { let! s' = s in return! B.rindex_from s' c }

    let rindex_from_opt s c = mnd { let! s' = s in return! B.rindex_from_opt s' c }

    
namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.Buffers
open Nemonuri.ByteChars.IO
open Nemonuri.OCamlDotNet.Primitives
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ofd = Nemonuri.OCamlDotNet.Primitives.OCamlFileDescriptors

module Buffer =

    type private bd = DrainableArrayBuilder<byte>
    let private mnd = TargetToSourceMonad<bd, OCamlBuffer>((fun v -> { Value = v }), (fun v -> v.Value))

    
    type t = Nemonuri.OCamlDotNet.Primitives.OCamlBuffer

    let create (n: OCamlInt) : t = mnd { return bd(n) }

    let contents (b: t) : OCamlString = mnd { let! b' = b in return! b'.AsSpan().ToArray() |> Obs.Unsafe.stringOfArray }

    let add_string (b: t) (s: OCamlString) = mnd { let! b' = b in return! b'.AddRange(Obs.stringToReadOnlySpan s) }

    let clear (b: t) = mnd { let! b' = b in return! b'.Clear() }

    let output_buffer (oc: OCamlOutChannel) (b: t) = mnd { 
            let! b':bd = b in return! Ofd.writeByteSpanToOutChannel oc (b'.AsSpan())
        }

    let add_char (b: t) (c: OCamlChar) = mnd { let! b' = b in return! b'.Add(c) }

    let add_subbytes (b: t) (s: OCamlBytes) (ofs: OCamlInt) (len: OCamlInt) =
        mnd { let! b' = b in return! b'.AddRange((Obs.bytesToReadOnlySpan s).Slice(ofs, len)) }
    
    let add_substring b (s: OCamlString) ofs len =
        String.mnd { let! s' = s in return! add_subbytes b s' ofs len }

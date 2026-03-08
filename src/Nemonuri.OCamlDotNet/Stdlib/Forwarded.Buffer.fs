namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.Collections
open Nemonuri.OCamlDotNet.Primitives
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ofd = Nemonuri.OCamlDotNet.Primitives.OCamlFileDescriptors

module Buffer =

    type private bd = DrainableArrayBuilder<byte>
    let private mnd = TargetToSourceMonad<bd, OCamlBuffer>((fun v -> { Value = v }), (fun v -> v.Value))

    
    type t = Nemonuri.OCamlDotNet.Primitives.OCamlBuffer

    let create (n: int) : t = mnd { return bd(n) }

    let contents (b: t) : OCamlString = mnd { let! b' = b in return! b'.AsSpan().ToArray() |> Obs.Unsafe.stringOfArray }

    let add_string (b: t) (s: OCamlString) = mnd { let! b' = b in return! b'.Append(Obs.stringToReadOnlySpan s) }

    let clear (b: t) = mnd { let! b' = b in return! b'.Clear() }

    let output_buffer (oc: OCamlOutChannel) (b: t) = mnd { 
        let! b' = b in 
        return! 
            let ary = b'.AsSpan().ToArray() in
            (Ofd.outChannelToStream oc).Write(ary, 0, ary.Length) 
    }


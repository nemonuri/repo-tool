// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.StringBuffer.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.OCamlDotNet.Zarith

module StringBuffer =


    //This is a **MUTABLE** string buffer
    //Although each function here returns a `t` the buffer is mutated in place.

    //The argument convention is chosen so that you can conveniently write code like:
    // sb |> add "hello" |> add " world" |> add "!"


    (* See FStar.StringBuffer.fsi *)
    type t = Buffer.t

    /// val create : int -> t
    let create (i: Z.t) = Buffer.create (Z.to_int i)

    /// val add: string -> t -> t
    let add s t = Buffer.add_string t s; t

    /// val contents: t -> string
    let contents b = Buffer.contents b

    /// val clear: t -> t
    let clear t = Buffer.clear t; t
    
    /// val output_channel: FStarC.Util.out_channel -> t -> unit
    let output_channel oc b = Buffer.output_buffer oc b

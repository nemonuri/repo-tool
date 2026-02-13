module Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Zarith

//--- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Effect.ml ---

type 'a ref = Microsoft.FSharp.Core.Ref<'a>

let (!) (r: 'a ref) = r.Value
let (:=) (x: 'a ref) y = x.Value <- y
let alloc x = Microsoft.FSharp.Core.Operators.ref x
let mk_ref = alloc
let raise = Microsoft.FSharp.Core.Operators.raise
let exit i = Microsoft.FSharp.Core.Operators.exit (Z.to_int i)

//---|

//--- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Effect.fsti ---

let try_with (doing: unit -> 'a) (handleError: exn -> 'a) : 'a = try doing() with e-> handleError e

exception Failure = Stdlib.Failure

let failwith = Stdlib.failwith

//---|

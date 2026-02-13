module Nemonuri.FStarDotNet.FStarC.Effect

open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Zarith

//--- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Effect.ml ---

type 'a ref = Stdlib.ref<'a>

let op_Bang (r:'a ref) = Stdlib.(!) r
let op_Colon_Equals x y = Stdlib.(:=) x y
let alloc x = Stdlib.ref x
let mk_ref = alloc
let raise = Stdlib.raise
let exit i = Stdlib.exit (Z.to_int i)

//---|

//--- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Effect.fsti ---

let try_with (doing: unit -> 'a) (handleError: exn -> 'a) : 'a = try doing() with e-> handleError e

exception Failure = Stdlib.Failure

let failwith = Stdlib.failwith

//---|

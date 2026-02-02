module Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet

//--- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Effect.ml ---

type 'a ref = Microsoft.FSharp.Core.Ref<'a>

let (!) (r: 'a ref) = r.Value
let (:=) (x: 'a ref) y = x.Value <- y
let alloc x = Microsoft.FSharp.Core.Operators.ref x
let mk_ref = alloc
let raise = Microsoft.FSharp.Core.Operators.raise
let exit i = Microsoft.FSharp.Core.Operators.exit (Z.to_int i)
let Failure = Microsoft.FSharp.Core.Operators.Failure

//---|
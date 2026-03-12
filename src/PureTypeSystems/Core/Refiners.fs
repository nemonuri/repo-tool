namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Microsoft.FSharp.Core.Operators.Unchecked

module Refiners =

    let inline (|Judgement|) (j: Judgement) = let det, tru = j.Deconstruct() in det, tru

    let inline (|Unknown|Thunk|False|True|) (Judgement(det, tru)) =
        match det, tru with
        | false, false -> Unknown Judgement.Unknown
        | false, true -> Thunk Judgement.Thunk
        | true, false -> False Judgement.False
        | true, true -> True Judgement.True

    type RefineResult<'a, 'r when 'r :> IRefinerPremise<'a> and 'r : unmanaged> =
    | RefineError of 'a * Judgement
    | RefineOk of Refined<'a, 'r>


    let refine<'a, 'r when 'r :> IRefinerPremise<'a> and 'r : unmanaged> (x: 'a) =
        let jud, rst = defaultof<'r>.Judge(&x) in
        match jud with
        | True _ -> Refined<'a, 'r>(rst) |> RefineOk
        | _ -> RefineError(rst, jud)
         

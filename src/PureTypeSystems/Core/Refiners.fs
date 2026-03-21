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

    [<NoComparison; NoEquality>]
    [<Struct>]
    type RefineResult<'a, 'r when 'r :> IJudgePremise<'a> and 'r : unmanaged> =
    | RefineError of 'a * Judgement
    | RefineOk of Refined<'a, 'r>


    let refine<'a, 'r when 'r :> IJudgePremise<'a> and 'r : unmanaged> (x: 'a) =
        let jud = defaultof<'r>.Judge(&x) in
        match jud with
        | True _ -> Refined<'a, 'r>(x) |> RefineOk
        | _ -> RefineError(x, jud)
        
    let toResult (r: RefineResult<'a,'r>) : Result<Refined<'a, 'r>, ('a * Judgement)> =
        match r with
        | RefineOk v -> Ok v
        | RefineError (a,j) -> Error (a,j)

    let toValueOption (r: RefineResult<'a,'r>) =
        match r with
        | RefineOk v -> ValueSome v
        | RefineError (a,j) -> ValueNone
    
    let tryRefineV<'a, 'r when 'r :> IJudgePremise<'a> and 'r : unmanaged> (x: 'a) : voption<Refined<'a, 'r>> = x |> refine |> toValueOption

    let trustMe<'a, 'r> (a: 'a) = Refined<'a, 'r>(a)


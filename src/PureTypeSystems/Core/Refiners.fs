namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Primitives.TypeExpressions
open type Nemonuri.PureTypeSystems.Primitives.Extensions.JudgementExtensions
open Microsoft.FSharp.Core.Operators.Unchecked

module Refiners =

    let inline (|Judgement|) (j: Judgement) = let det, tru = j.Deconstruct() in det, tru

    let inline (|Unknown|Testable|False|True|) (r: JudgeResult) =
        let (Judgement(det, tru)) = r.Judgement in
        match det, tru with
        | false, false -> Unknown Judgement.Unknown
        | false, true -> Testable (Judgement.Testable, r.TesterIntroducer)
        | true, false -> False Judgement.False
        | true, true -> True Judgement.True

    [<NoComparison; NoEquality>]
    [<Struct>]
    type RefineResult<'a, 'r> =
    | RefineError of 'a * JudgeResult
    | RefineOk of Refined<'a, 'r>

    let trustMe<'a, 'r when 'r :> IJudgePremise> (x: 'a) = RefinedTheory.TrustMe<'a, 'r>(x)

    let judge<'a, 'r when 'r :> IJudgePremise> (x: 'a) = JudgeTheory.ToHandle<'r,'a>().Judge(&x)

    let refine<'a, 'r when 'r :> IJudgePremise> (x: 'a) =
        let jud = judge<'a,'r> x in
        match jud with
        | True _ -> trustMe<'a, 'r>(x) |> RefineOk
        | _ -> RefineError(x, jud)
        
    let toResult (r: RefineResult<'a,'r>) : Result<Refined<'a, 'r>, ('a * JudgeResult)> =
        match r with
        | RefineOk v -> Ok v
        | RefineError (a,j) -> Error (a,j)

    let toValueOption (r: RefineResult<'a,'r>) =
        match r with
        | RefineOk v -> ValueSome v
        | RefineError (a,j) -> ValueNone
    
    let tryRefineV<'a, 'r when 'r :> IJudgePremise> (x: 'a) : voption<Refined<'a, 'r>> = x |> refine |> toValueOption

    
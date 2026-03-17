namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Microsoft.FSharp.Core.Operators.Unchecked
module R = Nemonuri.PureTypeSystems.Refiners

module Predicates =

    type Refiner<'a, 'p when 'p :> IPredicatePremise<'a> and 'p : unmanaged> =
        struct
            member _.Judge (pre: inref<'a>, post: byref<'a>): Judgement = 
                let p = defaultof<'p> in
                PredicateTheory.JudgeAsRefiner(&p, &pre, &post)

            interface IRefinerPremise<'a> with
                member x.Judge (pre: inref<'a>, post: byref<'a>): Judgement = x.Judge(&pre,&post)
        end

    let refine<'a, 'p when 'p :> IPredicatePremise<'a> and 'p : unmanaged> x = R.refine<'a, Refiner<'a,'p>> x
    
    let tryRefineV<'a, 'p when 'p :> IPredicatePremise<'a> and 'p : unmanaged> x = refine<'a,'p> x |> R.toValueOption
        


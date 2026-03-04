namespace Nemonuri.FStarDotNet.Primitives.Operations

open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Primitives.Abstractions
module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.Operations.FStarValues
module A = Nemonuri.FStarDotNet.Primitives.Abbreviations


module FStarTypeContexts =    

    module Abstractions =

        module Abs = Nemonuri.FStarDotNet.Primitives.Abstractions.Operations.FStarTypeContexts

        module Boxed = 

            let inline witness tc = Abs.Boxed.witness tc

            let inline tail tc = Abs.Boxed.tail tc
        
        let inline witness tc = Abs.witness tc

        let inline tail tc = Abs.tail tc

        let inline boxWitness tc = Abs.boxWitness tc

        let inline boxTail tc = Abs.boxTail tc


    let tail (tc: A.Tc<'t,'h>) = Abstractions.tail tc

    let witness (tc: A.Tc<'t,'h>) = Abstractions.witness tc

    let appendTail (tc: A.Tc<'t1,'s1>) (pur: 't1 -> 't2) : A.Tc<'t2, A.Tc<'t1, 's1>> = { Witness = tc; Pure = tail >> pur }

    let flatTail (tc: A.Tc<A.Tc<'t,'s1>, 's2>) : A.Tc<'t,'s2> = { Witness = tc |> witness; Pure = tc.Pure >> tail }

    let flatWitness (tc: A.Tc<'t1, A.Tc<'t2, 's2>>) (pur: 't2 -> A.Tc<'t2, 's2>) : A.Tc<'t1,'t2> = 
        { 
            Witness = tc |> witness |> tail; 
            Pure = pur >> tc.Pure
        }
    
    let toObj (a: 'a) : FStarObject = { Witness = a |> box }

    let toOmega _ = FStarOmega()

    
module FStarDependentTuples =

    open Nemonuri.FStarDotNet.Primitives.Operations.FStarKinds

    let create<'tail, 'head, 'target> 
        (head: 'head) (target: 'target) 
        : FStarDependentTuple<'tail, 'head, 'target> =
        let bij = getBijection<'tail, 'head, 'target> in
        let ks = bij.Pure.Invoke target in
        FStarDependentTuple(head, ks, bij)


#if false
module FStarKindContexts =

    module A = Nemonuri.FStarDotNet.Primitives.Abbreviations
    module K = Nemonuri.FStarDotNet.Primitives.Operations.FStarKinds

    let inline toMonad(_: A.kc<'k>) = K.KindMonad<'k>()
#endif
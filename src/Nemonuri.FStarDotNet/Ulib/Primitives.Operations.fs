namespace Nemonuri.FStarDotNet.Primitives

open Nemonuri.FStarDotNet.Primitives.Abstractions
module Ftc = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarTypeContexts
module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues
module A = Nemonuri.FStarDotNet.Primitives.Abbreviations


module FStarTypeContexts =    

(*
    let normalize (tc: A.tc<'t,'h>) : A.Tc<'t,'h> =
        match tc with
        | :? A.Tc<'t,'h> as v -> v
        | v -> { Witness = Ftc.witness tc; Lifter = fun _ -> Ftc.tail tc }
*)

    let tail (tc: A.Tc<'t,'h>) = Ftc.tail tc

    let witness (tc: A.Tc<'t,'h>) = Ftc.witness tc

(*
    let lifter (tc: A.Tc<'t,'h>) = tc.Lifter

    let cons 
        (extract: A.Tc<'t,'s1> -> 's2) 
        (pureLift: 's2 -> A.Tc<'t,'s1>) 
        (tc: A.Tc<'t,'s1>) 
        : A.Tc<A.Tc<'t,'s1>, 's2> =
        let wit = extract tc in { Tail = pureLift wit; Witness = wit }

    let castWitness 
        (caster: 's1 -> 's2) 
        (tc: A.Tc<'t,'s1>) 
        : A.Tc<'t,'s2> =
        { Tail = tc |> tail; Witness = tc |> witness |> caster }
*)

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

    open Nemonuri.FStarDotNet.Primitives.FStarKinds

    let create<'tail, 'head, 'target> 
        (head: 'head) (target: 'target) 
        : FStarDependentTuple<'tail, 'head, 'target> =
        let bij = getBijection<'tail, 'head, 'target> in
        let ks = bij.Pure.Invoke target in
        FStarDependentTuple(head, ks, bij)


module FStarTypeUniverses =

    open Nemonuri.FStarDotNet.Primitives.FStarKinds

    module Boxed =

        type Type = FStarOmega
        type typ = A.tc
        type type0 = A.tc<FStarOmega, FStarObject>
        type Type0 = A.Tc<FStarOmega, FStarObject>


    type Type<'a> = A.Tc<Boxed.Type, 'a>
    type typ<'a> = A.tc<Boxed.Type, 'a>
    type type0<'a> = A.tc<Boxed.Type0, 'a>
    type Type0<'a> = A.Tc<Boxed.Type0, 'a>

    type EqType<'a when 'a : equality> = Type0<'a>

    let pur (x: 'a) : Type0<'a> = 
        let pur0 (v: 'a) : Boxed.Type0 = { Witness = v |> FStarTypeContexts.toObj; Pure = FStarTypeContexts.toOmega }
        { Witness = x; Pure = pur0 }

    let inline extract (ty0: Type0<'a>) : 'a = ty0.Witness

    let inline toBoxed (x: Type0<'a>) : Boxed.Type0 = x |> FStarTypeContexts.tail

    type Monad =
        struct
            member inline this.Bind(t1: Type0<'s1>, sf: 's1 -> Type0<'s2>) : Type0<'s2> = sf (extract t1)
            member inline this.Return(s: 's1) : Type0<'s1> = pur s
        end
    
    let inline monad() = Monad()

    type ExtractorMonad = 
        struct
            member inline this.Bind(t1: Type0<'s1>, sf: 's1 -> Type0<'s2>) : Type0<'s2> = monad().Bind(t1, sf)
            member inline this.Return(s: 's1) : Type0<'s1> = monad().Return(s)
            member inline this.Run(s: Type0<'s1>) : 's1 = s |> extract
        end

    let inline emonad() = ExtractorMonad()

    type Comonad =
        struct
            member inline this.Bind(t1: 's1, sf: Type0<'s1> -> 's2) : 's2 = sf (pur t1)
            member inline this.Return(s: Type0<'s1>) : 's1 = extract s
        end

    let inline comonad() = Comonad()

    let type0ToKindSource (x: Type0<'a>) : KindSource<Boxed.Type0, 'a> = { Witness = x.Witness }
    let type0OfKindSource (x: KindSource<Boxed.Type0, 'a>) : Type0<'a> = x.Witness |> pur

    type Type0OfKindSourceMonad = 
        struct
            member inline this.Return(s1: KindSource<Boxed.Type0, 's1>) : Type0<'s1> = type0OfKindSource s1
            member inline this.Bind(t1: Type0<'s1>, sf: KindSource<Boxed.Type0, 's1> -> Type0<'s2>) : Type0<'s2> = type0ToKindSource t1 |> sf
        end

    type Type0ToKindSourceMonad = 
        struct
            member inline this.Return(s1: Type0<'s1>) : KindSource<Boxed.Type0, 's1> = type0ToKindSource s1
            member inline this.Bind(t1: KindSource<Boxed.Type0, 's1>, sf: Type0<'s1> -> KindSource<Boxed.Type0, 's2>) : KindSource<Boxed.Type0, 's2> = type0OfKindSource t1 |> sf
        end

module FStarKindContexts =

    module A = Nemonuri.FStarDotNet.Primitives.Abbreviations
    module K = Nemonuri.FStarDotNet.Primitives.FStarKinds

    let inline toMonad(_: A.kc<'k>) = K.KindMonad<'k>()

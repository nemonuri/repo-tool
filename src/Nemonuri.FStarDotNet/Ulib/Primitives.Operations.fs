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


module FStarTypeUniverses =

    type type0 = A.tc<FStarOmega, FStarObject>
    type Type0 = A.Tc<FStarOmega, FStarObject>
    
    type type0<'a> = A.tc<Type0, 'a>
    type Type0<'a> = A.Tc<Type0, 'a>

    type EqType<'a when 'a : equality> = Type0<'a>

    let inline pur (x: 'a) : Type0<'a> = 
        { 
            Witness = x
            Pure = fun v -> { Witness = v |> FStarTypeContexts.toObj; Pure = FStarTypeContexts.toOmega }
        }

    let inline extract (ty0: Type0<'a>) : 'a = ty0.Witness

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

#if false
module FStarLiftedValues =

    open Nemonuri.FStarDotNet.Primitives.Abbreviations

    let pureLift (x: 'a) : Fv<'a> = { Value = x }

    let extract (x: Fv<'a>) : 'a = Fv.extract x

    type FStar<'a when 'a :> tc> = 'a

    type FStarDelayed<'a when 'a :> tc> = Fv<unit> -> FStar<'a>

    type FStarException<'exn when 'exn :> System.Exception> = Fv<'exn>


    type Monad =
        struct
            member inline this.Bind(t1: Fv<'s1>, sf: 's1 -> Fv<'s2>) : Fv<'s2> = sf (extract t1)
            member inline this.Return(s: 's1) : Fv<'s1> = pureLift s
            member inline this.Zero() : Fv<unit> = pureLift()
        end
    
    type Comonad =
        struct
            member inline this.Bind(t1: 's1, sf: Fv<'s1> -> 's2) : 's2 = sf (pureLift t1)
            member inline this.Return(s: Fv<'s1>) : 's1 = extract s
            member inline this.Zero() : unit = ()
        end

    let inline monad() = Monad()

    let inline comonad() = Comonad()

    let inline map1 tf s1 = monad() { let! t1 = s1 in return tf t1 }
    let inline map2 tf s1 s2 = monad() { let! t1 = s1 in let! t2 = s2 in return tf t1 t2 }
    let inline map3 tf s1 s2 s3 = monad() { let! t1 = s1 in let! t2 = s2 in let! t3 = s3 in return tf t1 t2 t3 }
    let inline curryMap2 tf s1 s2 = monad() {let! t1 = s1 in let! t2 = s2 in return tf (t1, t2)}


module FStarSums =

    open TypeEquality

    let (|LeftOrRight|_|) (v: 'x) : option<FStarSum<'p,'q>> =
        match Teq.tryRefl<'x, 'p> with
        | Some teq -> 
            let r = Teq.cast teq v |> FStarSum<'p,'q>.FStarSumLeft in Some r
        | None ->
        match Teq.tryRefl<'x, 'q> with
        | Some teq -> 
            let r = Teq.cast teq v |> FStarSum<'p,'q>.FStarSumRight in Some r
        | None -> None

    let (|Left|Right|) (sumopt: FStarSum<'p,'q> option) : Choice<'p, 'q> =
        match sumopt with
        | None -> invalidArg (nameof sumopt) "Should not be None."
        | Some sum ->
        match sum with
        | FStarSumLeft (v: 'p) -> Left v
        | FStarSumRight (v: 'q) -> Right v
    
    [<AbstractClass; Sealed>]
    type FStarSumTheory =

        static member inline Create<'p,'q>(x: 'p) = FStarSum<'p,'q>.FStarSumLeft x
        static member inline Create<'p,'q>(x: 'q) = FStarSum<'p,'q>.FStarSumRight x


module FStarDependentTuples =

    open Nemonuri.FStarDotNet.Primitives.Abbreviations

    [<AbstractClass; Sealed>]
    type FStarDependentTypedValueSolverTheory<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                                when 'TTypeImplication :> imp<FStarOmega, 'TSourceTypeContext, FStarThunk<FStarOmega>>
                                                and 'TTypeImplication : unmanaged> =
        static let mutable solver: FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget> option = None

        static member SetSolver(sv: FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget>) = solver <- Some sv

        static member internal GetSolverField() = solver

        static member GetSolver() = Option.get solver
    
    type FStarDependentTupleConstruction<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                                when 'TTypeImplication :> imp<FStarOmega, 'TSourceTypeContext, FStarThunk<FStarOmega>>
                                                and 'TTypeImplication : unmanaged>() =
        class
            do 
                match FStarDependentTypedValueSolverTheory<'TSourceTypeContext, 'TTypeImplication, 'TTarget>.GetSolverField() with
                | Some _ -> ()
                | None -> FStarTypes.raiseInvalid typeof<FStarDependentTupleConstruction<'TSourceTypeContext, 'TTypeImplication, 'TTarget>>
            interface Abstractions.IFStarTypeContext with
                member this.BoxedWitness = FStarOmega() |> Ftc.Boxed.witness
                member this.BoxToTailTypeContext (): IFStarTypeContext = FStarOmega() |> Ftc.Boxed.tail
        end
#endif

module FStarKindContexts =

    module A = Nemonuri.FStarDotNet.Primitives.Abbreviations
    module K = Nemonuri.FStarDotNet.Primitives.FStarKinds

    let inline toMonad(_: A.kc<'k>) = K.KindMonad<'k>()

    


namespace Nemonuri.FStarDotNet.Primitives

open Nemonuri.FStarDotNet.Primitives.Abstractions
module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues

module FStarTypes =

    exception InvalidInhabitant of System.Type

    let raiseInvalid (ty: System.Type) = raise (InvalidInhabitant ty)

module FStarLiftedValues =

    open Nemonuri.FStarDotNet.Primitives.Abbreviations

    let lift (x: 'a) = Fv<'a>.create x

    let embed (x: Fv<'a>) = Fv.embed x

    type FStar<'a when 'a :> tc> = 'a

    type FStarDelayed<'a when 'a :> tc> = Fv<unit> -> FStar<'a>

    type FStarException<'exn when 'exn :> System.Exception> = Fv<'exn>


    type Monad =
        struct
            member inline this.Bind(t1: Fv<'s1>, sf: 's1 -> Fv<'s2>) : Fv<'s2> = sf (embed t1)
            member inline this.Return(s: 's1) : Fv<'s1> = lift s
            member inline this.Zero() : Fv<unit> = lift()
        end
    
    type Comonad =
        struct
            member inline this.Bind(t1: 's1, sf: Fv<'s1> -> 's2) : 's2 = sf (lift t1)
            member inline this.Return(s: Fv<'s1>) : 's1 = embed s
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
    type FStarDependentTupleTheory =

        static member inline Apply<'a, '_1, 'imp, '_2
                                    when 'a :> tc
                                    and '_1 :> thunk<'a>
                                    and 'imp :> imp<'a, '_1, '_2>> 
            (source: '_1) (impl: 'imp) : '_2 =
            let r: '_2 = impl.Invoke(source) in r

    [<AbstractClass; Sealed>]
    type FStarDependentTypedValueSolverTheory<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                                when 'TTypeImplication :> imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                                and 'TTypeImplication : unmanaged> =
        static let mutable solver: FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget> option = None

        static member SetSolver(sv: FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget>) = solver <- Some sv

        static member internal GetSolverField() = solver

        static member GetSolver() = Option.get solver
    
    type FStarDependentTupleConstruction<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                            when 'TTypeImplication :> imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                            and 'TTypeImplication : unmanaged>() =
        class
            do 
                match FStarDependentTypedValueSolverTheory<'TSourceTypeContext, 'TTypeImplication, 'TTarget>.GetSolverField() with
                | Some _ -> ()
                | None -> FStarTypes.raiseInvalid typeof<FStarDependentTupleConstruction<'TSourceTypeContext, 'TTypeImplication, 'TTarget>>
            interface Abstractions.IFStarTypeContext with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarObjectType.Tail
                member this.GetWitness (d: outref<objnull>) = d <- FStarObjectType.Witness
        end

namespace Nemonuri.FStarDotNet.Primitives

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


    type FunctorBuilder =
        struct
            member inline this.Bind(t1: Fv<'s1>, sf: 's1 -> Fv<'s2>) : Fv<'s2> = sf (embed t1)
            member inline this.Return(s: 's1) : Fv<'s1> = lift s
            member inline this.Zero() : Fv<unit> = lift()
        end

    let inline functor() = FunctorBuilder()

    let inline map1 tf s1 = functor() { let! t1 = s1 in return tf t1 }
    let inline map2 tf s1 s2 = functor() { let! t1 = s1 in let! t2 = s2 in return tf t1 t2 }
    let inline map3 tf s1 s2 s3 = functor() { let! t1 = s1 in let! t2 = s2 in let! t3 = s3 in return tf t1 t2 t3 }
    let inline curryMap2 tf s1 s2 = functor() {let! t1 = s1 in let! t2 = s2 in return tf (t1, t2)}


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

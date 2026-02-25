namespace Nemonuri.FStarDotNet.Primitives

module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues

module FStarTypes =

    exception InvalidInhabitant of System.Type

    let raiseInvalid (ty: System.Type) = raise (InvalidInhabitant ty)

module Functions =

    let inline curry f s1 s2 = f (s1, s2)

module FStarLiftedValues =

    type FunctorBuilder =
        struct
            member inline this.Bind(x: FStarLiftedValue<'a>, f: 'a -> 'b) = Fv.embed x |> f
            member inline this.Return(x: 'a) = FStarLiftedValue<'a>.create x
        end

    let inline map1 tf s1 = FunctorBuilder() { let! t1 = s1 in return tf t1 }
    let inline map2 tf s1 s2 = FunctorBuilder() { let! t1 = s1 in let! t2 = s2 in return tf t1 t2 }
    let inline map3 tf s1 s2 s3 = FunctorBuilder() { let! t1 = s1 in let! t2 = s2 in let! t3 = s3 in return tf t1 t2 t3 }
    let inline curryMap2 tf s1 s2 = (tf |> Functions.curry |> map2) s1 s2

    let create (x: 'a) = FStarLiftedValue<'a>.create x

namespace Nemonuri.FStarDotNet.Primitives

module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues

module FStarTypes =

    exception InvalidInhabitant of System.Type

    let raiseInvalid (ty: System.Type) = raise (InvalidInhabitant ty)

module Functions =

    let inline curry f s1 s2 = f (s1, s2)

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
(*
            member inline this.Bind2(t1: Fv<'s1>, t2: Fv<'s2>, sf: 's1 * 's2 -> Fv<'s2>) : Fv<'s2> = sf (embed t1, embed t2)
            member inline this.BindReturn(t1: Fv<'s1>, sf: 's1 -> 's2) : Fv<'s2> = sf (embed t1) |> lift
            member inline this.Bind2Return(t1: Fv<'s1>, t2: Fv<'s2>, sf: 's1 * 's2 -> 's3) : Fv<'s3> = sf (embed t1, embed t2) |> lift
            member inline this.Bind3Return(t1: Fv<'s1>, t2: Fv<'s2>, t3: Fv<'s3>, sf: 's1 * 's2 * 's3 -> 's4) : Fv<'s4> = sf (embed t1, embed t2, embed t3) |> lift
*)            
            member inline this.Return(s: 's1) : Fv<'s1> = lift s
            //member inline this.ReturnFrom(s: Fv<'s1>) : Fv<'s1> = s
            member inline this.Zero() : Fv<unit> = lift()
        end

    let inline map1 tf s1 = FunctorBuilder() { let! t1 = s1 in return tf t1 }
    let inline map2 tf s1 s2 = FunctorBuilder() { let! t1 = s1 in let! t2 = s2 in return tf t1 t2 }
    let inline map3 tf s1 s2 s3 = FunctorBuilder() { let! t1 = s1 in let! t2 = s2 in let! t3 = s3 in return tf t1 t2 t3 }
    let inline curryMap2 tf s1 s2 = FunctorBuilder() {let! t1 = s1 in let! t2 = s2 in return tf (t1, t2)}



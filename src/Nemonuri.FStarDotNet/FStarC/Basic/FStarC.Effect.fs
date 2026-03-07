// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Effect.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Primitives.Abbreviations
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

module Effect =

#if false
    new_effect ALL = ALL_h unit

    let all_pre = all_pre_h unit
    let all_post' (a : Type) (pre:Type) = all_post_h' unit a pre
    let all_post (a : Type) = all_post_h unit a
    let all_wp (a : Type) = all_wp_h unit a

    let lift_pure_all (a:Type) (p:pure_wp a)
    : all_wp a
    = fun post h -> p (fun x -> post (V x) h)

    sub_effect PURE ~> ALL { lift_wp = lift_pure_all }

    sub_effect DIV ~> ALL { lift_wp = lift_pure_all }

    effect All (a:Type) (pre:all_pre) (post:(h:unit -> Tot (all_post' a (pre h)))) =
    ALL a
        (fun (p : all_post a) (h : unit) -> pre h /\ (forall ra h1. post h ra h1 ==> p ra h1))

    effect ML (a:Type) = ALL a (fun (p:all_post a) (_:unit) -> forall (a:result a) (h:unit). p a h)
#endif

    type ML<'a> = 'a

    type ref<'a> = Core.ref<'a>

    let (!) (r: ref<'a>) : ML<'a> = r.Value

    let (:=) (r: ref<'a>) (x: 'a) : ML<unit> = r.Value <- x
        

    let alloc (x: 'a) : ref<'a> = { contents = x }
    let mk_ref x = alloc x

    let raise (e: Prims.exn) : ML<'a> = Operators.raise e

    let exit (n: Prims.int) : ML<'a> = 
        let n32: int = n |> bigint.op_Explicit in
        Operators.exit n32
    

    let try_with (s1: Prims.unit -> 'a) (s2: Prims.exn -> 'a) : ML<'a> = 
        try
            s1()
        with
            e -> s2 e


    let Failure (msg: Prims.string) : Prims.exn = Operators.Failure (Obs.stringToDotNetString msg)

    let failwith (msg: Prims.string) : ML<'a> = Operators.failwith (Obs.stringToDotNetString msg)

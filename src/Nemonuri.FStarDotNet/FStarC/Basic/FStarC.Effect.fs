// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Effect.fsti

(*
   Copyright 2008-2017 Microsoft Research

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*)

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Primitives.Abbreviations
module Flv = Nemonuri.FStarDotNet.Primitives.FStarLiftedValues

[<RequireQualifiedAccess>]
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

    type ML<'a when 'a :> tc> = 'a

    type ref<'a when 'a :> tc> = Fv<Core.ref<'a>>

    let (!) (r: ref<'a>) : ML<'a> = Flv.extract r |> _.Value

    let (:=) (r: ref<'a>) (x: 'a) : ML<Prims.unit> = 
        let t1 = Flv.extract r in
        Fv.create (t1.Value <- x)

    let alloc (x: 'a) : ref<'a> = Fv.create { contents = x }
    let mk_ref x = alloc x

    let raise (e: Prims.exn) : ML<'a> = Core.Operators.raise (Flv.extract e)

    let exit (n: Prims.int) : ML<'a> = Core.Operators.exit (Flv.extract n |> bigint.op_Explicit)

    let try_with (s1: Prims.unit -> ML<'a>) (s2: Prims.exn -> ML<'a>) : ML<'a> = 
        try
            s1 (Prims.unit.create())
        with
            e -> Fv.create e |> s2

    //exception Failure of string

    let Failure (msg: Prims.string) : Prims.exn = Flv.map1 (fun tmsg -> System.Exception(tmsg)) msg

    let failwith (msg: Prims.string) : ML<'a> = Core.Operators.failwith (Flv.extract msg)

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
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

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

    type ML<'a> = Prims.Type0<'a>

    type ref<'a> = Prims.Type0<Core.ref<'a>>

    let (!) (r: ref<'a>) : ML<'a> = Fu.monad() { let! tr = r in return tr.Value }

    let (:=) (r: ref<'a>) (x: 'a) : ML<unit> = Fu.monad() { let! tr = r in return tr.Value <- x }
        

    let alloc (x: 'a) : ref<'a> = Fu.monad() { return { contents = x } }
    let mk_ref x = alloc x

    let raise (e: Prims.exn) : ML<'a> = Fu.emonad() { let! te = e in return Operators.raise te }

    let exit (n: Prims.int) : ML<'a> = Fu.emonad() { 
        let! tn = n in 
        let n32: int = tn |> bigint.op_Explicit in
        return Operators.exit n32
    }

    let try_with (s1: ML<Prims.unit -> 'a>) (s2: ML<Prims.exn -> 'a>) : ML<'a> = 
        try
            Fu.monad() { let! t1 = s1 in return () |> Fu.pur |> t1 }
        with
            e -> Fu.monad() { let! t2 = s2 in return e |> Fu.pur |> t2 }


    let Failure (msg: Prims.string) : Prims.exn = Fu.monad() { let! tmsg = msg in return Operators.Failure tmsg }

    let failwith (msg: Prims.string) : ML<'a> = Fu.emonad() { let! tmsg = msg in return Operators.failwith tmsg }

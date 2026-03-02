// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/FStar.Pervasives.Native.fst

namespace Nemonuri.FStarDotNet.Forwarded

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives

module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

(*
   Copyright 2008-2018 Microsoft Research

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
[<RequireQualifiedAccess>]
module FStar_Pervasives_Native =

    (** [option a] represents either  [Some a]-value or a non-informative [None]. *)
    let None<'a> = Fu.monad { let r: Core.option<'a> = Core.None in return r }
    let Some (v: 'a) = Fu.monad { return Core.Some v }

    [<FStarConstructorProxy(nameof None)>]
    [<FStarConstructorProxy(nameof Some)>]
    type option<'a> = Prims.Type0<Core.option<'a>>

    let (|None|Some|) (x: option<'a>) =
        Fu.emonad {
            match! x with
            | Core.None -> return None
            | Core.Some v -> return Some v
        }


    (**** Tuples *)

    /// Aside from special support in extraction, the tuple types have
    /// special syntax in F*.
    ///
    /// For instance, rather than [tupleN a1 ... aN],
    /// we usually write [a1 & ... & aN] or [a1 * ... * aN].
    ///
    /// The latter notation is more common for those coming to F* from
    /// OCaml or F#. However, the [*] also clashes with the multiplication
    /// operator on integers define in FStar.Mul. For this reason, we now
    /// prefer to use the [&] notation, though there are still many uses
    /// of [*] remaining.
    ///
    /// Tuple values are introduced using as [a1, ..., an], rather than
    /// [MktupleN a1 ... aN].
    ///
    /// We define tuples up to a fixed arity of 14. We have considered
    /// splitting this module into 14 different modules, one for each
    /// tuple type rather than eagerly including 14-tuples in the
    /// dependence graph of all programs.

    (** Pairs: [tuple2 a b] is can be written either as [a * b], for
        notation compatible with OCaml's. Or, better, as [a & b]. *)
    let Mktuple2 (_1: 'a) (_2: 'b) = Fu.monad { return (_1, _2) }
    [<FStarConstructorProxy(nameof Mktuple2)>]
    type tuple2<'a, 'b> = Prims.Type0<'a * 'b>
    let (|Mktuple2|) (x: tuple2<_,_>) = Fu.comonad { return x }
        
    let fst (Mktuple2(_1, _)) = _1
    let snd (Mktuple2(_, _2)) = _2


    let Mktuple3 _1 _2 _3 = Fu.monad { return _1, _2, _3 }
    [<FStarConstructorProxy(nameof Mktuple3)>]
    type tuple3<'a, 'b, 'c> = Prims.Type0<'a * 'b * 'c>
    let (|Mktuple3|) (x: tuple3<_,_,_>) = Fu.comonad { return x }


    let Mktuple4 _1 _2 _3 _4 = Fu.monad { return _1, _2, _3, _4 }
    [<FStarConstructorProxy(nameof Mktuple4)>]
    type tuple4<'a, 'b, 'c, 'd> = Prims.Type0<'a * 'b * 'c * 'd>
    let (|Mktuple4|) (x: tuple4<_,_,_,_>) = Fu.comonad { return x }

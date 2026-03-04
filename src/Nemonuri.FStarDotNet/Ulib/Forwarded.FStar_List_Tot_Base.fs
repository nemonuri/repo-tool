// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/FStar.List.Tot.Base.fst

namespace Nemonuri.FStarDotNet.Forwarded

open Nemonuri.FStarDotNet

module FStar_List_Tot_Base =

    (** [append l1 l2] appends the elements of [l2] to the end of [l1]. Named as: OCaml, F#. Similar to: [List.app] in Coq. *)
    /// val append: list 'a -> list 'a -> Tot (list 'a)
    let rec append x y = 
        match x with
        | Prims.Nil -> y
        | Prims.Cons(a,tl) -> Prims.Cons a (append tl y)
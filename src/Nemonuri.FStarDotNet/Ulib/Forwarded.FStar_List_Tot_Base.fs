#nowarn "25" // Incomplete pattern matches

// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/FStar.List.Tot.Base.fst

namespace Nemonuri.FStarDotNet.Forwarded

open Nemonuri.FStarDotNet.Operators

module FStar_List_Tot_Base =

    (**
    Base operations
    *)

    (** [isEmpty l] returns [true] if and only if [l] is empty  *)
    /// val isEmpty: list 'a -> Tot bool
    let isEmpty l = 
        match l with
        | [] -> true
        | _ -> false

    (** [length l] returns the total number of elements in [l]. Named as
    in: OCaml, F#, Coq *)
    /// val length: list 'a -> Tot nat
    let rec length = function
        | [] -> (toInt 0)
        | _::tl -> (toInt 1) + length tl


    (** [existsb f l] returns [true] if, and only if, there exists some
    element [x] in [l] such that [f x] holds. *)
    /// val existsb: #a:Type
    ///     -> f:(a -> Tot bool)
    ///     -> list a
    ///     -> Tot bool
    let rec existsb (f: 'a -> bool) (l: list<'a>) : bool = 
        match l with
        | [] -> false
        | hd::tl -> if f hd then true else existsb f tl

    (** [rev_acc l1 l2] appends the elements of [l1] to the beginning of
    [l2], in reverse order. It is equivalent to [append (rev l1) l2], but
    is tail-recursive. Similar to: [List.rev_append] in OCaml, Coq. *)
    /// val rev_acc: list 'a -> list 'a -> Tot (list 'a)
    let rec rev_acc l acc = 
        match l with
        | [] -> acc
        | hd::tl -> rev_acc tl (hd::acc)

    (** [rev l] returns the list [l] in reverse order. Named as in: OCaml,
    F#, Coq. *)
    /// val rev: list 'a -> Tot (list 'a)
    let rev l = rev_acc l []

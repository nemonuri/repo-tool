#nowarn "1173"
#nowarn "1174"

// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Ord.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Ord.fst

namespace Nemonuri.FStarDotNet.FStarC.Class

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Order
open Nemonuri.FStarDotNet.FStarC.Class.Deq
open Microsoft.FSharp.Core.Operators.Unchecked

module Ord =

    type ord<'a> =
        interface
            abstract member super: deq<'a>
            abstract member cmp: 'a -> 'a -> order
        end
    
#if false

val sort
  (#a:Type) {| ord a |}
  (xs : list a)
  : list a

val sort_by
  (#a:Type) (f : a -> a -> order)
  (xs : list a)
  : list a

(* Deduplicate elements, preserving order as determined by the leftmost
occurrence. So dedup [a,b,c,a,f,e,c] = [a,b,c,f,e] *)
val dedup
  (#a:Type) {| ord a |}
  (xs : list a)
  : list a

(* Sort and deduplicate at once *)
val sort_dedup
  (#a:Type) {| ord a |}
  (xs : list a)
  : list a

(* Returns the difference of two lists, modulo order and duplication.
The first component is the elements only present in xs, and the second
is the elements only present in ys. *)
val ord_list_diff (#a:Type0) {| ord a |} (xs ys : list a) : list a & list a

instance val ord_eq (a:Type) (d : ord a) : Tot (deq a)

val (<?)  : #a:Type -> {| ord a |} -> a -> a -> bool
val (<=?) : #a:Type -> {| ord a |} -> a -> a -> bool
val (>?)  : #a:Type -> {| ord a |} -> a -> a -> bool
val (>=?) : #a:Type -> {| ord a |} -> a -> a -> bool

val min : #a:Type -> {| ord a |} -> a -> a -> a
val max : #a:Type -> {| ord a |} -> a -> a -> a

#endif

    let (<?) (_0: #ord<'a>) (x: 'a) (y: 'a) = _0.cmp x y =. Lt
    let (<=?) (_0: #ord<'a>) (x: 'a) (y: 'a) = _0.cmp x y <>. Gt
    let (>?) (_0: #ord<'a>) (x: 'a) (y: 'a) = _0.cmp x y =. Gt
    let (>=?) (_0: #ord<'a>) (x: 'a) (y: 'a) = _0.cmp x y <>. Lt

    let min (_0: #ord<'a>) (x: 'a) (y: 'a) = if x </(<=?) _0/> y then x else y
    let max (_0: #ord<'a>) (x: 'a) (y: 'a) = if x </(>=?) _0/> y then x else y

    /// instance ord_eq (a:Type) (d : ord a) : Tot (deq a) = d.super
    type ord_eq<'a, 'd when 'd :> ord<'a> and 'd : unmanaged> =
        struct
            interface deq<'a> with
                member this.(=?) x y = defaultof<'d>.super.(=?) x y
        end


    let rec sort (_0: #ord<'a>) xs =
        let rec insert (x:'a) (xs:list<'a>) : list<'a> =
            match xs with
            | [] -> [x]
            | y::ys -> if x </(<=?) _0/> y then x :: y :: ys else y :: insert x ys
        in
        match xs with
        | [] -> []
        | x::xs -> insert x (sort _0 xs)

    (* An advantage of not having instance canonicity:
    we can just construct a dictionary with this new function
    without to use a newtype (which would involve a traversal
    of the list to convert into!). *)
    let sort_by<'a> (f : 'a -> 'a -> order) xs =
        let d : ord<'a> = { 
            new ord<'a> with
                member _.super = { new deq<'a> with member _.(=?) a b = f a b =. Eq }
                member _.cmp x y = f x y
        } in
        sort (* #a *) d xs

    let dedup (_0: #ord<'a>) xs =
        let out = FStar.List.fold_left (fun out x -> if FStar.List.Tot.Base.existsb (fun y -> x </_0.super.(=?)/> y) out then out else x :: out) [] xs in
        FStar.List.Tot.Base.rev out

    let rec insert_nodup  ( _0: #ord<'a> ) (x:'a) (xs:list<'a>) : list<'a> =
        match xs with
        | [] -> [x]
        | y::ys ->
            match _0.cmp x y with
            | Eq -> xs
            | Lt -> x :: xs
            | Gt -> y :: insert_nodup _0 x ys

    let rec sort_dedup (_0: #ord<'a>) xs =
        match xs with
        | [] -> []
        | x::xs -> insert_nodup _0 x (sort_dedup _0 xs)

    let ord_list_diff (_0: #ord<'a>) (xs: list<'a>) (ys: list<'a>) : list<'a> * list<'a> =
        let xs = xs |> sort_dedup _0 in
        let ys = ys |> sort_dedup _0 in
        let rec go (xd, yd) xs ys : list<'a> * list<'a> =
            match xs, ys with
            | x::xs, y::ys -> (
                    match _0.cmp x y with
                    | Lt -> go (x::xd, yd)    xs      (y::ys)
                    | Eq -> go (xd,    yd)    xs      ys
                    | Gt -> go (xd,    y::yd) (x::xs) ys
                )
            (* One of the two is empty, that's it *)
            | xs, ys -> (List.rev_append xd xs, List.rev_append yd ys)
        in
        go ([], []) xs ys

    type ord_int =
        struct
            interface ord<Prims.int> with
                member _.super = defaultof<deq_int>
                member _.cmp x y = compare_int x y
        end
    

#if false
    instance ord_bool : ord bool = {
        super = solve;
        cmp = compare_bool;
    }

    instance ord_unit : ord unit = {
        super = solve;
        cmp = (fun _ _ -> Eq);
    }

    instance ord_string : ord string = {
        super = solve;
        cmp = (fun x y -> order_from_int (String.compare x y));
    }

    instance ord_option #a (d : ord a) : Tot (ord (option a)) = {
        super = solve;
        cmp = (fun x y -> match x, y with
            | None, None -> Eq
            | Some _, None -> Gt
            | None, Some _ -> Lt
            | Some x, Some y -> cmp x y
            );
    }

    instance ord_list #a (d : ord a) : Tot (ord (list a)) = {
    super = solve;
    cmp = (fun l1 l2 -> compare_list l1 l2 cmp);
    }

    instance ord_either #a #b (d1 : ord a) (d2 : ord b) : Tot (ord (either a b)) = {
    super = solve;
    cmp = (fun x y -> match x, y with
            | Inl _, Inr _ -> Lt
            | Inr _, Inl _ -> Gt
            | Inl x, Inl y -> cmp x y
            | Inr x, Inr y -> cmp x y
            );
    }
#endif
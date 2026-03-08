// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Order.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Order.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators

module Order =

    type order = | Lt | Eq | Gt


    // Some derived checks
    let ge (o : order) : bool = o <> Lt
    let le (o : order) : bool = o <> Gt
    let ne (o : order) : bool = o <> Eq

    // Just for completeness and consistency...
    let gt (o : order) : bool = o = Gt
    let lt (o : order) : bool = o = Lt
    let eq (o : order) : bool = o = Eq

    // Lexicographical combination, thunked to be lazy
    let lex (o1 : order) (o2 : unit -> order) : order =
        match o1, o2 with
        | Lt, _ -> Lt
        | Eq, _ -> o2 ()
        | Gt, _ -> Gt

    let order_from_int (i : Prims.int) : order =
        if i <. (toInt 0) then Lt
        else if i =. (toInt 0) then Eq
        else Gt

    let compare_int (i : Prims.int) (j : Prims.int) : order = order_from_int (i -. j)

    let compare_bool (b1: bool) (b2 : bool) : order =
        match b1, b2 with
        | false, true -> Lt
        | true, false -> Gt
        | _ -> Eq

    (*
    * It promises to call the comparator in strictly smaller elements
    * Useful when writing a comparator for an inductive type,
    *   that contains the list of itself as an argument to one of its
    *   data constructors
    *)
    let rec compare_list<'a>
        (l1 :list<'a>) (l2 :list<'a>)
        //(f:(x:a{x << l1} -> y:a{y << l2} -> order))
        (f:('a -> 'a -> order))
        : order
        = match l1, l2 with
            | [], [] -> Eq
            | [], _ -> Lt
            | _, [] -> Gt
            | x::xs, y::ys -> lex (f x y) (fun _ -> compare_list xs ys f)

    let compare_option (f : 'a -> 'a -> order) (x : option<'a>) (y : option<'a>) : order =
        match x, y with
        | None   , None   -> Eq
        | None   , Some _ -> Lt
        | Some _ , None   -> Gt
        | Some x , Some y -> f x y

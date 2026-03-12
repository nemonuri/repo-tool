
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Common.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Common.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Operators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.PSMap
module List = Nemonuri.FStarDotNet.FStar.List
module BU = Nemonuri.FStarDotNet.FStarC.Util
module SB = Nemonuri.FStarDotNet.FStarC.StringBuffer

module Common =

#if false



val rollback (msg:string) (pop: unit -> 'a) (stackref: ref (list 'c)) (depth: option int) : 'a

val runtime_assert (b:bool) (msg:string) : unit

(* Why two? This function was added during a refactoring, and
both variants existed. We cannot simply move to ";" since that is a
breaking change to anything that parses F* source code (like Vale). *)
val string_of_list  : ('a -> string) -> list 'a -> string
val string_of_list' : ('a -> string) -> list 'a -> string

val list_of_option (o:option 'a) : list 'a

val string_of_option (f : 'a -> string) (o : option 'a) : string

(* Was List.init, but F* doesn't have this in ulib *)
val tabulate (n:int) (f : int -> 'a) : list 'a

(** max_prefix f xs returns (l, r) such that
  * every x in l satisfies f
  * l@r == xs
  * and l is the largest list satisfying that
  *)
val max_prefix (f : 'a -> bool) (xs : list 'a) : list 'a & list 'a

(** max_suffix f xs returns (l, r) such that
  * every x in r satisfies f
  * l@r == xs
  * and r is the largest list satisfying that
  *)
val max_suffix (f : 'a -> bool) (xs : list 'a) : list 'a & list 'a

val eq_list (f: 'a -> 'a -> bool) (l1 l2 : list 'a) : bool

val psmap_to_list (m : PSMap.t 'a) : list (string & 'a)
val psmap_keys (m : PSMap.t 'a) : list string
val psmap_values (m : PSMap.t 'a) : list 'a

val option_to_list (o : option 'a) : list 'a
#endif

    /// val snapshot (msg:string) (push: 'a -> 'b) (stackref: ref (list 'c)) (arg: 'a) : (int & 'b)
    let snapshot msg (push: 'a -> 'b) (stackref: ref<list<'c>>) (arg: 'a) : (Prims.int * 'b) = BU.atomically (fun () ->
        let len : Prims.int = List.Tot.Base.length !stackref in
        let arg' = push arg in
        if FStarC.Debug.any () then Format.print2 (toString "(%s)snapshot %s\n"B) msg (Prims.string_of_int len);
        (len, arg'))

    let rollback msg (pop: unit -> 'a) (stackref: ref<list<'c>>) (depth: option<Prims.int>) =
        if FStarC.Debug.any () then Format.print2 (toString "(%s)rollback %s ... "B) msg (match depth with None -> (toString "None"B) | Some len -> Prims.string_of_int len);
        let rec aux n : 'a =
            if n <=. (toInt 0) then failwith (toString "(rollback) Too many pops"B)
            else if n =. (toInt 1) then pop ()
            else (ignore (pop ()); aux (n - (toInt 1))) in
        let curdepth = List.Tot.Base.length !stackref in
        let n = match depth with Some d -> curdepth - d | None -> (toInt 1) in
        if FStarC.Debug.any () then Format.print1 (toString " depth is %s\n "B)(Prims.string_of_int (List.Tot.Base.length (!stackref)));
        BU.atomically (fun () -> aux n)

    // This function is separate to make it easier to put breakpoints on it
    let raise_failed_assertion msg =
        failwith (Format.fmt1 (toString "Assertion failed: %s"B) msg)

    let runtime_assert b msg =
        if not b then raise_failed_assertion msg

    let __string_of_list (delim:Prims.string) (f : 'a -> Prims.string) (l : list<'a>) : Prims.string =
        match l with
        | [] -> (toString "[]"B)
        | x::xs ->
            let strb = SB.create (toInt 80) in
            strb |> SB.add (toString "["B) |> SB.add (f x) |> ignore;
            List.iter (fun x ->
                            strb |> SB.add delim |> SB.add (f x) |> ignore
                        ) xs ;
            strb |> SB.add (toString "]"B) |> ignore;
            SB.contents strb

    (* Why two? This function was added during a refactoring, and
    both variants existed. We cannot simply move to (toString ";"B) since that is a
    breaking change to anything that parses F* source code (like Vale). *)
    let string_of_list  f l = __string_of_list (toString ", "B) f l
    let string_of_list' f l = __string_of_list (toString "; "B) f l

    let list_of_option (o:option<'a>) : list<'a> =
        match o with
        | None -> []
        | Some x -> [x]

    let string_of_option f = function
        | None -> (toString "None"B)
        | Some x -> (toString "Some "B) ^ f x

    (* Was List.init, but F* doesn't have this in ulib *)
    let tabulate (n:Prims.int) (f : Prims.int -> 'a) : list<'a> =
        let rec aux i : list<'a> =
            if i <. n
            then f i :: aux (i + (toInt 1))
            else []
        in aux (toInt 0)

    (** max_prefix f xs returns (l, r) such that
        * every x in l satisfies f
        * l@r == xs
        * and l is the largest list satisfying that
        *)
    let rec max_prefix (f : 'a -> bool) (xs : list<'a>) : list<'a> * list<'a> =
        match xs with
        | [] -> [], []
        | x::xs when f x ->
            let l, r = max_prefix f xs in
            (x::l, r)
        | x::xs ->
            ([], x::xs)

    (** max_suffix f xs returns (l, r) such that
        * every x in r satisfies f
        * l@r == xs
        * and r is the largest list satisfying that
        *)
    let max_suffix (f : 'a -> bool) (xs : list<'a>) : list<'a> * list<'a> =
        let rec aux acc xs : list<'a> * list<'a> =
            match xs with
            | [] -> acc, []
            | x::xs when f x ->
                aux (x::acc) xs
            | x::xs ->
                (acc, x::xs)
        in
        xs |> List.Tot.Base.rev |> aux [] |> (fun (xs, ys) -> List.Tot.Base.rev ys, xs)

    let rec eq_list (f: 'a -> 'a -> bool) (l1: list<'a>) (l2 : list<'a>)
        : bool
        = match l1, l2 with
            | [], [] -> true
            | [], _ | _, [] -> false
            | x1::t1, x2::t2 -> f x1 x2 && eq_list f t1 t2

    let psmap_to_list m =
        psmap_fold m (fun k v a -> (k,v)::a) []
    let psmap_keys m =
        psmap_fold m (fun k v a -> k::a) []
    let psmap_values m =
        psmap_fold m (fun k v a -> v::a) []

    let option_to_list = function
        | None -> []
        | Some x -> [x]
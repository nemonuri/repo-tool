// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Stats.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Stats.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStar.Pervasives
open Nemonuri.FStarDotNet.FStar.Pervasives.Native
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.Class.Show
open Microsoft.FSharp.Core.Operators.Unchecked

module Stats =

    (* We only record stats when this is set to true. This is
    set by the Options module. *)
    /// val enabled : ref bool
    let enabled = alloc false

    (* This is set to true by the Options module whenever
    --stats true is passed, and never set to false. We use it
    to decide whether to print the stats at the end of
    the run. *)
    /// val ever_enabled : ref bool
    let ever_enabled = alloc false

#if false



#endif

    type stat = {
        ns_tree  : Prims.int;
        ns_exn   : Prims.int;
        ns_sub   : Prims.int;
        ncalls   : Prims.int;
    }

    //instance _ : monoid stat = {
    let private mzero = {
            ns_tree = (toInt 0);
            ns_exn = (toInt 0);
            ns_sub = (toInt 0);
            ncalls = (toInt 0);
        }
    let private mplus = (fun s1 s2 ->
            {
                ns_tree  = s1.ns_tree +. s2.ns_tree;
                ns_exn   = s1.ns_exn +. s2.ns_exn;
                ns_sub   = s1.ns_sub +. s2.ns_sub;
                ncalls   = s1.ncalls +. s2.ncalls;
            })
    //}

    (* the ref bool marks whether a given key is currently
        being recorded. This is so we avoid double counting
        the time taken by reentrant calls. *)
    let st : SMap.t<ref<bool> * stat> = SMap.create (toInt 10)

    (* Current stats we are logging. This is used to distinguish
        "tree" time (all the time taken by some call)
        vs "point" time (time taken by some call, subtracting
        the time in subcalls, if any). *)
    let stack : ref<list<Prims.string>> = mk_ref []

    let r_running (k : Prims.string) : ref<bool> =
        match SMap.try_find st k with
        | None ->
            let r = alloc false in
            SMap.add st k (r, mzero);
            r
        | Some (r, _) ->
            r

    let add (k : Prims.string) (s1 : stat) : unit =
        let (r, s0) =
            match SMap.try_find st k with
            | None -> (alloc false, mzero)
            | Some rs -> rs
        in
        SMap.add st k (r, mplus s0 s1)

    let do_record
        (key : Prims.string)
        (f : unit -> 'a)
        : 'a
        =
        stack := key :: !stack;
        let running = r_running key in
        let was_running = !running in
        running := true;
        let t0 = Timing.now_ns () in
        let resexn =
            try Inr (f ())
            with | e -> Inl e
        in
        running := was_running;
        let t1 = Timing.now_ns () in
        let ns = Timing.diff_ns t0 t1 in
        stack := FStar.List.tl !stack;
        if not was_running then (
            add key { mzero with ns_tree = ns };
            (* Take time out of the parent, if any. *)
            begin match !stack with
            | [] -> ()
            | k_par::_ -> add k_par { mzero with ns_sub = ns }
            end
        );
        add key { mzero with ncalls = (toInt 1) };
        match resexn with
        | Inr r ->
            r
        | Inl e ->
            add key { mzero with ns_exn = ns };
            raise e

    (* Count the time taken by `f ()` under a given stats key. *)
    /// val record
    ///     (key : Prims.string)
    ///     (f : unit -> 'a)
    ///     : 'a
    let record key f =
        if !enabled then
            do_record key f
        else
            f ()

    let lpad (len:Prims.int) (s:Prims.string) : Prims.string =
        let l = String.length s in
        if l >=. len then s else String.make (len - l) (toChar ' 'B) ^. s

    let max x y =
        if x >. y then x else y

    (* Generates a message with a table for all stat keys. *)
    /// val print_all () : Prims.string
    let print_all () : Prims.string =
        let keys = SMap.keys st in
        let points = List.map (fun k -> k, snd <| (Some'v <| SMap.try_find st k)) keys in // F* 와 F# 은, <| 우선순위가 묘하게 다르구나.
        (* Sort by (point) time. *)
        let points =
            points |>
            Class.Ord.sort_by (fun (_, s1) (_, s2) ->
                (s2.ns_tree -. s2.ns_sub) </(defaultof<Class.Ord.ord_int> :> Class.Ord.ord<Prims.int>).cmp/> (s1.ns_tree -. s1.ns_sub))
        in
        let longest_key = FStar.List.fold_left (fun acc (k, _) -> max acc (String.length k)) (toInt 20) points in
        let pr1 (p : (Prims.string * stat)) : Prims.string =
            let k, st = p in
            let show' n = (defaultof<showable_int> :> showable<Prims.int>).show n in
            Format.fmt5 (toString "  %s  %s %s ms %s ms %s ms"B)
                (lpad longest_key k)
                (lpad (toInt 8) (show' st.ncalls))
                (lpad (toInt 6) (show' (st.ns_tree  /. (toInt 1000000))))
                (lpad (toInt 6) (show' ((st.ns_tree -. st.ns_sub) /. (toInt 1000000))))
                (lpad (toInt 6) (show' (st.ns_exn   /. (toInt 1000000))))
        in
        Format.fmt5 
            (toString "  %s  %s %s %s %s"B) 
            (lpad longest_key (toString "key"B)) 
            (lpad (toInt 8) (toString "calls"B)) 
            (lpad (toInt 9) (toString "tree"B)) 
            (lpad (toInt 9) (toString "point"B)) 
            (lpad (toInt 9) (toString "exn"B)) ^. (toString "\n"B) ^.
            (points |> List.map pr1 |> String.concat (toString "\n"B))
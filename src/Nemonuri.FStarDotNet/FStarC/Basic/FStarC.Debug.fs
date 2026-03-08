// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Debug.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Debug.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
module String = Nemonuri.FStarDotNet.FStarC.String

module Debug =

#if false
(* State handling for this module. Used by FStarC.Options, which
is the only module that modifies the debug state. *)
val saved_state : Type0
val snapshot () : saved_state
val restore (s:saved_state) : unit

(* Enable debugging. This will make any() return true, but
does not enable any particular toggle. *)
val enable () : unit

(* Are we doing *any* kind of debugging? *)
val any () : bool

(* Print a quick message on stdout whenever debug is on. If the string
is not a constant, put this under an if to thunk it. *)
val tag (s : string) : unit

(* Obtain the toggle for a given debug key *)
val get_toggle (k : string) : ref bool

(* List all registered toggles *)
val list_all_toggles () : list string

(* Vanilla debug levels. Each level implies the previous lower one. *)
val low     () : bool
val medium  () : bool
val high    () : bool
val extreme () : bool

(* Enable a list of debug toggles. If will also call enable()
is key is non-empty, and will recognize "Low", "Medium",
"High", "Extreme" as special and call the corresponding
set_level_* function.

If any elemnt of the list starts with '-', then we disable
that toggle instead. *)
val enable_toggles (keys : list string) : unit

(* Sets the debug level to zero and sets all registered toggles
to false. any() will return false after this. *)
val disable_all () : unit

(* Nuclear option: enable ALL debug toggles. *)
val set_debug_all () : unit

(* Not used externally at the moment. *)
val set_level_low     () : unit
val set_level_medium  () : unit
val set_level_high    () : unit
val set_level_extreme () : unit
#endif


    (* Mutable state *)
    let anyref = mk_ref false
    let _debug_all : ref<bool> = mk_ref false
    let toggle_list : ref<PSMap.t<ref<bool>>> = mk_ref (PSMap.empty ())
    let dbg_level = mk_ref 0

    type saved_state = {
        toggles : list<Prims.string * bool>;
        any     : bool;
        all     : bool;
        level   : int;
    }

    let snapshot () : saved_state = {
        toggles = PSMap.fold !toggle_list (fun k r acc -> (k, !r) :: acc) [];
        any     = !anyref;
        all     = !_debug_all;
        level   = !dbg_level;
    }

    let register_toggle (k : Prims.string) : ref<bool> =
        let r = mk_ref false in
        if !_debug_all then
            r := true;
        toggle_list := PSMap.add !toggle_list k r;
        r

    let get_toggle (k : Prims.string) : ref<bool> =
        match PSMap.try_find !toggle_list k with
        | Some r -> r
        | None -> register_toggle k

    let restore (snapshot : saved_state) : unit =
        (* Set everything to false, then set all the saved ones
        to true. *)
        PSMap.iter !toggle_list (fun k r -> r := false);
        snapshot.toggles |> List.iter (fun (k, b) ->
            let r = get_toggle k in
            r := b);
        (* Also restore these references. *)
        anyref := snapshot.any;
        _debug_all := snapshot.all;
        dbg_level := snapshot.level;
        ()

    let list_all_toggles () : list<Prims.string> =
        PSMap.keys !toggle_list

    let any () = !anyref || !_debug_all

    let tag (s:Prims.string) =
        if any () then
            Format.print_string ((toString "DEBUG:"B) ^.  s ^. (toString "\n"B))

    let enable () = anyref := true

    let low     () = !dbg_level >= 1 || !_debug_all
    let medium  () = !dbg_level >= 2 || !_debug_all
    let high    () = !dbg_level >= 3 || !_debug_all
    let extreme () = !dbg_level >= 4 || !_debug_all

    let set_level_low     () = dbg_level := 1
    let set_level_medium  () = dbg_level := 2
    let set_level_high    () = dbg_level := 3
    let set_level_extreme () = dbg_level := 4

    let enable_toggles (keys : list<Prims.string>) : unit =
        if Prims.isCons keys then
            enable ();
        keys |> List.iter (fun k ->
            match k with
            | (ToString "Low"B) ->     set_level_low ()
            | (ToString "Medium"B) ->  set_level_medium ()
            | (ToString "High"B) ->    set_level_high ()
            | (ToString "Extreme"B) -> set_level_extreme ()
            | _ ->
                if String.length k >. (toInt 0) && String.get k (toInt 0) = (Core.Operators.int '-'B) then
                    let k = String.substring k (toInt 1) (String.length k -. (toInt 1)) in
                    let t = get_toggle k in
                    t := false
                else
                    let t = get_toggle k in
                    t := true
        )

    let disable_all () : unit =
        anyref := false;
        dbg_level := 0;
        PSMap.iter !toggle_list (fun k r -> r := false)

    let set_debug_all () : unit =
        _debug_all := true;
        dbg_level := 4;
        PSMap.iter !toggle_list (fun k r -> r := true)
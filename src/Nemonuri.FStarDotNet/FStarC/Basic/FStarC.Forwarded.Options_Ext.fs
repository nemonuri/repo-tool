// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.Ext.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.Ext.fst

namespace Nemonuri.FStarDotNet.FStarC.Forwarded

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.PSMap
open Nemonuri.OCamlDotNet.Forwarded


module Options_Ext =

    type key   = Prims.string
    type value = Prims.string

    type ext_state = private | E of _map : psmap<Prims.string>
        with
            member this.map = match this with | E v -> v
        end

    (* Default extension options *)
    let private defaults = [
        ((toString "context_pruning"B), (toString "true"B));
        ((toString "prune_decls"B), (toString "true"B))
    ]

    let init : ext_state =
        E <| List.fold_right (fun (k,v) m -> psmap_add m k v)
                defaults
                (psmap_empty ())
    
#if false
    (* For boolean-like options, return true if enabled. We consider an
    extension enabled when set to a non-empty string what is NOT "off",
    "false", or "0" (and this comparison is case-insensitive). *)
    val enabled (k:key) : bool

    (* Get a list of all KV pairs that "begin" with k, considered
    as a namespace. *)
    val getns (ns:string) : list (key & value)

    (* List all pairs *)
    val all () : list (key & value)

    val save () : ext_state
    val restore (s:ext_state) : unit

    val reset () : unit
#endif

    let private cur_state = alloc init

    (* Set a key-value pair in the map *)
    /// val set (k:key) (v:value) : unit
    let set (k:key) (v:value) : unit =
        cur_state := E (psmap_add (!cur_state).map k v)
    
    (* Get the value from the map, or return "" if not there *)
    /// val get (k:key) : value
    let get (k:key) : value =
        let r = 
            match psmap_try_find (!cur_state).map k with
            | None -> (toString ""B)
            | Some v -> v
        in
        r
    
    let enabled (k:key) : bool =
        let v = get k in
        let v = FStarC.String.lowercase v in
        v <> (toString ""B) && not (v = (toString "off"B) || v = (toString "false"B) || v = (toString "0"B))
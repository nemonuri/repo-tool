// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/data/FStarC.PSMap.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_PSMap.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives

module PSMap =

    type private StringMap = Nemonuri.OCamlDotNet.Forwarded.Map.S<Prims.string>
    exception Found

    (* persistent (pure) string map *)

    /// type t 'value
    type t<'value> = Nemonuri.OCamlDotNet.Forwarded.Map.t<Prims.string,'value>

    /// val empty: unit -> t 'value // GH-1161
    let empty() : t<'value> = StringMap.empty<'value>()

    /// val add: t 'value -> string -> 'value -> t 'value
    let add (map: t<'value>) (key: Prims.string) (value: 'value) = StringMap.add key value map

    /// val find_default: t 'value -> string -> 'value -> 'value
    let find_default (map: t<'value>) (key: Prims.string) (dflt: 'value) =
        match StringMap.find_opt key map with
        | None -> dflt
        | Some v -> v

    /// val of_list : list (string & 'value) -> t 'value
    let of_list (l: (Prims.string * 'value) list) = StringMap.of_list l

    /// val try_find: t 'value -> string -> option 'value
    let try_find (map: t<'value>) key = StringMap.find_opt key map

    /// val fold: t 'value -> (string -> 'value -> 'a -> 'a) -> 'a -> 'a
    let fold (m: t<'value>) f (init: 'a) = StringMap.fold<'value, 'a> f m init

    /// val find_map: t 'value -> (string -> 'value -> option 'a) -> option 'a
    let find_map (m: t<'value>) f =
        let res = ref None in
        let upd k v =
            let r = f k v in
            if r <> None then ((Effect.(:=) res r); raise Found) in
        (try StringMap.iter upd m with Found -> ());
        (Effect.(!) res)

    /// val modify: t 'value -> string -> (option 'value -> 'value) -> t 'value
    let modify (m: t<'value>) (k: Prims.string) (upd: 'value option -> 'value) =
        StringMap.update k (fun vopt -> Some (upd vopt)) m
    
    /// val merge: t 'value -> t 'value -> t 'value
    let merge (m1: t<'value>) (m2: t<'value>) : t<'value> =
        fold m1 (fun k v m -> add m k v) m2

    //val remove: t 'value -> string -> t 'value
    let remove (m: t<'value>) (key: Prims.string) : t<'value> = StringMap.remove key m

    //val keys : t 'value -> list string
    let keys (m: t<'value>) = fold m (fun k _ acc -> k::acc) []

    //val iter : t 'value -> (string -> 'value -> unit) -> unit
    let iter (m:t<'value>) f = StringMap.iter f m

    (* Aliases. We use inline_for_extraction so we don't have to define
    these in the underlying ML file. *)
    [<inline_for_extraction>] 
    type psmap<'a> = t<'a>

    [<inline_for_extraction>] 
    let inline psmap_empty () = empty ()

    [<inline_for_extraction>] 
    let inline psmap_add m k v = add m k v

    [<inline_for_extraction>] 
    let inline psmap_find_default m k v = find_default m k v

    [<inline_for_extraction>] 
    let inline psmap_try_find m k = try_find m k

    [<inline_for_extraction>] 
    let inline psmap_fold m f a = fold m f a

    [<inline_for_extraction>] 
    let inline psmap_find_map m f = find_map m f

    [<inline_for_extraction>] 
    let inline psmap_modify m k f = modify m k f

    [<inline_for_extraction>] 
    let inline psmap_merge m1 m2= merge m1 m2

    [<inline_for_extraction>] 
    let inline psmap_remove m k = remove m k

    [<inline_for_extraction>] 
    let inline psmap_iter m f = iter m f


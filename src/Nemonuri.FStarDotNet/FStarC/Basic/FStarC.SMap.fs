// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/data/FStarC.SMap.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.OCamlDotNet.Zarith

module SMap =

#if false

(* mutable string map *)

type t 'value
val create : int -> t 'value
val clear : t 'value -> unit
val add : t 'value -> string -> 'value -> unit
val of_list : list (string & 'value) -> t 'value
val try_find : t 'value -> string -> option 'value
val fold : t 'value -> (string -> 'value -> 'a -> 'a) -> 'a -> 'a
val remove : t 'value -> string -> unit
(* The list may contain duplicates. *)
val keys : t 'value -> list string
val copy : t 'value -> t 'value
val size : t 'value -> int
val iter : t 'value -> (string -> 'value -> unit) -> unit

(* Aliases. We use inline_for_extraction so we don't have to define
these in the underlying ML file. *)
inline_for_extraction type smap = t
inline_for_extraction let smap_create u = create u
inline_for_extraction let smap_clear m = clear m
inline_for_extraction let smap_add m k v = add m k v
inline_for_extraction let smap_of_list l = of_list l
inline_for_extraction let smap_try_find m k = try_find m k
inline_for_extraction let smap_fold m f a = fold m f a
inline_for_extraction let smap_remove m k = remove m k
inline_for_extraction let smap_keys m = keys m
inline_for_extraction let smap_copy m = copy m
inline_for_extraction let smap_size m = size m
inline_for_extraction let smap_iter m f = iter m f

#endif

    type private StringHashtbl = Nemonuri.OCamlDotNet.Forwarded.Hashtbl.S<Prims.string>

    type t<'value> = Nemonuri.OCamlDotNet.Forwarded.Hashtbl.t<Prims.string,'value>

    let create (i:Z.t) : 'value t = StringHashtbl.create (Z.to_int i)
    let clear (s:('value t)) = StringHashtbl.clear s
    let add (m:'value t) k (v:'value) = StringHashtbl.replace m k v
    let of_list (l: (Prims.string * 'value) list) =
        let s = StringHashtbl.create (List.length l) in
        FStar.List.iter (fun (x,y) -> add s x y) l;
        s
    let try_find (m:'value t) k = StringHashtbl.find_opt m k
    let fold (m:'value t) f a = StringHashtbl.fold f m a
    let remove (m:'value t) k = StringHashtbl.remove m k
    let keys (m:'value t) = fold m (fun k _ acc -> k::acc) []
    let copy (m:'value t) = StringHashtbl.copy m
    let size (m:'value t) = Z.of_int (StringHashtbl.length m)
    let iter (m:'value t) f = StringHashtbl.iter f m
    
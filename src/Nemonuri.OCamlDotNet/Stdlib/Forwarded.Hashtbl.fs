namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.OCamlDotNet.Primitives
open System.Collections.Generic
open Internal.Utilities.Collections

/// reference: https://ocaml.org/manual/5.4/api/Hashtbl.html
module Hashtbl =

    type t<'a,'b when 'a : not null> = internal { Value: HashMultiMap<'a,'b> }

    let private mnd<'a,'b when 'a : not null> = TargetToSourceMonad<HashMultiMap<'a,'b>, t<'a,'b>>((fun v -> { Value = v }), (fun v -> v.Value))

    let create (n: OCamlInt) : t<'a,'b> = mnd { return HashMultiMap<'a,'b>(n, EqualityComparer<'a>.Default) }

    let clear (tbl: t<'a,'b>) = mnd { let! tbl' = tbl in return! tbl'.Clear() }

    let reset (tbl: t<'a,'b>) = clear tbl (* not supported *)

    let copy (tbl: t<'a,'b>) = mnd { let! tbl' = tbl in return tbl'.Copy() }

    let add (tbl: t<'a,'b>) (key: 'a) (data: 'b) = mnd { let! tbl' = tbl in return! tbl'.Add(key, data) }

    let find_opt (tbl: t<'a,'b>) (x: 'a) : 'b option = mnd { let! tbl' = tbl in return! tbl'.TryFind(x) }

    let find (tbl: t<'a,'b>) (x: 'a) : 'b =
        match find_opt tbl x with
        | Some b -> b
        | None -> raise Not_found
    
    let find_all (tbl: t<'a,'b>) (x: 'a) = mnd { let! tbl' = tbl in return! tbl'.FindAll(x) }

    let mem (tbl: t<'a,'b>) (x: 'a) = mnd { let! tbl' = tbl in return! tbl'.ContainsKey(x) }

    let remove (tbl: t<'a,'b>) (x: 'a) = mnd { let! tbl' = tbl in return! tbl'.Remove(x) }

    let replace (tbl: t<'a,'b>) (key: 'a) (data: 'b) = mnd { let! tbl' = tbl in return! tbl'.Replace(key, data) }

    let iter (f: 'a -> 'b -> unit) (tbl: t<'a,'b>) = mnd { let! tbl' = tbl in return! tbl'.Iterate(f) }

    let to_seq (tbl: t<'a,'b>) : seq<'a * 'b> = mnd { let! tbl' = tbl in return! tbl' |> Seq.map (fun kv -> kv.Key, kv.Value)  }

    let filter_map_inplace (f: 'a -> 'b -> 'b option) (tbl: t<'a,'b>) = 
        to_seq tbl |> Seq.iter (fun (k, v) -> match f k v with | Some v -> replace tbl k v | None -> remove tbl k )

    let fold (f: 'a -> 'b -> 'acc -> 'acc) (tbl: t<'a,'b>) (init: 'acc) : 'acc = mnd { let! tbl' = tbl in return! tbl'.Fold f init }

    let length (tbl: t<'a,'b>) = mnd { let! tbl' = tbl in return! tbl'.Count }

    let to_seq_keys (tbl: t<'a,'b>) : seq<'a> = Seq.map fst (to_seq tbl)

    [<AbstractClass; Sealed>]
    type S<'key when 'key : not null> =

        static member create n : t<'key,'a> = create n

        static member clear (tbl: t<'key,'a>) = clear tbl

        static member reset (tbl: t<'key,'a>) = reset tbl

        static member add (tbl: t<'key,'a>) key data = add tbl key data

        static member remove (tbl: t<'key,'a>) key = remove tbl key

        static member replace (tbl: t<'key,'a>) key data = replace tbl key data

        static member find_opt (tbl: t<'key,'a>) x = find_opt tbl x

        static member fold (f: 'key -> 'a -> 'acc -> 'acc) (tbl: t<'key,'a>) init = fold f tbl init

        static member copy (tbl: t<'key,'a>) = copy tbl

        static member length (tbl: t<'key,'a>) = length tbl

        static member iter (f: 'key -> 'a -> unit) (tbl: t<'key,'a>) = iter f tbl

        


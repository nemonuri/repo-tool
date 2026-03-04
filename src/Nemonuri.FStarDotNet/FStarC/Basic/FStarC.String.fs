// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.String.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_String.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Batteries
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.FStarOperators
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses
module S = Nemonuri.OCamlDotNet.Forwarded.String


module String =

    (* The name of this file is misleading: most string functions are to be found in
    util.fsi *)

    /// val make:    int -> char -> string
    let make (i: Prims.int) (c: Char.char) : Prims.string = Fu.monad { let! ti = i in let! tc = c in return BatUTF8.init (Z.to_int ti) (fun _ -> BatUChar.chr tc ) }

    /// val strcat:  string -> string -> Tot string
    let strcat (s: Prims.string) (t: Prims.string) : Prims.string = Prims.op_Hat s t

    let private batstring_nsplit s t = 
        match s =. stringO ""B with 
        | BTrue -> Nil 
        | BFalse -> 
            Fu.monad { let! s' = s in let! t' = t in return List.map Fu.pur (BatString.split_on_string t' s') }

    /// val split:   chars: list char -> s: string -> Tot (list string)
    let split (seps: Prims.list<Char.char>) (s: Prims.string) : Prims.list<Prims.string> =
        let rec repeat_split acc = function
            | Nil -> acc
            | Cons(sep,seps) ->
                let usep = Fu.monad { let! sep' = sep in return BatUTF8.init 1 (fun _ -> BatUChar.chr sep') } in
                let l = FStar.List.concatMap (fun x -> batstring_nsplit x usep) acc in
                repeat_split l seps in
        repeat_split (s >:: Nil) seps
    
    /// val concat:  separator: string -> strings: list string -> Tot string
    let concat (separator: Prims.string) (strings: Prims.list<Prims.string>) : Prims.string =
        Fu.monad {
            let! separator' = separator in 
            let! strings' = strings in
            let strings'' = List.map Fu.extract strings' in
            return S.concat separator' strings''
        }

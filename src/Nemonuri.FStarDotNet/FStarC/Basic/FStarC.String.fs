// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.String.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_String.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarC
open Nemonuri.OCamlDotNet.Batteries
open Nemonuri.OCamlDotNet.Zarith
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses
module S = Nemonuri.OCamlDotNet.Forwarded.String


module String =

    (* The name of this file is misleading: most string functions are to be found in
    util.fsi *)

    /// val make:    int -> char -> string
    let make (i: Prims.int) (c: Char.char) : Prims.string = Fu.monad { let! ti = i in let! tc = c in return BatUTF8.init (Z.to_int ti) (fun _ -> BatUChar.chr tc ) }

    /// val strcat:  string -> string -> Tot string
    let strcat (s: Prims.string) (t: Prims.string) : Prims.string = Prims.op_Hat s t

    let private batstring_nsplit s t = if s = S.empty then [] else BatString.split_on_string t s

    /// val split:   chars: list char -> s: string -> Tot (list string)
    let split (chars: Prims.list<Char.char>) (s: Prims.string) : Prims.list<Prims.string> =
        let rec repeat_split acc = function
            | [] -> acc
            | sep::seps ->
            let usep = BatUTF8.init 1 (fun _ -> BatUChar.chr sep) in
            let l = List.collect (fun x -> batstring_nsplit x usep) acc in
            repeat_split l seps in
        Fu.monad { 
            let! chars' = chars in 
            let! s' = s in  
            let chars'' = List.map Fu.extract chars' in
            let result = repeat_split [s'] chars'' in
            return List.map Fu.pur result 
        } // 비효율적이야....'rec' 에서 문제가 생기네... 'List' 안에서는, 본래 타입으로 돌아가게 할까? 일단 더 지켜보고.
    
    /// val concat:  separator: string -> strings: list string -> Tot string
    let concat (separator: Prims.string) (strings: Prims.list<Prims.string>) : Prims.string =
        Fu.monad {
            let! separator' = separator in let! strings' = strings in
            let strings'' = List.map Fu.extract strings' in
            return S.concat separator' strings''
        }

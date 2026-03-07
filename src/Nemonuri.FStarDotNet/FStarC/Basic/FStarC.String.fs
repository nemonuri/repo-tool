// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.String.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_String.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Batteries
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.FStarOperators
module S = Nemonuri.OCamlDotNet.Forwarded.String
module List = Nemonuri.OCamlDotNet.Forwarded.List

module String =

    (* The name of this file is misleading: most string functions are to be found in
    util.fsi *)

    /// val make:    int -> char -> string
    let make (i: Prims.int) (c: Char.char) : Prims.string = BatUTF8.init (Z.to_int i) (fun _ -> BatUChar.chr c )

    /// val strcat:  string -> string -> Tot string
    let strcat (s: Prims.string) (t: Prims.string) : Prims.string = Prims.op_Hat s t

    let batstring_nsplit s t =
        if s =. (toString ""B) then [] else BatString.split_on_string t s

    /// val split:   chars: list char -> s: string -> Tot (list string)
    let split seps s =
        let rec repeat_split acc = function
            | [] -> acc
            | sep::seps ->
                let usep = BatUTF8.init 1 (fun _ -> BatUChar.chr sep) in
                let l = List.flatten (List.map (fun x -> batstring_nsplit x usep) acc)  in
                repeat_split l seps in
        repeat_split [s] seps
    
    /// val concat:  separator: string -> strings: list string -> Tot string
    let concat sep ls = S.concat sep ls

    (* Negative if s1<s2, zero if equal, positive if s1>s2 *)
    /// val compare: s1: string -> s2: string -> Tot int
    let compare x y = Z.of_int (S.compare x y)

    /// val strlen:  string -> Tot nat
    let strlen s = Z.of_int (BatUTF8.length s)
    
    /// val length:  string -> Tot nat
    let length s = strlen s
    
    /// val lowercase: string -> Tot string
    let lowercase s = S.lowercase_ascii s
    
    /// val uppercase: string -> Tot string
    let uppercase s = S.uppercase_ascii s

    /// val escaped: string -> Tot string
    let escaped s = S.escaped s

    /// val string_of_char : char -> Tot string
    let string_of_char (c: Char.char) = BatString.of_char (Nemonuri.OCamlDotNet.Forwarded.Char.chr c)

    (* may fail with index out of bounds *)
    /// val substring: string -> start:int -> len:int -> string
    let substring s i j =
        BatUTF8.init (Z.to_int j) (fun k -> BatUTF8.get s (k + Z.to_int i))

    /// val get: string -> int -> char
    let get s i = BatUChar.code (BatUTF8.get s (Z.to_int i))

    /// val collect: (char -> string) -> string -> string
    let collect f s =
        let r = ref (toString ""B) in
        BatUTF8.iter (fun c -> Effect.(:=) r ((Effect.(!) r) ^. f (BatUChar.code c))) s; (Effect.(!) r)

    exception Found of int
    /// val index_of: string -> char -> int
    let index_of s c =
        let c = BatUChar.chr c in
        try let _ = BatUTF8.iteri (fun c' i -> if c = c' then raise (Found i) else ()) s in Z.of_int (-1)
        with Found i -> Z.of_int i

    /// val index: string -> int -> char
    let index s i = get s i

    /// val list_of_string : string -> list char
    let list_of_string s = List.init (BatUTF8.length s) (fun i -> BatUChar.code (BatUTF8.get s i))

    /// val string_of_list: list char -> string
    let string_of_list l = BatUTF8.init (List.length l) (fun i -> BatUChar.chr (l[i]))

    /// val (^) : string -> string -> string
    let (^) s t = strcat s t

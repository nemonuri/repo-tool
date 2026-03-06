// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Getopt.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Getopt.ml

#nowarn "25" // Incomplete pattern matches on this expression. For example, the value 'Empty' may indicate a case not covered by the pattern(s).

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.OCamlDotNet.Forwarded.Array.Operators
open Nemonuri.OCamlDotNet.Primitives.Operations
module Obs = Nemonuri.OCamlDotNet.Primitives.Operations.OCamlByteSpanSources
module Ef = Nemonuri.FStarDotNet.FStarC.Effect
module OCamlStrings = Nemonuri.OCamlDotNet.Forwarded.String

module Getopt =

    let noshort : Char.char = 0
    let nolong : Prims.string = (toString ""B)
    type opt_variant<'a> =
    | ZeroArgs of (unit -> 'a)
    | OneArg of (Prims.string -> 'a) * Prims.string

    type opt'<'a> = Char.char * Prims.string * opt_variant<'a>
    type opt = opt'<unit>

    type parse_cmdline_res =
    | Empty
    | Error of (Prims.string * Prims.string) // second arg is the long name of the failed option
    | Success

    let bind l f =
        match l with
        | Error _ -> l
        | Success -> f ()
        (* | Empty  *)
        (* ^ Empty does not occur internally. *)
    
    (* Returns None if this wasn't an option arg (did not start with "-")
    * Otherwise, returns Some (o, s) where [s] is the trimmed option, and [o]
    * is the opt we found in specs (possibly None if not present, which should
    * trigger an error) *)
    let find_matching_opt specs s : (opt option * Prims.string) option =
        if OCamlStrings.length s < 2 then
            None
        else if OCamlStrings.sub s 0 2 = (toString "--"B) then
            (* long opts *)
            let strim = OCamlStrings.sub s 2 ((OCamlStrings.length s) - 2) in
            let o = FStar.List.tryFind (fun (_, option, _) -> option = strim) specs in
            Some (o, strim)
        else if OCamlStrings.sub s 0 1 = (toString "-"B) then
            (* short opts *)
            let strim = OCamlStrings.sub s 1 ((OCamlStrings.length s) - 1) in
            let o = FStar.List.tryFind (fun (shortoption, _, _) -> FStarC.String.make Z.one shortoption = strim) specs in
            Some (o, strim)
        else
            None

    (* remark: doesn't work with files starting with -- *)
    let rec parse (opts:opt list) def ar ix max i : parse_cmdline_res =
        if ix > max then Success
        else
            let arg = ((.()) ar ix) in
            let go_on () = bind (def arg) (fun _ -> parse opts def ar (ix + 1) max (i + 1)) in
            match find_matching_opt opts arg with
            | None -> go_on ()
            | Some (None, _) -> Error ((toString "unrecognized option '"B) ^. arg ^. (toString "'\n"B), arg)
            | Some (Some (_, opt, p), argtrim) ->
                begin match p with
                | ZeroArgs f -> f (); parse opts def ar (ix + 1) max (i + 1)
                | OneArg (f, name) ->
                    if ix + 1 > max
                    then Error ((toString "last option '"B) ^. argtrim ^. (toString "' takes an argument but has none\n"B), opt)
                    else
                    let r =
                        try (f ((.()) ar (ix + 1)); Success)
                        with _ -> Error ((toString "wrong argument given to option `"B) ^. argtrim ^. (toString "`\n"B), opt)
                    in bind r (fun () -> parse opts def ar (ix + 2) max (i + 1))
                end
    
    let parse_array specs others args offset =
        parse specs others args offset (Array.length args - 1) 0

    /// val parse_cmdline: list opt -> (string -> parse_cmdline_res) -> parse_cmdline_res
    let parse_cmdline specs others =
        if Array.length Sys.argv = 1 then Empty
        else parse_array specs others Sys.argv 1
    
    let private int_of_char (c: OCamlChar) = Core.Operators.int c

    /// val parse_string: list opt -> (string -> parse_cmdline_res) -> string -> parse_cmdline_res
    let parse_string specs others (str: Prims.string) =
        let split_spaces (str:Prims.string) =
            let seps = [int_of_char ' 'B; int_of_char '\t'B] in
            FStar.List.filter (fun s -> s <> (toString ""B)) (FStarC.String.split seps str)
        in
        (* to match the style of the F# code in FStar.GetOpt.fs *)
        let index_of str c =
            try
                OCamlStrings.index str c
            with Not_found -> -1
        in
        let substring_from s j =
            let len = OCamlStrings.length s - j in
            OCamlStrings.sub s j len
        in
        let rec split_quoted_fragments (str:Prims.string) =
            let i = index_of str '\''B in
            if i < 0 then Some (split_spaces str)
            else 
                let prefix = OCamlStrings.sub str 0 i in
                let suffix = substring_from str (i + 1) in
                let j = index_of suffix '\''B in
                if j < 0 then None
                else 
                    let quoted_frag = OCamlStrings.sub suffix 0 j in
                    let rest = split_quoted_fragments (substring_from suffix (j + 1)) in
                    match rest with
                    | None -> None
                    | Some rest -> Some (split_spaces prefix @ quoted_frag::rest)
        in
        match split_quoted_fragments str with
        | None -> Error((toString "Failed to parse options; unmatched quote \"'\""B), (toString ""B))
        | Some args ->
        parse_array specs others (Array.of_list args) 0

    /// val parse_list: list opt -> (string -> parse_cmdline_res) -> list string -> parse_cmdline_res
    let parse_list specs others lst =
        parse_array specs others (Array.of_list lst) 0

    /// val cmdline: unit -> list string
    let cmdline () =
        Array.to_list (Sys.argv)

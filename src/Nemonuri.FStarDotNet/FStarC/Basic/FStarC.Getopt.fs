// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Getopt.fsti
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Getopt.ml

#nowarn "25" // Incomplete pattern matches on this expression. For example, the value 'Empty' may indicate a case not covered by the pattern(s).

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.Primitives
open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.OCamlDotNet.Forwarded.Array.Operators
open Nemonuri.OCamlDotNet.Primitives.Operations
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses
module B = Nemonuri.FStarDotNet.FStarC.BaseTypes
module Obs = Nemonuri.OCamlDotNet.Primitives.Operations.OCamlByteSpanSources
module Ef = Nemonuri.FStarDotNet.FStarC.Effect
module Pn = Nemonuri.FStarDotNet.FStar.Pervasives.Native

module Getopt =

    let inline private a2s s = Obs.Unsafe.stringOfArray s

    [<RequireQualifiedAccess>]
    type Topt_variant<'a> =
    | ZeroArgs of (Prims.unit -> 'a)
    | OneArg of Ef.ML<(Prims.string -> 'a) * Prims.string>
    type opt_variant<'a> = Ef.ML<Topt_variant<'a>>

    let ZeroArgs x = Fu.pur (Topt_variant.ZeroArgs x)
    let OneArg x = Fu.pur (Topt_variant.OneArg x)
    let (|ZeroArgs|OneArg|) (Type0 x) =
        match x with
        | Topt_variant.ZeroArgs v -> ZeroArgs v
        | Topt_variant.OneArg v -> OneArg v

    type opt'<'a> = Ef.ML<B.char * Prims.string * opt_variant<'a>>
    type opt = opt'<Prims.unit>

    [<RequireQualifiedAccess>]
    type Tparse_cmdline_res =
    | Empty
    | Error of Ef.ML<Prims.string * Prims.string> // second arg is the long name of the failed option
    | Success
    type parse_cmdline_res = Ef.ML<Tparse_cmdline_res>

    let Empty = Fu.pur (Tparse_cmdline_res.Empty)
    let Error x = Fu.pur (Tparse_cmdline_res.Error x)
    let Success = Fu.pur (Tparse_cmdline_res.Success)
    let (|Empty|Error|Success|) (Type0 x) =
        match x with
        | Tparse_cmdline_res.Empty -> Empty
        | Tparse_cmdline_res.Error v -> Error v
        | Tparse_cmdline_res.Success -> Success


    let noshort: B.char = Fu.monad { return 0 }
    let nolong : Prims.string = Fu.monad { return (a2s ""B) }

    let parse_list: Prims.list<opt> -> Ef.ML<Prims.string -> parse_cmdline_res> -> Prims.list<Prims.string> -> parse_cmdline_res = raise (System.NotImplementedException())
    let cmdline: Prims.unit -> Prims.list<Prims.string> = raise (System.NotImplementedException())

   
    let bind l f =
        match l with  
        | Error _ -> l
        | Success -> f unitO
    (* | Empty  *)
    (* ^ Empty does not occur internally. *)

    (* Returns None if this wasn't an option arg (did not start with "-")
    * Otherwise, returns Some (o, s) where [s] is the trimmed option, and [o]
    * is the opt we found in specs (possibly None if not present, which should
    * trigger an error) *)
    let find_matching_opt specs s : Pn.tuple2<opt Pn.option, Prims.string> Pn.option =
        match Fu.monad { let! s' = s in return String.length s' < 2 } with
        | BTrue -> None
        | BFalse -> 
        match Fu.monad { let! s' = s in return String.sub s' 0 2 = (a2s "--"B) } with
        | BTrue ->
            let strim = Fu.monad { let! s' = s in return String.sub s' 2 ((String.length s') - 2) } in
            let o = FStar.List.tryFind (fun (Type0(_, option, _)) -> option =. strim) specs in
            Some (o .&. strim)
        | BFalse -> 
        match Fu.monad { let! s' = s in return String.sub s' 0 1 = (a2s "-"B) } with
        | BTrue ->
        (* short opts *)
            let strim = Fu.monad { let! s' = s in return String.sub s' 1 ((String.length s') - 1) } in
            let o = FStar.List.tryFind (fun (Type0(shortoption, _, _)) -> FStarC.String.make (intO 1) shortoption =. strim) specs in
            Some (o .&. strim)
        | BFalse ->
            None


    (* remark: doesn't work with files starting with -- *)
    let rec parse (opts:opt Prims.list) def ar ix max i : parse_cmdline_res =
        match Fu.monad { let! ix' = ix in let! max' = max in return ix' > max' } with
        | BTrue -> Success
        | BFalse ->
            let arg = Fu.monad { let! ar' = ar in let! ix' = ix in return (.()) ar' ix' } in
            let go_on (Type0()) = bind (def arg) (fun _ -> parse opts def ar (Fu.monad {let! ix' = ix in return ix' + 1}) max (Fu.monad {let! i' = i in return i' + 1})) in
            match find_matching_opt opts arg with
            | None -> go_on unitO
            | Some (Type0(None, _)) -> Error (stringO "unrecognized option '"B ^. arg ^. stringO "'\n"B .&. arg)
            | Some (Type0(Some (Type0(_, opt, p)), argtrim)) ->
                begin match p with
                | ZeroArgs f -> f unitO >: parse opts def ar (Fu.monad {let! ix' = ix in return ix' + 1}) max (Fu.monad {let! i' = i in return i' + 1})
                | OneArg (Type0(f, name)) ->
                    match Fu.monad { let! ix' = ix in let! max' = max in return ix' + 1 > max' } with
                    | BTrue -> Error (stringO "last option '"B ^. argtrim ^. stringO "' takes an argument but has none\n"B .&. opt)
                    | BFalse ->
                        let r =
                            try f (Fu.monad { let! ar' = ar in let! ix' = ix in return (.()) ar' (ix' + 1) }) >: Success
                            with _ -> Error (stringO "wrong argument given to option `"B ^. argtrim ^. stringO "`\n"B .&. opt)
                        in bind r (fun (Type0()) -> parse opts def ar (Fu.monad {let! ix' = ix in return ix' + 2}) max (Fu.monad {let! i' = i in return i' + 1}))
                end

    let parse_array specs others args offset =
        parse specs others args offset (Fu.monad { let! args' = args in return Array.length args' - 1}) (Fu.monad { return 0 })

    /// val parse_cmdline: list opt -> (string -> parse_cmdline_res) -> parse_cmdline_res
    let parse_cmdline specs others =
        match Fu.monad { return Array.length Sys.argv = 1 } with
        | BTrue -> Empty
        | BFalse -> parse_array specs others (Fu.monad { return Sys.argv}) (Fu.monad { return 1 })

#if false
    /// val parse_string: list opt -> (string -> parse_cmdline_res) -> string -> parse_cmdline_res
    let parse_string specs others (str:string) =
        let split_spaces (str:string) =
            let seps = [int_of_char ' '; int_of_char '\t'] in
            FStar.List.filter (fun s -> s != "") (FStar.String.split' seps str)
        in
        (* to match the style of the F# code in FStar.GetOpt.fs *)
        let index_of str c =
            try
                String.index str c
            with Not_found -> -1
        in
        let substring_from s j =
            let len = String.length s - j in
            String.sub s j len
        in
        let rec split_quoted_fragments (str:string) =
            let i = index_of str '\'' in
            if i < 0 then Some (split_spaces str)
            else let prefix = String.sub str 0 i in
                let suffix = substring_from str (i + 1) in
                let j = index_of suffix '\'' in
                if j < 0 then None
                else let quoted_frag = String.sub suffix 0 j in
                        let rest = split_quoted_fragments (substring_from suffix (j + 1)) in
                        match rest with
                        | None -> None
                        | Some rest -> Some (split_spaces prefix @ quoted_frag::rest)

        in
        match split_quoted_fragments str with
        | None -> Error("Failed to parse options; unmatched quote \"'\"", "")
        | Some args ->
            parse_array specs others (Array.of_list args) 0

  let parse_list specs others lst =
    parse_array specs others (Array.of_list lst) 0

  let cmdline () =
    Array.to_list (Sys.argv)
#endif
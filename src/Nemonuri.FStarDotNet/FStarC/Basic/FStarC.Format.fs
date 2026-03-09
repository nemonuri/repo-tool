// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Format.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Format.ml

namespace Nemonuri.FStarDotNet.FStarC

(* Formatting/printing utils *)

open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.OCamlDotNet.Forwarded.Out_channel
open Nemonuri.OCamlDotNet.Batteries
open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.Json
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ofd = Nemonuri.OCamlDotNet.Primitives.OCamlFileDescriptors

module Format =

    type printer = {
        printer_prinfo: Prims.string -> unit;
        printer_prwarning: Prims.string -> unit;
        printer_prerror: Prims.string -> unit;
        printer_prgeneric: Prims.string -> (unit -> Prims.string) -> (unit -> json) -> unit
    }

#if false
val default_printer : printer
val set_printer : printer -> unit

val print_raw : string -> unit
val print_generic: string -> ('a -> string) -> ('a -> json) -> 'a -> unit
val print_any : 'a -> unit

val print_string (s : string) : unit

val fmt
  (spec : string)
  (args : list string)
  : string

val fmt1
  (spec : string)
  (arg1 : string)
  : string

val fmt2
  (spec : string)
  (arg1 arg2 : string)
  : string

val fmt3
  (spec : string)
  (arg1 arg2 arg3 : string)
  : string

val fmt4
  (spec : string)
  (arg1 arg2 arg3 arg4 : string)
  : string

val fmt5
  (spec : string)
  (arg1 arg2 arg3 arg4 arg5 : string)
  : string

val fmt6
  (spec : string)
  (arg1 arg2 arg3 arg4 arg5 arg6 : string)
  : string

val print
  (spec : string)
  (args : list string)
  : unit

val print1
  (spec : string)
  (arg1 : string)
  : unit

val print2
  (spec : string)
  (arg1 arg2 : string)
  : unit

val print3
  (spec : string)
  (arg1 arg2 arg3 : string)
  : unit

val print4
  (spec : string)
  (arg1 arg2 arg3 arg4 : string)
  : unit

val print5
  (spec : string)
  (arg1 arg2 arg3 arg4 arg5 : string)
  : unit

val print6
  (spec : string)
  (arg1 arg2 arg3 arg4 arg5 arg6 : string)
  : unit

val print_error: string -> unit
val print1_error: string -> string -> unit
val print2_error: string -> string -> string -> unit
val print3_error: string -> string -> string -> string -> unit

val print_warning: string -> unit
val print1_warning: string -> string -> unit
val print2_warning: string -> string -> string -> unit
val print3_warning: string -> string -> string -> string -> unit

val flush_stdout () : unit

val stdout_isatty () : option bool

// These functions have no effect
val colorize : string -> (string & string) -> string
val colorize_bold : string -> string
val colorize_red : string -> string
val colorize_yellow : string -> string
val colorize_cyan : string -> string
val colorize_green : string -> string
val colorize_magenta : string -> string
#endif


    let private stdout_isatty () = Some (Unix.isatty Unix.stdout)

    (* NOTE: this is deciding whether or not to color by looking
        at stdout_isatty(), which may be a wrong choice if
        we're instead outputting to stderr. e.g.
            fstar.exe Blah.fst 2>errlog
        will colorize the errors in the file if stdout is not
        also redirected.
    *)
    let colorize (o, c) s =
        match stdout_isatty () with
        | Some true -> o ^. s ^. c
        | _ -> s

    let colorize_bold    = colorize ((toString "\x1b[39;1m"B), (toString "\x1b[0m"B))
    let colorize_red     = colorize ((toString "\x1b[31;1m"B), (toString "\x1b[0m"B))
    let colorize_yellow  = colorize ((toString "\x1b[33;1m"B), (toString "\x1b[0m"B))
    let colorize_cyan    = colorize ((toString "\x1b[36;1m"B), (toString "\x1b[0m"B))
    let colorize_green   = colorize ((toString "\x1b[32;1m"B), (toString "\x1b[0m"B))
    let colorize_magenta = colorize ((toString "\x1b[35;1m"B), (toString "\x1b[0m"B))



    let default_printer =
        {   printer_prinfo = (fun s -> output_string stdout s; flush stdout);
            printer_prwarning = (fun s -> output_string stderr (colorize_yellow s); flush stdout; flush stderr);
            printer_prerror = (fun s -> output_string stderr (colorize_red s); flush stdout; flush stderr);
            printer_prgeneric = fun label get_string get_json -> output_string stdout (label ^. (toString ": "B) ^. (get_string())) ; flush stdout; }

    let current_printer = ref default_printer
    let set_printer printer = current_printer := printer


    let print_raw s = Out_channel.set_binary_mode stdout true; output_string stdout s; flush stdout
    let print_string s = (!current_printer).printer_prinfo s
    let print_generic label to_string to_json a = (!current_printer).printer_prgeneric label (fun () -> to_string a) (fun () -> to_json a)
    let print_any s = (!current_printer).printer_prinfo (JsonSerializers.serialize s)


    (* restore pre-2.11 BatString.nsplit behavior,
        see https://github.com/ocaml-batteries-team/batteries-included/issues/845 *)
    let batstring_nsplit s t =
        if s = (toString ""B) then [] else BatString.split_on_string t s

    let fmt (fmt:Prims.string) (args:Prims.string list) =
        let frags = batstring_nsplit fmt (toString "%s"B) in
        if List.length frags <> List.length args + 1 then
            failwith 
              ((toString "Not enough arguments to format Prims.string "B) ^. 
                    fmt ^. (toString " : expected "B) ^. (Int.to_string (List.length frags)) ^. 
                    (toString " got ["B) ^. (String.concat (toString ", "B) args) ^. 
                    (toString "] frags are ["B) ^. (String.concat (toString ", "B) frags) ^. (toString "]"B))
        else
            let sbldr = StringBuffer.create (Z.of_int 80) in
            ignore (StringBuffer.add (List.hd frags) sbldr);
            List.iter2
                    (fun frag arg -> sbldr |> StringBuffer.add arg |> StringBuffer.add frag |> ignore)
                    (List.tl frags) args;
            StringBuffer.contents sbldr


    let fmt1 f a = fmt f [a]
    let fmt2 f a b = fmt f [a;b]
    let fmt3 f a b c = fmt f [a;b;c]
    let fmt4 f a b c d = fmt f [a;b;c;d]
    let fmt5 f a b c d e = fmt f [a;b;c;d;e]
    let fmt6 f a b c d e g = fmt f [a;b;c;d;e;g]

    let flush_stdout () = flush stdout

    let print1 a b = print_string (fmt1 a b)
    let print2 a b c = print_string (fmt2 a b c)
    let print3 a b c d = print_string (fmt3 a b c d)
    let print4 a b c d e = print_string (fmt4 a b c d e)
    let print5 a b c d e f = print_string (fmt5 a b c d e f)
    let print6 a b c d e f g = print_string (fmt6 a b c d e f g)
    let print f args = print_string (fmt f args)

    let print_error s = (!current_printer).printer_prerror s
    let print1_error a b = print_error (fmt1 a b)
    let print2_error a b c = print_error (fmt2 a b c)
    let print3_error a b c d = print_error (fmt3 a b c d)

    let print_warning s = (!current_printer).printer_prwarning s
    let print1_warning a b = print_warning (fmt1 a b)
    let print2_warning a b c = print_warning (fmt2 a b c)
    let print3_warning a b c d = print_warning (fmt3 a b c d)


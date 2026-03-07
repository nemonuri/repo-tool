// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
module Effect = Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.Getopt
module BaseTypes = Nemonuri.FStarDotNet.FStarC.BaseTypes
open Nemonuri.FStarDotNet.FStarC.VConfig

module Options =

    (* Set externally, checks if the directory exists and otherwise
    logs an issue. Cannot do it here due to circular deps. *)
    let check_include_dir : Effect.ref<Prims.string -> unit> = Effect.mk_ref (fun (s:Prims.string) -> ())

    (* Raised when a processing a pragma an a non-settable option
    appears there. *)
    exception NotSettable of Prims.string

    type codegen_t =
    | OCaml
    | FSharp
    | Krml
    | Plugin
    | PluginNoLib
    | Extension

    //let __test_norm_all = mk_ref false

    type split_queries_t = | No | OnFailure | Always

    type message_format_t = | Json | Human | Github

    type option_val =
    | Bool of bool
    | String of Prims.string
    | Path of Prims.string
    | Int of int
    | List of list<option_val>
    | Unset

    type optionstate = PSMap.t<option_val>

    type opt_type =
    | Const of option_val
    // --cache_checked_modules
    | IntStr of Prims.string (* label *)
    // --z3rlimit 5
    | BoolStr
    // --admit_smt_queries true
    | PathStr of Prims.string (* label *)
    // --fstar_home /build/fstar
    | SimpleStr of Prims.string (* label *)
    // --admit_except xyz
    | EnumStr of list<Prims.string>
    // --codegen OCaml
    | OpenEnumStr of list<Prims.string> (* suggested values (not exhaustive) *) * Prims.string (* label *)
    // --debug …
    | PostProcessed of ((option_val -> option_val) (* validator *) * opt_type (* elem spec *))
    // For options like --extract_module that require post-processing or validation
    | Accumulated of opt_type (* elem spec *)
    // For options like --extract_module that can be repeated (LIFO, accumulate the new element via Cons, at the head)
    | ReverseAccumulated of opt_type (* elem spec *)
    // For options like --include that can be repeated (FIFO, accumulate the new element via snoc, at the tail)
    | WithSideEffect of ((unit -> unit) * opt_type (* elem spec *))
    // For options like --version that have side effects

        
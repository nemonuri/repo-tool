// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open System.Collections.Generic
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

module Options =

    module Ef = Nemonuri.FStarDotNet.FStarC.Effect

    (* Set externally, checks if the directory exists and otherwise
    logs an issue. Cannot do it here due to circular deps. *)
    let check_include_dir: Ef.ref<Prims.string -> Prims.unit> = Ef.mk_ref ( fun s -> Fu.zero )
        
    (* Raised when a processing a pragma an a non-settable option
    appears there. *)
    exception FStar_NotSettable of Prims.string
    let NotSettable (msg: Prims.string) = Fu.monad { return FStar_NotSettable msg }
    let (|NotSettable|_|) (exn: Prims.exn) = 
        Fu.emonad { 
            match! exn with 
            | FStar_NotSettable s -> return Some s
            | _ -> return None
        }
    
    type Tcodegen_t =
    | OCaml
    | FSharp
    | Krml
    | Plugin
    | PluginNoLib
    | Extension
    type codegen_t = Ef.ML<Tcodegen_t>

    type Tsplit_queries_t = | No | OnFailure | Always
    type split_queries_t = Ef.ML<Tsplit_queries_t>

    type Tmessage_format_t = | Json | Human | Github
    type message_format_t = Ef.ML<Tmessage_format_t>

    type Toption_val =
    | Bool of Prims.bool
    | String of Prims.string
    | Path of Prims.string
    | Int of Prims.int
    | List of Prims.list<option_val>
    | Unset
    and option_val = Ef.ML<Toption_val>

    type optionstate = Ef.ML<Dictionary<Prims.string, option_val>>   //Collections.Map<Core.string, option_val>

    type Topt_type =
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
    | EnumStr of Prims.list<Prims.string>
    // --codegen OCaml
    | OpenEnumStr of Ef.ML<Prims.list<Prims.string> (* suggested values (not exhaustive) *) * Prims.string (* label *)>
    // --debug …
    | PostProcessed of Ef.ML<(option_val -> option_val) (* validator *) * opt_type (* elem spec *)>
    // For options like --extract_module that require post-processing or validation
    | Accumulated of opt_type (* elem spec *)
    // For options like --extract_module that can be repeated (LIFO, accumulate the new element via Cons, at the head)
    | ReverseAccumulated of opt_type (* elem spec *)
    // For options like --include that can be repeated (FIFO, accumulate the new element via snoc, at the tail)
    | WithSideEffect of Ef.ML<(Prims.unit -> Prims.unit) * opt_type (* elem spec *)>
    // For options like --version that have side effects
    and opt_type = Ef.ML<Topt_type>

    
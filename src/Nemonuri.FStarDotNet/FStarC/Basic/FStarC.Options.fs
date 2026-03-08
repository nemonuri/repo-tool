#nowarn "25" // Incomplete pattern matches
#nowarn "1173"
#nowarn "1174"

// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Options.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStar
open Nemonuri.FStarDotNet.FStar.Pervasives.Native
open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.PSMap
open Nemonuri.FStarDotNet.FStarC.Getopt
open Nemonuri.FStarDotNet.FStarC.BaseTypes
open Nemonuri.FStarDotNet.FStarC.VConfig
open Nemonuri.FStarDotNet.FStarC.String
open Nemonuri.FStarDotNet.FStarC.Util
open Nemonuri.FStarDotNet.FStarC.Class.Deq
open Nemonuri.FStarDotNet.FStarC.Class.Show
open Microsoft.FSharp.Core.Operators.Unchecked

module Options =

    module Ext =

        open Nemonuri.FStarDotNet.FStarC.Forwarded

        type key = Options_Ext.key
        type value = Options_Ext.value

        type ext_state = Options_Ext.ext_state

        let set k v = Options_Ext.set k v

        let get k = Options_Ext.get k

        let enabled k = Options_Ext.enabled k

        let getns ns = Options_Ext.getns ns

        let all () = Options_Ext.all ()

        let save () = Options_Ext.save()

        let restore s = Options_Ext.restore s

        let reset () = Options_Ext.reset ()



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
    | Int of Prims.int
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

#if false
val defaults                    : list (string & option_val)

val init                        : unit    -> unit  //sets the current options to their defaults
val clear                       : unit    -> unit  //wipes the stack of options, and then inits
val restore_cmd_line_options    : bool -> parse_cmdline_res //inits or clears (if the flag is set) the current options and then sets it to the cmd line
val with_restored_cmd_line_options : (unit -> 'a) -> 'a
(* Control the option stack *)
(* Briefly, push/pop are used by the interactive mode and internal_*
 * by #push-options/#pop-options. Read the comment in the .fs for more
 * details. *)
val push                        : unit -> unit
val pop                         : unit -> unit
val internal_push               : unit -> unit
val internal_pop                : unit -> bool (* returns whether it worked or not, false should be taken as a hard error *)
val depth                       : unit -> int (* number of elements in internal option stack, besides current. If >0, internal_pop should succeed. *)
val snapshot                    : unit -> (int & unit)
val rollback                    : option int -> unit
val peek                        : unit -> optionstate
val set                         : optionstate -> unit
val set_verification_options    : optionstate -> unit

(* Print the current optionstate as a string that could be passed to fstar.exe, e.g.
"--z3rlimit 25 --include /some/path" *)
val show_options                : unit -> string

val parse_cmd_line              : unit    -> parse_cmdline_res & list string
val add_verify_module           : string  -> unit

val set_option_warning_callback : (string -> unit) -> unit
val desc_of_opt_type            : opt_type -> option string
val all_specs_with_types        : list (char & string & opt_type & Pprint.document)
val help_for_option             : string -> option Pprint.document
val settable                    : string -> bool

val abort_counter : ref int

val admit_smt_queries           : unit    -> bool
val set_admit_smt_queries       : bool    -> unit
val admit_except                : unit    -> option string
val compat_pre_core_should_register : unit    -> bool
val compat_pre_core_should_check : unit    -> bool
val compat_pre_core_set         : unit    -> bool
val compat_pre_typed_indexed_effects: unit -> bool
val disallow_unification_guards : unit    -> bool
val cache_checked_modules       : unit    -> bool
val cache_off                   : unit    -> bool
val print_cache_version         : unit    -> bool
val cmi                         : unit    -> bool
val codegen                     : unit    -> option codegen_t
val parse_codegen               : string  -> option codegen_t
val codegen_libs                : unit    -> list (list string)
val profile_enabled             : module_name:option string -> profile_phase:string -> bool
val profile_group_by_decl       : unit    -> bool
val defensive                   : unit    -> bool // true if checks should be performed
val defensive_error             : unit    -> bool // true if "error"
val defensive_abort             : unit    -> bool // true if "abort"
val dep                         : unit    -> option string
val detail_errors               : unit    -> bool
val detail_hint_replay          : unit    -> bool
val display_usage               : unit    -> unit
val any_dump_module             : unit    -> bool
val dump_ast                    : unit    -> bool
val dump_module                 : string  -> bool
val eager_subtyping             : unit    -> bool
val error_contexts              : unit    -> bool
val expose_interfaces           : unit    -> bool
val message_format              : unit    -> message_format_t
val file_list                   : unit    -> list string
val force                       : unit    -> bool
val fstar_bin_directory         : string
val get_option                  : string  -> option_val
val help                        : unit    -> bool
val hide_uvar_nums              : unit    -> bool
val hint_info                   : unit    -> bool
val hint_file_for_src           : string  -> string
val ide                         : unit    -> bool
val ide_id_info_off             : unit    -> bool
val set_ide_filename            : string -> unit
val ide_filename                : unit -> option string
val print                       : unit    -> bool
val print_in_place              : unit    -> bool
val initial_fuel                : unit    -> int
val initial_ifuel               : unit    -> int
val interactive                 : unit    -> bool
val keep_query_captions         : unit    -> bool
val lang_extensions             : unit    -> list string
val lax                         : unit    -> bool
val load                        : unit    -> list string
val load_cmxs                   : unit    -> list string
val log_queries                 : unit    -> bool
val log_failing_queries         : unit    -> bool
val log_types                   : unit    -> bool
val max_fuel                    : unit    -> int
val max_ifuel                   : unit    -> int
val ml_ish                      : unit    -> bool
val ml_ish_effect               : unit    -> string
val set_ml_ish                  : unit    -> unit
val no_location_info            : unit    -> bool
val no_prelude                  : unit    -> bool
val no_plugins                  : unit    -> bool
val no_smt                      : unit    -> bool
val normalize_pure_terms_for_extraction
                                : unit    -> bool
val output_to                   : unit    -> option string
val krmloutput                  : unit    -> option string
val list_plugins                : unit    -> bool
val expand_include              : unit    -> option string
val locate                      : unit    -> bool
val locate_lib                  : unit    -> bool
val locate_ocaml                : unit    -> bool
val locate_file                 : unit    -> option string
val locate_z3                   : unit    -> option string
val output_deps_to              : unit    -> option string
val custom_prims                : unit    -> option string
val print_bound_var_types       : unit    -> bool
val print_effect_args           : unit    -> bool
val print_expected_failures     : unit    -> bool
val print_implicits             : unit    -> bool
val print_real_names            : unit    -> bool
val print_universes             : unit    -> bool
val print_z3_statistics         : unit    -> bool
val proof_recovery              : unit    -> bool
val quake_lo                    : unit    -> int
val quake_hi                    : unit    -> int
val quake_keep                  : unit    -> bool
val query_cache                 : unit    -> bool
val query_stats                 : unit    -> bool
val read_checked_file           : unit    -> option string
val read_krml_file              : unit    -> option string
val record_hints                : unit    -> bool
val record_options              : unit    -> bool
val retry                       : unit    -> bool
val reuse_hint_for              : unit    -> option string
val report_assumes              : unit    -> option string
val set_option                  : string  -> option_val -> unit
val set_options                 : string -> parse_cmdline_res
val should_be_already_cached    : string  -> bool
val should_print_message        : string  -> bool
val should_extract              : string  -> codegen_t -> bool
val should_check                : string  -> bool (* Should check this module, lax or not. *)
val should_check_file           : string  -> bool (* Should check this file, lax or not. *)
val should_verify               : string  -> bool (* Should check this module with verification enabled. *)
val should_verify_file          : string  -> bool (* Should check this file with verification enabled. *)
val silent                      : unit    -> bool
val smt                         : unit    -> option string
val smtencoding_elim_box        : unit    -> bool
val smtencoding_nl_arith_default: unit    -> bool
val smtencoding_nl_arith_wrapped: unit    -> bool
val smtencoding_nl_arith_native : unit    -> bool
val smtencoding_l_arith_default : unit    -> bool
val smtencoding_l_arith_native  : unit    -> bool
val smtencoding_valid_intro     : unit    -> bool
val smtencoding_valid_elim      : unit    -> bool
val split_queries               : unit    -> split_queries_t
val stats                       : unit    -> bool
val tactic_raw_binders          : unit    -> bool
val tactics_failhard            : unit    -> bool
val tactics_info                : unit    -> bool
val tactic_trace                : unit    -> bool
val tactic_trace_d              : unit    -> int
val tactics_nbe                 : unit    -> bool
val tcnorm                      : unit    -> bool
val timing                      : unit    -> bool
val trace_error                 : unit    -> bool
val ugly                        : unit    -> bool
val unthrottle_inductives       : unit    -> bool
val unsafe_tactic_exec          : unit    -> bool
val use_eq_at_higher_order      : unit    -> bool
val use_hints                   : unit    -> bool
val use_hint_hashes             : unit    -> bool
val use_native_tactics          : unit    -> option string
val use_tactics                 : unit    -> bool
val using_facts_from            : unit    -> list (list string & bool)
val warn_default_effects        : unit    -> bool
val with_saved_options          : (unit -> 'a) -> 'a
val with_options                : string -> (unit -> 'a) -> 'a
val z3_cliopt                   : unit    -> list string
val z3_smtopt                   : unit    -> list string
val z3_refresh                  : unit    -> bool
val z3_rlimit                   : unit    -> int
val z3_rlimit_factor            : unit    -> int
val z3_seed                     : unit    -> int
val z3_version                  : unit    -> string
val no_positivity               : unit    -> bool
val warn_error                  : unit    -> string
val set_error_flags_callback    : ((unit  -> parse_cmdline_res) -> unit)
val use_nbe                     : unit    -> bool
val use_nbe_for_extraction      : unit    -> bool
val trivial_pre_for_unannotated_effectful_fns
                                : unit    -> bool

(* List of enabled debug toggles. *)
val debug_keys                  : unit    -> list string

(* Whether we are debugging every module and not just the ones
in the cmdline. *)
val debug_all_modules           : unit    -> bool

// HACK ALERT! This is to ensure we have no dependency from Options to Version,
// otherwise, since Version is regenerated all the time, this invalidates the
// whole build tree. A classy technique I learned from the OCaml compiler.
val _version: ref string
val _platform: ref string
val _compiler: ref string
val _date: ref string
val _commit: ref string

val debug_embedding: ref bool
val eager_embedding: ref bool

val get_vconfig : unit -> vconfig
val set_vconfig : vconfig -> unit
#endif

    let private debug_embedding = mk_ref false
    let private eager_embedding = mk_ref false

    let private as_bool = function
        | Bool b -> b
        | _ -> failwith     (toString "Impos: expected Bool"B)
    let private as_int = function
        | Int b -> b
        | _ -> failwith     (toString "Impos: expected Int"B)
    let private as_string = function
        | String b -> b
        | Path b -> b
        | _ -> failwith     (toString "Impos: expected String"B)
    let private as_list' = function
        | List ts -> ts
        | _ -> failwith     (toString "Impos: expected List"B)
    let private as_list as_t x =
        as_list' x |> List.map as_t
    let private as_option as_t = function
        | Unset -> None
        | v -> Some (as_t v)
    let private as_comma_string_list = function
        | List ls -> List.flatten <| List.map (fun l -> split (as_string l) (toString ","B)) ls
        | _ -> failwith     (toString "Impos: expected String (comma list)"B)
    
    (* The option state is a stack of stacks. Why? First, we need to
    * support #push-options and #pop-options, which provide the user with
    * a stack-like option control, useful for rlimits and whatnot. Second,
    * there's the interactive mode, which allows to traverse a file and
    * backtrack over it, and must update this state accordingly. So for
    * instance consider the following code:
    *
    *   1. #push-options "A"
    *   2. let f = ...
    *   3. #pop-options
    *
    * Running in batch mode starts with a singleton stack, then pushes,
    * then pops. In the interactive mode, say we go over line 1. Then
    * our current state is a stack with two elements (original state and
    * state+"A"), but we need the previous state too to backtrack if we run
    * C-c C-p or whatever. We can also go over line 3, and still we need
    * to keep track of everything to backtrack. After processing the lines
    * one-by-one in the interactive mode, the stacks are: (top at the head)
    *
    *      (orig)
    *      (orig + A) (orig)
    *      (orig)
    *
    * No stack should ever be empty! Any of these failwiths should never be
    * triggered externally. IOW, the API should protect this invariant.
    *
    * We also keep a snapshot of the stateful modules that are modified by
    * changing options. Currently: Debug, Ext (extension options) and Stats.
    *)
    type history1 =
        Debug.saved_state *
        Ext.ext_state *
        bool * (* value of Stats.enabled *)
        optionstate

    let fstar_options : ref<optionstate> = mk_ref (PSMap.empty ())

    let snapshot_all () : history1 =
        (Debug.snapshot (), Ext.save (), !Stats.enabled, !fstar_options)

    let restore_all (h : history1) : unit =
        let dbg, ext, stats, opts = h in
        Debug.restore dbg;
        Ext.restore ext;
        Stats.enabled := stats;
        fstar_options := opts


    let history : ref<list<list<history1>>> =
        mk_ref [] // IRRELEVANT: see clear() below

    let peek () = !fstar_options

    let internal_push () =
        let lev1::rest = !history in
        let newhd = snapshot_all () in
        history := (newhd :: lev1) :: rest

    let internal_pop () =
        let lev1::rest = !history in
        match lev1 with
        | [] -> false
        | snap :: lev1' ->
            restore_all snap;
            history := lev1' :: rest;
            true

    let push () = // already signal-atomic
        (* This turns a stack like

                    4
                    3
                    2 1      current:5
        into:
                    5
                4 4
                3 3
                2 2 1      current:5

        i.e.  current state does not change, and
        current minor stack does not change. The
        "next" previous stack (now with 2,3,4,5)
        has a copy of 5 at the top so we can restore regardless
        of what we do in the current stack or the current state. *)

        internal_push ();
        let lev1::_ = !history in
        history := lev1 :: !history;
        ignore (internal_pop());
        ()

    let pop () = // already signal-atomic
        match !history with
        | [] -> failwith (toString "TOO MANY POPS!"B)
        | _::levs ->
            history := levs;
            if not (internal_pop ()) then
                failwith (toString "aaa!!!"B)

    let set o =
        fstar_options := o

    let depth () =
        let lev::_ = !history in
        List.length lev

    let snapshot ()    = Common.snapshot (toString "Options"B) push history ()
    let rollback depth = Common.rollback (toString "Options"B) pop  history depth

    let set_option k v =
        let map : optionstate = peek() in
        if k = (toString "report_assumes"B)
        then 
            match psmap_try_find map k with
            | Some (String (ToString "error"B)) ->
                //It's already set to error; ignore any attempt to change it
                ()
            | _ -> fstar_options := psmap_add map k v
        else fstar_options := psmap_add map k v

    let set_option' (k,v) =  set_option k v
    let set_admit_smt_queries (b:bool) = set_option (toString "admit_smt_queries"B) (Bool b)


    let private defaults = [
        ((toString "abort_on"B)                                  , Int (toInt 0));
        ((toString "admit_except"B)                              , Unset);
        ((toString "admit_smt_queries"B)                         , Bool false);
        ((toString "already_cached"B)                            , Unset);
        ((toString "cache_checked_modules"B)                     , Bool false);
        ((toString "cache_off"B)                                 , Bool false);
        ((toString "cmi"B)                                       , Bool false);
        ((toString "codegen-lib"B)                               , List []);
        ((toString "codegen"B)                                   , Unset);
        ((toString "compat_pre_core"B)                           , Unset);
        ((toString "compat_pre_typed_indexed_effects"B)          , Bool false);
        ((toString "debug_all"B)                                 , Bool false);
        ((toString "debug_all_modules"B)                         , Bool false);
        ((toString "debug"B)                                     , List []);
        ((toString "defensive"B)                                 , String (toString "no"B));
        ((toString "dep"B)                                       , Unset);
        ((toString "detail_errors"B)                             , Bool false);
        ((toString "detail_hint_replay"B)                        , Bool false);
        ((toString "disallow_unification_guards"B)               , Bool false);
        ((toString "dump_ast"B)                                  , Bool false);
        ((toString "dump_module"B)                               , List []);
        ((toString "eager_subtyping"B)                           , Bool false);
        ((toString "error_contexts"B)                            , Bool false);
        ((toString "expand_include"B)                            , Unset);
        ((toString "expose_interfaces"B)                         , Bool false);
        ((toString "extract_all"B)                               , Bool false);
        ((toString "extract_module"B)                            , List []);
        ((toString "extract_namespace"B)                         , List []);
        ((toString "extract"B)                                   , Unset);
        ((toString "ext"B)                                       , Unset);
        ((toString "force"B)                                     , Bool false);
        ((toString "fuel"B)                                      , Unset);
        ((toString "help"B)                                      , Bool false);
        ((toString "hide_uvar_nums"B)                            , Bool false);
        ((toString "hint_dir"B)                                  , Unset);
        ((toString "hint_file"B)                                 , Unset);
        ((toString "hint_hook"B)                                 , Unset);
        ((toString "hint_info"B)                                 , Bool false);
        ((toString "ide"B)                                       , Bool false);
        ((toString "ide_id_info_off"B)                           , Bool false);
        ((toString "ifuel"B)                                     , Unset);
        ((toString "include"B)                                   , List []);
        ((toString "initial_fuel"B)                              , Int (toInt 2));
        ((toString "initial_ifuel"B)                             , Int (toInt 1));
        ((toString "keep_query_captions"B)                       , Bool true);
        ((toString "krmloutput"B)                                , Unset);
        ((toString "lang_extensions"B)                           , List []);
        ((toString "lax"B)                                       , Bool false);
        ((toString "list_plugins"B)                              , Bool false);
        ((toString "load_cmxs"B)                                 , List []);
        ((toString "load"B)                                      , List []);
        ((toString "locate"B)                                    , Bool false);
        ((toString "locate_file"B)                               , Unset);
        ((toString "locate_lib"B)                                , Bool false);
        ((toString "locate_ocaml"B)                              , Bool false);
        ((toString "locate_z3"B)                                 , Unset);
        ((toString "log_failing_queries"B)                       , Bool false);
        ((toString "log_queries"B)                               , Bool false);
        ((toString "log_types"B)                                 , Bool false);
        ((toString "max_fuel"B)                                  , Int (toInt 8));
        ((toString "max_ifuel"B)                                 , Int (toInt 2));
        ((toString "message_format"B)                            , String (toString "auto"B));
        ((toString "MLish"B)                                     , Bool false);
        ((toString "MLish_effect"B)                              , String (toString "FStar.Effect"B));
        ((toString "no_extract"B)                                , List []);
        ((toString "no_location_info"B)                          , Bool false);
        ((toString "no_plugins"B)                                , Bool false);
        ((toString "__no_positivity"B)                           , Bool false);
        ((toString "no_prelude"B)                                , Bool false);
        ((toString "normalize_pure_terms_for_extraction"B)       , Bool false);
        ((toString "no_smt"B)                                    , Bool false);
        ((toString "no_tactics"B)                                , Bool false);
        ((toString "output_deps_to"B)                            , Unset);
        ((toString "output_to"B)                                 , Unset);
        ((toString "pretype"B)                                   , Bool true);
        ((toString "prims_ref"B)                                 , Unset);
        ((toString "prims"B)                                     , Unset);
        ((toString "print"B)                                     , Bool false);
        ((toString "print_bound_var_types"B)                     , Bool false);
        ((toString "print_cache_version"B)                       , Bool false);
        ((toString "print_effect_args"B)                         , Bool false);
        ((toString "print_expected_failures"B)                   , Bool false);
        ((toString "print_full_names"B)                          , Bool false);
        ((toString "print_implicits"B)                           , Bool false);
        ((toString "print_in_place"B)                            , Bool false);
        ((toString "print_universes"B)                           , Bool false);
        ((toString "print_z3_statistics"B)                       , Bool false);
        ((toString "prn"B)                                       , Bool false);
        ((toString "profile_component"B)                         , Unset);
        ((toString "profile_group_by_decl"B)                     , Bool false);
        ((toString "profile"B)                                   , Unset);
        ((toString "proof_recovery"B)                            , Bool false);
        ((toString "quake_hi"B)                                  , Int (toInt 1));
        ((toString "quake"B)                                     , Int (toInt 0));
        ((toString "quake_keep"B)                                , Bool false);
        ((toString "quake_lo"B)                                  , Int (toInt 1));
        ((toString "query_cache"B)                               , Bool false);
        ((toString "query_stats"B)                               , Bool false);
        ((toString "read_checked_file"B)                         , Unset);
        ((toString "read_krml_file"B)                            , Unset);
        ((toString "record_hints"B)                              , Bool false);
        ((toString "record_options"B)                            , Bool false);
        ((toString "report_assumes"B)                            , Unset);
        ((toString "retry"B)                                     , Bool false);
        ((toString "reuse_hint_for"B)                            , Unset);
        ((toString "silent"B)                                    , Bool false);
        ((toString "smtencoding.elim_box"B)                      , Bool false);
        ((toString "smtencoding.l_arith_repr"B)                  , String (toString "boxwrap"B));
        ((toString "smtencoding.nl_arith_repr"B)                 , String (toString "boxwrap"B));
        ((toString "smtencoding.valid_elim"B)                    , Bool false);
        ((toString "smtencoding.valid_intro"B)                   , Bool true);
        ((toString "smt"B)                                       , Unset);
        ((toString "split_queries"B)                             , String (toString "on_failure"B));
        ((toString "stats"B)                                     , Bool false);
        ((toString "tactic_raw_binders"B)                        , Bool false);
        ((toString "tactics_failhard"B)                          , Bool false);
        ((toString "tactics_info"B)                              , Bool false);
        ((toString "__tactics_nbe"B)                             , Bool false);
        ((toString "tactic_trace"B)                              , Bool false);
        ((toString "tactic_trace_d"B)                            , Int (toInt 0));
        ((toString "tcnorm"B)                                    , Bool true);
        ((toString "timing"B)                                    , Bool false);
        ((toString "trace_error"B)                               , Bool false);
        ((toString "trivial_pre_for_unannotated_effectful_fns"B) , Bool true);
        ((toString "ugly"B)                                      , Bool false);
        ((toString "unsafe_tactic_exec"B)                        , Bool false);
        ((toString "unthrottle_inductives"B)                     , Bool false);
        ((toString "use_eq_at_higher_order"B)                    , Bool false);
        ((toString "use_hint_hashes"B)                           , Bool false);
        ((toString "use_hints"B)                                 , Bool false);
        ((toString "use_native_tactics"B)                        , Unset);
        ((toString "use_nbe"B)                                   , Bool false);
        ((toString "use_nbe_for_extraction"B)                    , Bool false);
        ((toString "using_facts_from"B)                          , Unset);
        ((toString "verify_module"B)                             , List []);
        ((toString "warn_default_effects"B)                      , Bool false);
        ((toString "warn_error"B)                                , List []);
        ((toString "z3cliopt"B)                                  , List []);
        ((toString "z3refresh"B)                                 , Bool false);
        ((toString "z3rlimit_factor"B)                           , Int (toInt 1));
        ((toString "z3rlimit"B)                                  , Int (toInt 5));
        ((toString "z3seed"B)                                    , Int (toInt 0));
        ((toString "z3smtopt"B)                                  , List []);
        ((toString "z3version"B)                                 , String (toString "4.13.3"B));
    ]

    let init () =
        Debug.disable_all ();
        Ext.reset ();
        fstar_options := psmap_empty ();
        defaults |> List.iter set_option'                          //initialize it with the default values

    let clear () =
        history := [[]];
        init()

    (* Run it now. *)
    let _ = clear ()

    let get_option s =
        match psmap_try_find (peek ()) s with
        | None -> failwith ((toString "Impossible: option "B) ^.s^. (toString " not found"B))
        | Some s -> s

    let rec option_val_to_string (v:option_val) : Prims.string =
        match v with
        | Bool b -> (toString "Bool "B) ^. (defaultof<showable_bool> :> showable<bool>).show b
        | String s -> (toString "String "B) ^. (defaultof<showable_string> :> showable<Prims.string>).show s
        | Path s -> (toString "Path "B) ^. (defaultof<showable_string> :> showable<Prims.string>).show s
        | Int i -> (toString "Int "B) ^. (defaultof<showable_int> :> showable<Prims.int>).show i
        | List vs -> (toString "List "B) ^. Common.string_of_list option_val_to_string vs
        | Unset -> (toString "Unset"B)

    type showable_option_val =
        struct
            interface showable<option_val> with
                member _.show v = option_val_to_string v
        end
    

    let rec eq_option_val (v1: option_val) (v2: option_val) : bool =
        match v1, v2 with
        | Bool x1, Bool x2 -> x1 </(defaultof<deq_bool> :> deq<bool>).(=?)/> x2
        | String x1, String x2 -> x1 </(defaultof<deq_string> :> deq<Prims.string>).(=?)/> x2
        | Path x1, Path x2 -> x1 </(defaultof<deq_string> :> deq<Prims.string>).(=?)/> x2
        | Int x1, Int x2 -> x1 </(defaultof<deq_int> :> deq<Prims.int>).(=?)/> x2
        | Unset, Unset -> true
        | List x1, List x2 ->
            Common.eq_list eq_option_val x1 x2
        | _, _ -> false

    type deq_option_val =
        struct
            interface deq<option_val> with
                member _.(=?) x y = eq_option_val x y
        end

    let rec list_try_find ( _0: #deq<'a> ) (k : 'a) (l : list<'a * 'b>)
        : option<'b>
        =
            match l with
            | [] -> None
            | (k', v') :: l' ->
                if k </_0.(=?)/> k'
                then Some v'
                else list_try_find _0 k l'

    [<NoEquality; NoComparison>]
    type private ListMonad =
        struct
            member inline _.Return(x: 'a) = [x]
            member inline _.ReturnFrom(x: list<'a>) = x
            member inline _.Bind(x: list<'a>, f: 'a -> list<'b>) : list<'b> = List.concatMap f x
        end

    let show_options () =
        let s = peek () in
        let kvs : list<Prims.string * option_val> =
            ListMonad() {
                let! k = Common.psmap_keys s in
                (* verify_module is only set internally. *)
                if k = (toString "verify_module"B) then return! [] else
                let v = Some'v <| psmap_try_find s k in
                let v0 = list_try_find defaultof<deq_string> k defaults in
                if v0 </(defaultof<deq_option<option_val,deq_option_val>> :> deq<option<option_val>>).(=?)/> (Some v) then
                    return! []
                else
                    return (k, v)
            }
        in
        let rec show_optionval v =
            match v with
            | String s -> (toString "\""B) ^. s ^. (toString "\""B) // FIXME: proper escape
            | Bool b -> (defaultof<showable_bool> :> showable<bool>).show b
            | Int i -> (defaultof<showable_int> :> showable<Prims.int>).show i
            | Path s -> s
            | List s -> List.map show_optionval s |> String.concat (toString ","B)
            | Unset -> (toString "<unset>"B)
        in
        let show1 (k, v) =
            Format.fmt2 (toString "--%s %s"B) k (show_optionval v)
        in
        kvs |> List.map show1 |> String.concat (toString "\n"B)

    let set_verification_options o =
        (* This are all the options restored when processing a check_with
            attribute. All others are unchanged. We do this for two reasons:
            1) It's unsafe to just set everything (e.g. verify_module would
                    cause lax verification, so we need to filter some stuff out).
            2) So we don't propagate meaningless debugging options, which
                    is probably not intended.
        *)
        let verifopts = [
            (toString "initial_fuel"B);
            (toString "max_fuel"B);
            (toString "initial_ifuel"B);
            (toString "max_ifuel"B);
            (toString "detail_errors"B);
            (toString "detail_hint_replay"B);
            (toString "no_smt"B);
            (toString "quake"B);
            (toString "retry"B);
            (toString "smtencoding.elim_box"B);
            (toString "smtencoding.nl_arith_repr"B);
            (toString "smtencoding.l_arith_repr"B);
            (toString "smtencoding.valid_intro"B);
            (toString "smtencoding.valid_elim"B);
            (toString "tcnorm"B);
            (toString "no_plugins"B);
            (toString "no_tactics"B);
            (toString "z3cliopt"B);
            (toString "z3smtopt"B);
            (toString "z3refresh"B);
            (toString "z3rlimit"B);
            (toString "z3rlimit_factor"B);
            (toString "z3seed"B);
            (toString "z3version"B);
            (toString "trivial_pre_for_unannotated_effectful_fns"B);
        ] in
        List.iter (fun k -> set_option k (psmap_try_find o k |> Some'v)) verifopts

    let lookup_opt s c =
        c (get_option s)


    let file_list_ : ref<list<Prims.string>> = mk_ref []
#if false
    (* In `parse_filename_arg specs arg`:

        * `arg` is a filename argument to be parsed. If `arg` is of the
            form `@file`, then `file` is a response file, from which further
            arguments (including further options) are read. Nested response
            files (@ response file arguments within response files) are
            supported.

        * `specs` is the list of option specifications (- and --)

        * `enable_filenames` is a boolean, true if non-response file
        * filenames should be handled.

    *)

    let rec parse_filename_arg specs enable_filenames arg =
        if Util.starts_with arg (toString "@"B)
        then begin
            // read and parse a response file
            let filename = Util.substring_from arg (toInt 1) in
            let lines = Util.file_get_lines filename in
            Getopt.parse_list specs (parse_filename_arg specs enable_filenames) lines
        end else begin
            if enable_filenames
            then file_list_ := !file_list_ @ [arg];
            Success
        end

    (* A copy of the optionstate right after parsing the command line,
    so we can reset back to it. *)
    let parsed_args_state : ref<option<history1>> = mk_ref None

    let parse_cmd_line () =
        let res = Getopt.parse_cmdline all_specs_getopt (parse_filename_arg all_specs_getopt true) in
        let res =
            if res = Success
            then set_error_flags()
            else res
        in
        (* Set the include path, and check that they exist. We do the existence check
        here, and not in the handler for --include, to respect a --warn_error ignoring
        this warning. *)
        let () =
            let paths = as_list as_string (get_option (toString "include"B)) in
            paths |> List.iter (fun p -> !check_include_dir p);
            Find.set_include_path (Find.get_include_path () @ paths);
            ()
        in
        parsed_args_state := Some (snapshot_all ());
        res, !file_list_
#endif
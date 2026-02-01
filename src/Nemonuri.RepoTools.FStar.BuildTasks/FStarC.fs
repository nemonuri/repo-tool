#nowarn "62" // This construct is for ML compatibility. Consider using the '+' operator instead. This may require a type annotation to indicate it acts on strings. This message can be disabled using '--nowarn:62' or '#nowarn "62"'
#nowarn "3370" // The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.

namespace Nemonuri.RepoTools.FStar.BuildTasks.FStarC

module PSMap =

    //--- Copy from: https://github.com/FStarLang/FStar/blob/master/src/ml/FStarC_PSMap.ml ---

    type private StringMap<'value> = Map<string, 'value>
    exception Found

    type t<'value> = StringMap<'value>
    let empty (_: unit) : 'value t = Map.empty<string, 'value>
    let add (map: 'value t) (key: string) (value: 'value) = Map.add key value map
    let find_default (map: 'value t) (key: string) (dflt: 'value) = 
        Map.tryFind key map |> Option.defaultValue dflt
    let of_list (l: (string * 'value) list) : 'value t = List.fold (fun acc (k,v) -> add acc k v) (empty ()) l
    let try_find (map: 'value t) (key: string) = Map.tryFind key map
    let fold (m:'value t) f a = Map.foldBack f m a
    let find_map (m:'value t) f =
        let res = ref None in
        let upd k v =
            let r = f k v in
            if r <> None then (res := r; raise Found) in
        (try Map.iter upd m with Found -> ());
        !res
    let modify (m: 'value t) (k: string) (upd: 'value option -> 'value) =
        Map.change k (fun vopt -> Some (upd vopt)) m

    let merge (m1: 'value t) (m2: 'value t) : 'value t =
        fold m1 (fun k v m -> add m k v) m2

    let remove (m: 'value t)  (key:string) : 'value t = Map.remove key m

    let keys m = fold m (fun k _ acc -> k::acc) []
    let iter (m:'value t) f = Map.iter f m

    //---|

    //--- Copy from: https://github.com/FStarLang/FStar/blob/master/src/data/FStarC.PSMap.fsti ---
    type psmap<'a> = t<'a>
    let inline psmap_empty () = empty ()
    let inline psmap_add m k v = add m k v
    let inline psmap_find_default m k v = find_default m k v
    let inline psmap_try_find m k = try_find m k
    let inline psmap_fold m f a = fold m f a
    let inline psmap_find_map m f = find_map m f
    let inline psmap_modify m k f = modify m k f
    let inline psmap_merge m1 m2= merge m1 m2
    let inline psmap_remove m k = remove m k
    let inline psmap_iter m f = iter m f
    //---|

module Options = 

    open PSMap

    let split (target: string) (separator: string) = 
        target.Split([|separator|], System.StringSplitOptions.None) |> List.ofArray

    //--- Copy from: https://github.com/FStarLang/FStar/blob/master/src/basic/FStarC.Options.fsti ---

    type codegen_t =
    | OCaml
    | FSharp
    | Krml
    | Plugin
    | PluginNoLib
    | Extension

    type option_val =
    | Bool of bool
    | String of string
    | Path of string
    | Int of int
    | List of option_val list
    | Unset

    type split_queries_t = | No | OnFailure | Always

    type message_format_t = | Json | Human | Github

    type optionstate = option_val PSMap.t 

    //---|

    //--- Copy from: https://github.com/FStarLang/FStar/blob/master/src/basic/FStarC.Options.fst ---
    let as_bool = function
    | Bool b -> b
    | _ -> failwith "Impos: expected Bool"
    let as_int = function
    | Int b -> b
    | _ -> failwith "Impos: expected Int"
    let as_string = function
    | String b -> b
    | Path b -> b
    | _ -> failwith "Impos: expected String"
    let as_list' = function
    | List ts -> ts
    | _ -> failwith "Impos: expected List"
    let as_list as_t x =
        as_list' x |> List.map as_t
    let as_option as_t = function
    | Unset -> None
    | v -> Some (as_t v)
    let as_comma_string_list = function
    | List ls -> List.collect (fun l -> split (as_string l) ",") ls
    | _ -> failwith "Impos: expected String (comma list)"

    let defaults = [
        ("abort_on"                                  , Int 0);
        ("admit_except"                              , Unset);
        ("admit_smt_queries"                         , Bool false);
        ("already_cached"                            , Unset);
        ("cache_checked_modules"                     , Bool false);
        ("cache_off"                                 , Bool false);
        ("cmi"                                       , Bool false);
        ("codegen-lib"                               , List []);
        ("codegen"                                   , Unset);
        ("compat_pre_core"                           , Unset);
        ("compat_pre_typed_indexed_effects"          , Bool false);
        ("debug_all"                                 , Bool false);
        ("debug_all_modules"                         , Bool false);
        ("debug"                                     , List []);
        ("defensive"                                 , String "no");
        ("dep"                                       , Unset);
        ("detail_errors"                             , Bool false);
        ("detail_hint_replay"                        , Bool false);
        ("disallow_unification_guards"               , Bool false);
        ("dump_ast"                                  , Bool false);
        ("dump_module"                               , List []);
        ("eager_subtyping"                           , Bool false);
        ("error_contexts"                            , Bool false);
        ("expand_include"                            , Unset);
        ("expose_interfaces"                         , Bool false);
        ("extract_all"                               , Bool false);
        ("extract_module"                            , List []);
        ("extract_namespace"                         , List []);
        ("extract"                                   , Unset);
        ("ext"                                       , Unset);
        ("force"                                     , Bool false);
        ("fuel"                                      , Unset);
        ("help"                                      , Bool false);
        ("hide_uvar_nums"                            , Bool false);
        ("hint_dir"                                  , Unset);
        ("hint_file"                                 , Unset);
        ("hint_hook"                                 , Unset);
        ("hint_info"                                 , Bool false);
        ("ide"                                       , Bool false);
        ("ide_id_info_off"                           , Bool false);
        ("ifuel"                                     , Unset);
        ("include"                                   , List []);
        ("initial_fuel"                              , Int 2);
        ("initial_ifuel"                             , Int 1);
        ("keep_query_captions"                       , Bool true);
        ("krmloutput"                                , Unset);
        ("lang_extensions"                           , List []);
        ("lax"                                       , Bool false);
        ("list_plugins"                              , Bool false);
        ("load_cmxs"                                 , List []);
        ("load"                                      , List []);
        ("locate"                                    , Bool false);
        ("locate_file"                               , Unset);
        ("locate_lib"                                , Bool false);
        ("locate_ocaml"                              , Bool false);
        ("locate_z3"                                 , Unset);
        ("log_failing_queries"                       , Bool false);
        ("log_queries"                               , Bool false);
        ("log_types"                                 , Bool false);
        ("max_fuel"                                  , Int 8);
        ("max_ifuel"                                 , Int 2);
        ("message_format"                            , String "auto");
        ("MLish"                                     , Bool false);
        ("MLish_effect"                              , String "FStar.Effect");
        ("no_extract"                                , List []);
        ("no_location_info"                          , Bool false);
        ("no_plugins"                                , Bool false);
        ("__no_positivity"                           , Bool false);
        ("no_prelude"                                , Bool false);
        ("normalize_pure_terms_for_extraction"       , Bool false);
        ("no_smt"                                    , Bool false);
        ("no_tactics"                                , Bool false);
        ("output_deps_to"                            , Unset);
        ("output_to"                                 , Unset);
        ("pretype"                                   , Bool true);
        ("prims_ref"                                 , Unset);
        ("prims"                                     , Unset);
        ("print"                                     , Bool false);
        ("print_bound_var_types"                     , Bool false);
        ("print_cache_version"                       , Bool false);
        ("print_effect_args"                         , Bool false);
        ("print_expected_failures"                   , Bool false);
        ("print_full_names"                          , Bool false);
        ("print_implicits"                           , Bool false);
        ("print_in_place"                            , Bool false);
        ("print_universes"                           , Bool false);
        ("print_z3_statistics"                       , Bool false);
        ("prn"                                       , Bool false);
        ("profile_component"                         , Unset);
        ("profile_group_by_decl"                     , Bool false);
        ("profile"                                   , Unset);
        ("proof_recovery"                            , Bool false);
        ("quake_hi"                                  , Int 1);
        ("quake"                                     , Int 0);
        ("quake_keep"                                , Bool false);
        ("quake_lo"                                  , Int 1);
        ("query_cache"                               , Bool false);
        ("query_stats"                               , Bool false);
        ("read_checked_file"                         , Unset);
        ("read_krml_file"                            , Unset);
        ("record_hints"                              , Bool false);
        ("record_options"                            , Bool false);
        ("report_assumes"                            , Unset);
        ("retry"                                     , Bool false);
        ("reuse_hint_for"                            , Unset);
        ("silent"                                    , Bool false);
        ("smtencoding.elim_box"                      , Bool false);
        ("smtencoding.l_arith_repr"                  , String "boxwrap");
        ("smtencoding.nl_arith_repr"                 , String "boxwrap");
        ("smtencoding.valid_elim"                    , Bool false);
        ("smtencoding.valid_intro"                   , Bool true);
        ("smt"                                       , Unset);
        ("split_queries"                             , String "on_failure");
        ("stats"                                     , Bool false);
        ("tactic_raw_binders"                        , Bool false);
        ("tactics_failhard"                          , Bool false);
        ("tactics_info"                              , Bool false);
        ("__tactics_nbe"                             , Bool false);
        ("tactic_trace"                              , Bool false);
        ("tactic_trace_d"                            , Int 0);
        ("tcnorm"                                    , Bool true);
        ("timing"                                    , Bool false);
        ("trace_error"                               , Bool false);
        ("trivial_pre_for_unannotated_effectful_fns" , Bool true);
        ("ugly"                                      , Bool false);
        ("unsafe_tactic_exec"                        , Bool false);
        ("unthrottle_inductives"                     , Bool false);
        ("use_eq_at_higher_order"                    , Bool false);
        ("use_hint_hashes"                           , Bool false);
        ("use_hints"                                 , Bool false);
        ("use_native_tactics"                        , Unset);
        ("use_nbe"                                   , Bool false);
        ("use_nbe_for_extraction"                    , Bool false);
        ("using_facts_from"                          , Unset);
        ("verify_module"                             , List []);
        ("warn_default_effects"                      , Bool false);
        ("warn_error"                                , List []);
        ("z3cliopt"                                  , List []);
        ("z3refresh"                                 , Bool false);
        ("z3rlimit_factor"                           , Int 1);
        ("z3rlimit"                                  , Int 5);
        ("z3seed"                                    , Int 0);
        ("z3smtopt"                                  , List []);
        ("z3version"                                 , String "4.13.3");
    ]

    type fstar_options_t =
        val private fstar_options : optionstate ref
        
        new() = { fstar_options = { contents = psmap_empty() } }

        member this.peek() = !this.fstar_options

        member this.set_option k v =
            let map : optionstate = this.peek() in
            if k = "report_assumes" then
                match psmap_try_find map k with
                | Some (String "error") ->
                    //It's already set to error; ignore any attempt to change it
                    ()
                | _ -> this.fstar_options := psmap_add map k v
            else this.fstar_options := psmap_add map k v

        member this.get_option s =
            match psmap_try_find (this.peek ()) s with
            | None -> failwith ("Impossible: option " ^s^ " not found")
            | Some s -> s
        
        member this.init() =
            let set_option' (k, v) = this.set_option k v
            this.fstar_options := psmap_empty()
            defaults |> List.iter set_option'
        
        member this.lookup_opt s c =
            c (this.get_option s)

        member this.get_abort_on                ()      = this.lookup_opt "abort_on"                 as_int
        member this.get_admit_smt_queries       ()      = this.lookup_opt "admit_smt_queries"        as_bool
        member this.get_admit_except            ()      = this.lookup_opt "admit_except"             (as_option as_string)
        member this.get_compat_pre_core         ()      = this.lookup_opt "compat_pre_core"          (as_option as_int)

        member this.get_compat_pre_typed_indexed_effects ()  = this.lookup_opt "compat_pre_typed_indexed_effects" as_bool
        member this.get_disallow_unification_guards  ()      = this.lookup_opt "disallow_unification_guards"      as_bool

        member this.get_already_cached          ()      = this.lookup_opt "already_cached"           (as_option (as_list as_string))
        member this.get_cache_checked_modules   ()      = this.lookup_opt "cache_checked_modules"    as_bool
        member this.get_cache_off               ()      = this.lookup_opt "cache_off"                as_bool
        member this.get_print_cache_version     ()      = this.lookup_opt "print_cache_version"      as_bool
        member this.get_cmi                     ()      = this.lookup_opt "cmi"                      as_bool
        member this.get_codegen                 ()      = this.lookup_opt "codegen"                  (as_option as_string)
        member this.get_codegen_lib             ()      = this.lookup_opt "codegen-lib"              (as_list as_string)
        member this.get_defensive               ()      = this.lookup_opt "defensive"                as_string
        member this.get_dep                     ()      = this.lookup_opt "dep"                      (as_option as_string)
        member this.get_detail_errors           ()      = this.lookup_opt "detail_errors"            as_bool
        member this.get_detail_hint_replay      ()      = this.lookup_opt "detail_hint_replay"       as_bool
        member this.get_dump_ast                ()      = this.lookup_opt "dump_ast"                 as_bool
        member this.get_dump_module             ()      = this.lookup_opt "dump_module"              (as_list as_string)
        member this.get_eager_subtyping         ()      = this.lookup_opt "eager_subtyping"          as_bool
        member this.get_error_contexts          ()      = this.lookup_opt "error_contexts"           as_bool
        member this.get_expose_interfaces       ()      = this.lookup_opt "expose_interfaces"        as_bool
        member this.get_message_format          ()      = this.lookup_opt "message_format"           as_string
        member this.get_extract                 ()      = this.lookup_opt "extract"                  (as_option (as_list as_string))
        member this.get_extract_module          ()      = this.lookup_opt "extract_module"           (as_list as_string)
        member this.get_extract_namespace       ()      = this.lookup_opt "extract_namespace"        (as_list as_string)
        member this.get_force                   ()      = this.lookup_opt "force"                    as_bool
        member this.get_help                    ()      = this.lookup_opt "help"                     as_bool
        member this.get_hide_uvar_nums          ()      = this.lookup_opt "hide_uvar_nums"           as_bool
        member this.get_hint_info               ()      = this.lookup_opt "hint_info"                as_bool
        member this.get_hint_dir                ()      = this.lookup_opt "hint_dir"                 (as_option as_string)
        member this.get_hint_file               ()      = this.lookup_opt "hint_file"                (as_option as_string)
        member this.get_ide                     ()      = this.lookup_opt "ide"                      as_bool
        member this.get_ide_id_info_off         ()      = this.lookup_opt "ide_id_info_off"          as_bool
        member this.get_print                   ()      = this.lookup_opt "print"                    as_bool
        member this.get_print_in_place          ()      = this.lookup_opt "print_in_place"           as_bool
        member this.get_initial_fuel            ()      = this.lookup_opt "initial_fuel"             as_int
        member this.get_initial_ifuel           ()      = this.lookup_opt "initial_ifuel"            as_int
        member this.get_keep_query_captions     ()      = this.lookup_opt "keep_query_captions"      as_bool
        member this.get_lang_extensions         ()      = this.lookup_opt "lang_extensions"                     (as_list as_string)
        member this.get_lax                     ()      = this.lookup_opt "lax"                      as_bool
        member this.get_load                    ()      = this.lookup_opt "load"                     (as_list as_string)
        member this.get_load_cmxs               ()      = this.lookup_opt "load_cmxs"                (as_list as_string)
        member this.get_log_queries             ()      = this.lookup_opt "log_queries"              as_bool
        member this.get_log_failing_queries     ()      = this.lookup_opt "log_failing_queries"      as_bool
        member this.get_log_types               ()      = this.lookup_opt "log_types"                as_bool
        member this.get_max_fuel                ()      = this.lookup_opt "max_fuel"                 as_int
        member this.get_max_ifuel               ()      = this.lookup_opt "max_ifuel"                as_int
        member this.get_MLish                   ()      = this.lookup_opt "MLish"                    as_bool
        member this.get_MLish_effect            ()      = this.lookup_opt "MLish_effect"             as_string
        member this.get_no_extract              ()      = this.lookup_opt "no_extract"               (as_list as_string)
        member this.get_no_location_info        ()      = this.lookup_opt "no_location_info"         as_bool
        member this.get_no_prelude              ()      = this.lookup_opt "no_prelude"               as_bool
        member this.get_no_plugins              ()      = this.lookup_opt "no_plugins"               as_bool
        member this.get_no_smt                  ()      = this.lookup_opt "no_smt"                   as_bool
        member this.get_normalize_pure_terms_for_extraction
                                        ()      = this.lookup_opt "normalize_pure_terms_for_extraction" as_bool
        member this.get_output_to               ()      = this.lookup_opt "output_to"                (as_option as_string)
        member this.get_krmloutput              ()      = this.lookup_opt "krmloutput"               (as_option as_string)
        member this.get_output_deps_to          ()      = this.lookup_opt "output_deps_to"           (as_option as_string)
        member this.get_ugly                    ()      = this.lookup_opt "ugly"                     as_bool
        member this.get_prims                   ()      = this.lookup_opt "prims"                    (as_option as_string)
        member this.get_print_bound_var_types   ()      = this.lookup_opt "print_bound_var_types"    as_bool
        member this.get_print_effect_args       ()      = this.lookup_opt "print_effect_args"        as_bool
        member this.get_print_expected_failures ()      = this.lookup_opt "print_expected_failures"  as_bool
        member this.get_print_full_names        ()      = this.lookup_opt "print_full_names"         as_bool
        member this.get_print_implicits         ()      = this.lookup_opt "print_implicits"          as_bool
        member this.get_print_universes         ()      = this.lookup_opt "print_universes"          as_bool
        member this.get_print_z3_statistics     ()      = this.lookup_opt "print_z3_statistics"      as_bool
        member this.get_prn                     ()      = this.lookup_opt "prn"                      as_bool
        member this.get_proof_recovery          ()      = this.lookup_opt "proof_recovery"           as_bool
        member this.get_quake_lo                ()      = this.lookup_opt "quake_lo"                 as_int
        member this.get_quake_hi                ()      = this.lookup_opt "quake_hi"                 as_int
        member this.get_quake_keep              ()      = this.lookup_opt "quake_keep"               as_bool
        member this.get_query_cache             ()      = this.lookup_opt "query_cache"              as_bool
        member this.get_query_stats             ()      = this.lookup_opt "query_stats"              as_bool
        member this.get_read_checked_file       ()      = this.lookup_opt "read_checked_file"        (as_option as_string)
        member this.get_read_krml_file          ()      = this.lookup_opt "read_krml_file"           (as_option as_string)
        member this.get_list_plugins            ()      = this.lookup_opt "list_plugins"             as_bool
        member this.get_locate                  ()      = this.lookup_opt "locate"                   as_bool
        member this.get_locate_lib              ()      = this.lookup_opt "locate_lib"               as_bool
        member this.get_locate_ocaml            ()      = this.lookup_opt "locate_ocaml"             as_bool
        member this.get_locate_file             ()      = this.lookup_opt "locate_file"              (as_option as_string)
        member this.get_expand_include          ()      = this.lookup_opt "expand_include"           (as_option as_string)
        member this.get_locate_z3               ()      = this.lookup_opt "locate_z3"                (as_option as_string)
        member this.get_record_hints            ()      = this.lookup_opt "record_hints"             as_bool
        member this.get_record_options          ()      = this.lookup_opt "record_options"           as_bool
        member this.get_retry                   ()      = this.lookup_opt "retry"                    as_bool
        member this.get_reuse_hint_for          ()      = this.lookup_opt "reuse_hint_for"           (as_option as_string)
        member this.get_report_assumes          ()      = this.lookup_opt "report_assumes"           (as_option as_string)
        member this.get_silent                  ()      = this.lookup_opt "silent"                   as_bool
        member this.get_smt                     ()      = this.lookup_opt "smt"                      (as_option as_string)
        member this.get_smtencoding_elim_box    ()      = this.lookup_opt "smtencoding.elim_box"     as_bool
        member this.get_smtencoding_nl_arith_repr ()    = this.lookup_opt "smtencoding.nl_arith_repr" as_string
        member this.get_smtencoding_l_arith_repr()      = this.lookup_opt "smtencoding.l_arith_repr" as_string
        member this.get_smtencoding_valid_intro ()      = this.lookup_opt "smtencoding.valid_intro"  as_bool
        member this.get_smtencoding_valid_elim  ()      = this.lookup_opt "smtencoding.valid_elim"   as_bool
        member this.get_split_queries           ()      = this.lookup_opt "split_queries"            as_string
        member this.get_stats                   ()      = this.lookup_opt "stats"                    as_bool
        member this.get_tactic_raw_binders      ()      = this.lookup_opt "tactic_raw_binders"       as_bool
        member this.get_tactics_failhard        ()      = this.lookup_opt "tactics_failhard"         as_bool
        member this.get_tactics_info            ()      = this.lookup_opt "tactics_info"             as_bool
        member this.get_tactic_trace            ()      = this.lookup_opt "tactic_trace"             as_bool
        member this.get_tactic_trace_d          ()      = this.lookup_opt "tactic_trace_d"           as_int
        member this.get_tactics_nbe             ()      = this.lookup_opt "__tactics_nbe"            as_bool
        member this.get_tcnorm                  ()      = this.lookup_opt "tcnorm"                   as_bool
        member this.get_timing                  ()      = this.lookup_opt "timing"                   as_bool
        member this.get_trace_error             ()      = this.lookup_opt "trace_error"              as_bool
        member this.get_unthrottle_inductives   ()      = this.lookup_opt "unthrottle_inductives"    as_bool
        member this.get_unsafe_tactic_exec      ()      = this.lookup_opt "unsafe_tactic_exec"       as_bool
        member this.get_use_eq_at_higher_order  ()      = this.lookup_opt "use_eq_at_higher_order"   as_bool
        member this.get_use_hints               ()      = this.lookup_opt "use_hints"                as_bool
        member this.get_use_hint_hashes         ()      = this.lookup_opt "use_hint_hashes"          as_bool
        member this.get_use_native_tactics      ()      = this.lookup_opt "use_native_tactics"       (as_option as_string)
        member this.get_no_tactics              ()      = this.lookup_opt "no_tactics"               as_bool
        member this.get_using_facts_from        ()      = this.lookup_opt "using_facts_from"         (as_option (as_list as_string))
        member this.get_verify_module           ()      = this.lookup_opt "verify_module"            (as_list as_string)
        member this.get_version                 ()      = this.lookup_opt "version"                  as_bool
        member this.get_warn_default_effects    ()      = this.lookup_opt "warn_default_effects"     as_bool
        member this.get_z3cliopt                ()      = this.lookup_opt "z3cliopt"                 (as_list as_string)
        member this.get_z3smtopt                ()      = this.lookup_opt "z3smtopt"                 (as_list as_string)
        member this.get_z3refresh               ()      = this.lookup_opt "z3refresh"                as_bool
        member this.get_z3rlimit                ()      = this.lookup_opt "z3rlimit"                 as_int
        member this.get_z3rlimit_factor         ()      = this.lookup_opt "z3rlimit_factor"          as_int
        member this.get_z3seed                  ()      = this.lookup_opt "z3seed"                   as_int
        member this.get_z3version               ()      = this.lookup_opt "z3version"                as_string
        member this.get_no_positivity           ()      = this.lookup_opt "__no_positivity"          as_bool
        member this.get_warn_error              ()      = this.lookup_opt "warn_error"               (as_list as_string)
        member this.get_use_nbe                 ()      = this.lookup_opt "use_nbe"                  as_bool
        member this.get_use_nbe_for_extraction  ()      = this.lookup_opt "use_nbe_for_extraction"                  as_bool
        member this.get_trivial_pre_for_unannotated_effectful_fns
                                        ()      = this.lookup_opt "trivial_pre_for_unannotated_effectful_fns"    as_bool
        member this.get_profile                 ()      = this.lookup_opt "profile"                  (as_option (as_list as_string))
        member this.get_profile_group_by_decl   ()      = this.lookup_opt "profile_group_by_decl"    as_bool
        member this.get_profile_component       ()      = this.lookup_opt "profile_component"        (as_option (as_list as_string))



    let settable = function
    | "__temp_fast_implicits"
    | "abort_on"
    | "admit_except"
    | "admit_smt_queries"
    | "compat_pre_core"
    | "compat_pre_typed_indexed_effects"
    | "disallow_unification_guards"
    | "debug"
    | "debug_all"
    | "debug_all_modules"
    | "defensive"
    | "detail_errors"
    | "detail_hint_replay"
    | "eager_subtyping"
    | "error_contexts"
    | "hide_uvar_nums"
    | "hint_dir"
    | "hint_file"
    | "hint_info"
    | "fuel"
    | "ext"
    | "ifuel"
    | "initial_fuel"
    | "initial_ifuel"
    | "ide_id_info_off"
    | "keep_query_captions"
    | "lang_extensions"
    | "load"
    | "load_cmxs"
    | "log_queries"
    | "log_failing_queries"
    | "log_types"
    | "max_fuel"
    | "max_ifuel"
    | "no_plugins"
    | "__no_positivity"
    | "normalize_pure_terms_for_extraction"
    | "no_smt"
    | "no_tactics"
    | "print_bound_var_types"
    | "print_effect_args"
    | "print_expected_failures"
    | "print_full_names"
    | "print_implicits"
    | "print_universes"
    | "print_z3_statistics"
    | "prn"
    | "quake_lo"
    | "quake_hi"
    | "quake_keep"
    | "quake"
    | "query_cache"
    | "query_stats"
    | "record_hints"
    | "record_options"
    | "retry"
    | "reuse_hint_for"
    | "report_assumes"
    | "silent"
    | "smtencoding.elim_box"
    | "smtencoding.l_arith_repr"
    | "smtencoding.nl_arith_repr"
    | "smtencoding.valid_intro"
    | "smtencoding.valid_elim"
    | "split_queries"
    | "stats"
    | "tactic_raw_binders"
    | "tactics_failhard"
    | "tactics_info"
    | "__tactics_nbe"
    | "tactic_trace"
    | "tactic_trace_d"
    | "tcnorm"
    | "timing"
    | "trace_error"
    | "ugly"
    | "unthrottle_inductives"
    | "use_eq_at_higher_order"
    | "using_facts_from"
    | "warn_error"
    | "z3cliopt"
    | "z3smtopt"
    | "z3refresh"
    | "z3rlimit"
    | "z3rlimit_factor"
    | "z3seed"
    | "z3version"
    | "trivial_pre_for_unannotated_effectful_fns"
    | "profile_group_by_decl"
    | "profile_component"
    | "profile" -> true
    | _ -> false

    //---|


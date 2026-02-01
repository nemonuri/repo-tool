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

module Options = 

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


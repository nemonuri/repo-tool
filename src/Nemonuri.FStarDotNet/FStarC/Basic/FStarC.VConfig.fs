// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.VConfig.fsti

module Nemonuri.FStarDotNet.FStarC.VConfig

(** This type represents the set of verification-relevant options used
    to check a particular definition. It can be read from tactics via
    sigelt_opts and set via the check_with attribute.
 *)
type vconfig = {
  initial_fuel                              : int;
  max_fuel                                  : int;
  initial_ifuel                             : int;
  max_ifuel                                 : int;
  detail_errors                             : bool;
  detail_hint_replay                        : bool;
  no_smt                                    : bool;
  quake_lo                                  : int;
  quake_hi                                  : int;
  quake_keep                                : bool;
  retry                                     : bool;
  smtencoding_elim_box                      : bool;
  smtencoding_nl_arith_repr                 : string;
  smtencoding_l_arith_repr                  : string;
  smtencoding_valid_intro                   : bool;
  smtencoding_valid_elim                    : bool;
  tcnorm                                    : bool;
  no_plugins                                : bool;
  no_tactics                                : bool;
  z3cliopt                                  : list<string>;
  z3smtopt                                  : list<string>;  
  z3refresh                                 : bool;
  z3rlimit                                  : int;
  z3rlimit_factor                           : int;
  z3seed                                    : int;
  z3version                                 : string;
  trivial_pre_for_unannotated_effectful_fns : bool;
  reuse_hint_for                            : option<string>;
}

(** Marker to check a sigelt with a particular vconfig, not really used internally.. *)
let check_with (vcfg : vconfig) : unit = ()
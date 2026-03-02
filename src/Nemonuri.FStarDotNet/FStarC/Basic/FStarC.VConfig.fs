// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.VConfig.fsti

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Forwarded
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

module VConfig =

    (** This type represents the set of verification-relevant options used
        to check a particular definition. It can be read from tactics via
        sigelt_opts and set via the check_with attribute.
    *)
    type FStar_vconfig = {
        initial_fuel                              : Prims.int;
        max_fuel                                  : Prims.int;
        initial_ifuel                             : Prims.int;
        max_ifuel                                 : Prims.int;
        detail_errors                             : Prims.bool;
        detail_hint_replay                        : Prims.bool;
        no_smt                                    : Prims.bool;
        quake_lo                                  : Prims.int;
        quake_hi                                  : Prims.int;
        quake_keep                                : Prims.bool;
        retry                                     : Prims.bool;
        smtencoding_elim_box                      : Prims.bool;
        smtencoding_nl_arith_repr                 : Prims.string;
        smtencoding_l_arith_repr                  : Prims.string;
        smtencoding_valid_intro                   : Prims.bool;
        smtencoding_valid_elim                    : Prims.bool;
        tcnorm                                    : Prims.bool;
        no_plugins                                : Prims.bool;
        no_tactics                                : Prims.bool;
        z3cliopt                                  : Prims.list<Prims.string>;
        z3smtopt                                  : Prims.list<Prims.string>;  
        z3refresh                                 : Prims.bool;
        z3rlimit                                  : Prims.int;
        z3rlimit_factor                           : Prims.int;
        z3seed                                    : Prims.int;
        z3version                                 : Prims.string;
        trivial_pre_for_unannotated_effectful_fns : Prims.bool;
        reuse_hint_for                            : FStar_Pervasives_Native.option<Prims.string>;
    }
    type vconfig = Effect.ML<FStar_vconfig>

    (** Marker to check a sigelt with a particular vconfig, not really used internally.. *)
    let check_with (vcfg : vconfig) : Prims.unit = Fu.pur ()

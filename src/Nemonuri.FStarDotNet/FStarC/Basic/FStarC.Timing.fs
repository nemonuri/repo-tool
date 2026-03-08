// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Timing.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Timing.ml

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.OCamlDotNet.Zarith

module Timing =

    type time_ns = int64

    /// val now_ns : unit -> time_ns
    let now_ns () = System.DateTime.Now.Ticks

    /// val diff_ns : time_ns -> time_ns -> int
    let diff_ns (t1: time_ns) (t2: time_ns) =
        Z.of_int (Core.Operators.int32 ((-) t2 t1))

    /// val diff_ms : time_ns -> time_ns -> int
    let diff_ms t1 t2 = Z.div (diff_ns t1 t2) (Z.of_int 1000000)

    /// val record_ns (f : unit -> 'a) : 'a & int
    let record_ns f =
        let start = now_ns () in
        let res = f () in
        let elapsed = diff_ns start (now_ns()) in
        res, elapsed

    /// val record_ms (f : unit -> 'a) : 'a & int
    let record_ms f =
        let res, ns = record_ns f in
        res, Z.div ns (Z.of_int 1000000)
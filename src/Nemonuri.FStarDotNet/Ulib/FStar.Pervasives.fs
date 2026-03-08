#nowarn "25"    // Incomplete pattern matches

// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/FStar.Pervasives.fsti

namespace Nemonuri.FStarDotNet.FStar

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Forwarded


module Pervasives =

    module Native =

        type option<'a> = FStar_Pervasives_Native.option<'a>

        let Some'v o = FStar_Pervasives_Native.Some'v o

    (** Values of type [a] or type [b] *)
    type either<'a,'b> =
    | Inl of v : 'a
    | Inr of v : 'b

    let Inl'v (Inl(v)) = v
    let Inr'v (Inr(v)) = v


namespace Nemonuri.OCamlDotNet.Primitives

open Nemonuri.OCamlDotNet.Primitives.TypeShadowing

module Printf =

    type format<'a,'b,'c> = OCamlFormat<'a,'b,'c>

    //let fprintf (outchan: out_channel) (format: format<'a, out_channel, unit>) (arg: 'a) =

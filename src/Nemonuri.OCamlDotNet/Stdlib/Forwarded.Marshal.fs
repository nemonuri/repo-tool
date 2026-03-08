namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.OCamlDotNet.Primitives

module Marshal =

    type extern_flags = 
    | No_sharing  (* Don't preserve sharing *)
    | Closures  (* Send function closures *)
    | Compat_32  (* Ensure 32-bit compatibility *)

    // let to_channel<'a> (chan: OCamlOutChannel) (v: 'a) (flags: extern_flags list) =

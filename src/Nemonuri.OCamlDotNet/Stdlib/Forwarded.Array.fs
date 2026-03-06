namespace Nemonuri.OCamlDotNet.Forwarded

module Fa = Microsoft.FSharp.Collections.Array

/// reference: https://ocaml.org/manual/5.4/api/Array.html
module Array =

    type t<'a> = array<'a>

    let length a = Fa.length a

    let get a n = Fa.get a n

    let of_list l = Fa.ofList l

    let to_list a = Fa.toList a

    module Operators =

        let ( .() ) a n = get a n
    
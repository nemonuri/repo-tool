namespace Nemonuri.OCamlDotNet.Primitives

open Nemonuri.OCamlDotNet.Primitives.Operations
module S = OCamlByteSpanSources

module Exceptions =

    exception Not_found = Nemonuri.OCamlDotNet.Primitives.Not_found

    exception Invalid_argument = System.ArgumentException

    let invalid_arg (msg: OCamlString) = let s = OCamlStrings.toDotNetString msg in raise (System.ArgumentException(s))

    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? System.ArgumentException as ae -> ae.Message |> OCamlStrings.ofDotNetString |> Some
        | _ -> None
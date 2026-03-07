namespace Nemonuri.OCamlDotNet.Primitives

module O = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

module Exceptions =

    exception Not_found = Nemonuri.OCamlDotNet.Primitives.Not_found

    exception Invalid_argument = System.ArgumentException

    let invalid_arg (msg: OCamlString) = let s = O.stringToDotNetString msg in raise (System.ArgumentException(s))

    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? System.ArgumentException as ae -> ae.Message |> O.stringOfDotNetString |> Some
        | _ -> None
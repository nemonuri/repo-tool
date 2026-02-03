// Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUTF8.html

namespace Nemonuri.OCamlDotNet

/// UTF-8 encoded Unicode strings. The type is normal string.
module BatUTF8 =

    open type System.Text.Encoding

    /// UTF-8 encoded Unicode strings. The type is normal string.
    type t = System.ReadOnlySpan<byte>

    exception Malformed_code

    /// validate s successes if s is valid UTF-8, otherwise raises Malformed_code. 
    /// Other functions assume strings are valid UTF-8, so it is prudent to test their validity for strings from untrusted origins.
    //let validate (str: t) : unit =
        //UTF8.GetCharCount str |> ignore


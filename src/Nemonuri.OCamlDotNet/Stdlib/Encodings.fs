namespace Nemonuri.OCamlDotNet.Primitives

open System.Text

module Encodings =

    let utf8NoBom = UTF8Encoding(false,true)

    let tryToUtf8 ( enc: Encoding ) = match enc with | :? UTF8Encoding as u8 -> Some u8 | _ -> None

    let isUtf8 ( enc: Encoding ) = tryToUtf8 enc |> Option.isSome

    let isUtf8NoBom ( enc: Encoding ) =
        match tryToUtf8 enc with
        | None -> false
        | Some u8 -> u8.GetPreamble() |> Array.isEmpty
    
    

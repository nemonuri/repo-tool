namespace Nemonuri.OCamlDotNet.Primitives

open System.Text

module Encodings =

    let utf8NoBom = UTF8Encoding(false,true)

    let tryToUtf8 ( enc: Encoding ) = match enc with | :? UTF8Encoding as u8 -> ValueSome u8 | _ -> ValueNone

    let isUtf8 ( enc: Encoding ) = tryToUtf8 enc |> ValueOption.isSome

    let isUtf8NoBom ( enc: Encoding ) =
        match tryToUtf8 enc with
        | ValueNone -> false
        | ValueSome u8 -> u8.GetPreamble() |> Array.isEmpty
    
    

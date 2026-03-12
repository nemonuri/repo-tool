namespace Nemonuri.OCamlDotNet.Primitives

open System
open System.Buffers
open System.Buffers.Text
open Microsoft.FSharp.NativeInterop
open Nemonuri.Buffers
open Nemonuri.Transcodings
open Nemonuri.Transcodings.Utf8Encodings
module Bs = Nemonuri.OCamlDotNet.Primitives.ByteSpans


[<Struct>]
[<NoEquality; NoComparison>]
type OCamlFormat4<'TSource,'TState,'TResidue,'TResult> = internal {
    FormatBuilder: DrainableArrayBuilder<byte>;
    Writer: 'TSource -> 'TState -> 'TResidue;
    Flusher: 'TState -> 'TResidue -> 'TResult
}

type OCamlFormat<'TSource,'TBuffer,'TResult> = OCamlFormat4<'TSource,'TBuffer,'TResult,'TResult>

module Formats =

    type IntDecimal =
        struct
            interface IFormatterPremise<int,DefaultSize> with
                member _.StandardFormat = StandardFormat('D')
                member _.TryFormat (value: int, destination: Span<byte>, bytesWritten: byref<int>, format: StandardFormat): bool = 
                    Utf8Formatter.TryFormat(value, destination, &bytesWritten, format)
        end
    
    type EscapedChar =
        struct
            interface IFormatterPremise<OCamlChar,DefaultSize> with
                member _.StandardFormat = Unchecked.defaultof<_>
                member _.TryFormat (value: OCamlChar, destination: Span<byte>, bytesWritten: byref<int>, _: StandardFormat): bool = 
                    let tempStore = DotNetSpans.NativePtrToSpan(NativePtr.stackalloc<OCamlChar> Bs.escapeCharAndCountDestSize, Bs.escapeCharAndCountDestSize)
                    let stepCount = Bs.escapeCharAndCount tempStore value 
                    if stepCount <= 0 || destination.Length < stepCount then bytesWritten <- 0; false else
                    let stepSource = tempStore.Slice(0, stepCount)
                    let mutable placeHolder = 0
                    Unchecked.defaultof<Identity<OCamlChar>>.Transcode(stepSource, destination, &placeHolder, &bytesWritten) |> ignore
                    true
        end
    
    // type OCamlFormat4



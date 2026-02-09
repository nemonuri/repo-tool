#nowarn "9" // Unverifiable .NET IL code

namespace Nemonuri.OCamlDotNet

open System;
open Microsoft.FSharp.NativeInterop;

[<Struct; System.Runtime.CompilerServices.IsByRefLike>]
type SpanState<'T when 'T : unmanaged> = { span: Span<'T>; index: int }


//[<Struct; System.Runtime.CompilerServices.IsByRefLike>]
type SpanBuilder<'T when 'T : unmanaged> =

    [<CustomOperation>]
    member _.alloc (length: int, cont: SpanState<'T> -> SpanState<'T>) : SpanState<'T> =
        let ptr = NativePtr.stackalloc<'T> length |> NativePtr.toVoidPtr
        { span = Span<'T>(ptr, length); index = 0 }

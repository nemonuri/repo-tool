namespace Nemonuri.OCamlDotNet.Primitives

open System
open Microsoft.FSharp.NativeInterop

module DotNetArrays =

    let createUninitialized<'a> (n: int) =
#if NET8_0_OR_GREATER
        System.GC.AllocateUninitializedArray<'a>(length = n, pinned = false)
#else
        Array.create n Unchecked.defaultof<'a>
#endif

[<Sealed; AbstractClass>]
type DotNetSpans =
    static member inline WriteAndCount(dest: Span<'T>, s0: 'T) = dest[0] <- s0; 1
    static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T) = dest[1] <- s1; DotNetSpans.WriteAndCount(dest, s0)+1
    static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T, s2: 'T) = dest[2] <- s2; DotNetSpans.WriteAndCount(dest, s0, s1)+1
    static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T, s2: 'T, s3: 'T) = dest[3] <- s3; DotNetSpans.WriteAndCount(dest, s0, s1, s2)+1

    static member inline NativePtrToSpan(ptr: nativeptr<'T>, length: int) = Span<'T>(ptr |> NativePtr.toVoidPtr, length)

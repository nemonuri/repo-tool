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

module DotNetNativeInts =
    
    open Nemonuri.PureTypeSystems
    open Nemonuri.PureTypeSystems.Primitives
    open System.Runtime.CompilerServices
    module Rf = Nemonuri.PureTypeSystems.Refiners

    type private EqualSize<'l, 'r when 'l : unmanaged and 'r : unmanaged> =
        struct
            static member Judge (pre: inref<'l>, post: byref<'l>): Judgement =
                if sizeof<'l> = sizeof<nativeint> then    
                    post <- pre;
                    Judgement.True
                else
                    post <- Unchecked.defaultof<_>;
                    Judgement.False

            interface IRefinerPremise<'l> with
                member _.Judge (pre: inref<'l>, post: byref<'l>): Judgement = EqualSize<'l, 'r>.Judge(&pre, &post)
        end

    let private tryRefineOfEqualSize (handle: 'h) = Rf.tryRefineV<'h, EqualSize<'h, nativeint>>(handle)
    
    let private refinedToNativeInt (h: Refined<'h, EqualSize<'h, nativeint>>) : nativeint =
        let mutable h' = h.Value in
        let r = Unsafe.As<_,_>(&h') in
        r
    
    let toNativeInt (h: 'handle) = tryRefineOfEqualSize h |> ValueOption.get |> refinedToNativeInt

    let private tryRefineToEqualSize<'h when 'h : unmanaged>(n: nativeint) = Rf.tryRefineV<nativeint, EqualSize<nativeint, 'h>>(n) 
    
    let private ofRefinedNativeInt (n: Refined<nativeint, EqualSize<nativeint, 'h>>) : 'h =
        let mutable n' = n.Value in
        let r = Unsafe.As<_,_>(&n') in
        r
    
    let ofNativeInt (n: nativeint) = tryRefineToEqualSize n |> ValueOption.get |> ofRefinedNativeInt

module DotNetStreams = begin

    open System.IO
    open Unchecked
    open Nemonuri.PureTypeSystems
    open Nemonuri.PureTypeSystems.Primitives
    module Rf = Nemonuri.PureTypeSystems.Refiners

    type CanRead<'TStream when 'TStream :> Stream> = struct

        static member Judge (pre: inref<'TStream>, post: byref<'TStream>): Judgement = 
            match pre.CanRead with
            | true -> post <- pre; Judgement.True
            | false -> post <- defaultof<_>; Judgement.False

        interface IRefinerPremise<'TStream> with
            member _.Judge (pre, post): Judgement = CanRead.Judge(&pre, &post)
    end

    type CanWrite<'TStream when 'TStream :> Stream> = struct

        static member Judge (pre: inref<'TStream>, post: byref<'TStream>): Judgement = 
            match pre.CanWrite with
            | true -> post <- pre; Judgement.True
            | false -> post <- defaultof<_>; Judgement.False

        interface IRefinerPremise<'TStream> with
            member _.Judge (pre, post): Judgement = CanRead.Judge(&pre, &post)
    end

    let tryRefineToCanRead (s: 's) = Rf.tryRefineV<'s,CanRead<'s>> s

    let tryRefineToCanWrite (s: 's) = Rf.tryRefineV<'s,CanWrite<'s>> s

end

namespace Nemonuri.OCamlDotNet.Primitives.DotNetBasics

module ByteBufferWriters = begin

    open System
    open Nemonuri.Transcodings

    // <'bw when 'bw :> IBufferWriter<byte>>
    let write (bw: byref<'bw>) (rbs: ReadOnlySpan<byte>) =
        let mutable sourcesRead = 0 in
        TranscoderTheory.TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,'bw>(rbs, &bw, &sourcesRead);
        sourcesRead

end
namespace Nemonuri.OCamlDotNet.Primitives

open System
open TypeEquality
open System.Buffers
open System.Buffers.Text
open CommunityToolkit.Diagnostics
open Microsoft.FSharp.NativeInterop
open Nemonuri.Buffers
open Nemonuri.Transcodings
open type Nemonuri.Transcodings.TranscoderTheory
open Nemonuri.Transcodings.Utf8Encodings
open System.Runtime.CompilerServices
open Nemonuri.OCamlDotNet.Primitives.FormatBasics
module Bs = Nemonuri.OCamlDotNet.Primitives.ByteSpans
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources


[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormat<'TContext, 'TBuffer, 'TResult, 'TFinal> =
    private | OCamlFormat of LegacyFormat<'TContext, 'TBuffer, 'TResult> //* Fb.IFinalizer<'TBuffer, 'TResult, 'TFinal>

#if false
type IStateMonadPremise<'TState> =
    interface
        abstract member Return<'T> : 'T -> ('TState -> ('T * 'TState))

        abstract member Bind<'T1,'T2> : (('TState -> ('T1 * 'TState)) * ('T1 -> 'TState -> ('T2 * 'TState))) -> ('TState -> ('T2 * 'TState))
    end

type IFolderPremise<'TState> =
    interface
        abstract member Fold<'T> : 'TState -> 'T -> 'TState
    end

// OCamlFormatSegment<'TSource>

type IOCamlFormatter<'TBuffer> =
    interface
        abstract member Format<'TSource> : ('TBuffer * OCamlFormatSegment<'TSource> * 'TSource) -> 'TBuffer
    end
#endif

module Formats =

    let write (fin: IFinalizer<_,_,_>) env (OCamlFormat(lfmt)) ctx =
        let exnOpt =
            try
                Legacies.toWriterFactory lfmt env ctx; None
            with e -> 
                Some e
        in
        fin.Finalize(&env.BufferState, &env.ResultState, exnOpt)

    let empty = OCamlFormat(Legacies.empty)

    let cons hd (tl: OCamlFormat<_,_,_,_>) = 
        let (OCamlFormat(ltl)) = tl in
        OCamlFormat(Legacies.cons hd ltl)



#if false
    type IFinalizer<'TBuffer, 'TOk, 'TError> = Fb.IFinalizer<'TBuffer, 'TOk, ('TOk * 'TError)>

    type ToFinalizerPremise =
        struct
            static member ToFinalizer
        end



    type transcoder<'a> = TranscoderHandle<'a,byte,OCamlString>

    let toFormatter (tc: transcoder<'a>) = 
        assert ( sizeof<transcoder<'a>> = sizeof<nativeint> )
        let mutable tc' = tc in
        let ni = Unsafe.As<transcoder<'a>, nativeint>(&tc') in
        Guard.IsNotEqualTo(ni,0)
        OCamlFormatter (ni, typeof<'a>)
    
    let tryOfFormatter<'a> (OCamlFormatter (ni,st)) = 
        assert ( sizeof<transcoder<'a>> = sizeof<nativeint> )
        Guard.IsNotEqualTo(ni,0)
        if typeof<'a> <> st then ValueNone else
        let mutable ni' = ni in
        let tc = Unsafe.As<nativeint, transcoder<'a>>(&ni') in
        ValueSome tc


    let private teq_ofsl (p: Teq<'a,'b>) : Teq<OCamlFormatSegmentList<'a>,OCamlFormatSegmentList<'b>> = Teq.Cong.believeMe p

    let private coreToSegmentList (fc: OCamlFormatCore<'hd,'b,'tl>) : OCamlFormatSegmentList<'hd->'tl> =
        match fc with
        | OCamlFormatCore.Cons(_,fs) -> fs
        | OCamlFormatCore.Empty(_,hf,tf) -> 
            let emt : OCamlFormatSegmentList<'b> = Segments.empty<'b> in
            let proof0 = Teq.Cong.func hf tf in
            let proof1: Teq<OCamlFormatSegmentList<(unit -> 'b)>,OCamlFormatSegmentList<('hd -> 'tl)>> = teq_ofsl proof0 in
            Teq.cast proof1 emt


    let empty<'b when 'b :> IBufferWriter<byte>> : OCamlFormat<unit,'b,unit,'b> = 
        {
            Core = OCamlFormatCore.Empty((fun bws _ -> bws), Teq.refl<unit>, Teq.refl<'b>);
            Drainer = fun _ -> ();
        }

    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (Segments.Flatten(le, fo, tr, tc)) (s: 's) : 'b =
        let mutable b' = b in
        let mutable placeHolder: int = 0 in
        
        let inline writePlane src = TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,'b>(Obs.stringToReadOnlySpan src, &b', &placeHolder); 
        let inline writeFormatted src fmt = tc.TranscodeSingletonWhileDestinationTooSmall(src,&b',fmt,&placeHolder);

        writePlane le;
        writeFormatted s fo;
        writePlane tr;

        b'

    let private toWriter_core (ofc: OCamlFormatCore<'hd,'b,'tl>) : 'b -> 'hd -> 'tl =
        match ofc with
        | OCamlFormatCore.Empty(writer, hf, tf) -> 
            let proof0 = Teq.Cong.func hf tf in
            let proof1 = Teq.Cong.func Teq.refl proof0 in
            Teq.cast proof1 writer
        | OCamlFormatCore.Cons(writer,_) -> writer

    let private teq_ofsl (p: Teq<'a,'b>) : Teq<OCamlFormatSegmentList<'a>,OCamlFormatSegmentList<'b>> = Teq.Cong.believeMe p

    let private cons_core (fmtSeg: OCamlFormatSegment<'newHd>) (prev: OCamlFormat<'hd,'b,'r,'tl>) : OCamlFormatCore<'newHd,'b,'hd->'tl> =
        let prevCore: OCamlFormatCore<'hd,'b,'tl> = prev.Core in
        let accImpl = toWriter_core prevCore in
        let nextImpl bws (newHd: 'newHd) (hd: 'hd) =
            // last pushed, first written.
            let nextB = write_core bws fmtSeg newHd in
            accImpl nextB hd
        in
        match prevCore with
        | OCamlFormatCore.Cons(_,fs) -> 
            let nextFs = Segments.cons fmtSeg fs in
            OCamlFormatCore.Cons(nextImpl, nextFs)
        | OCamlFormatCore.Empty(_,hf,tf) ->
            let emt = Segments.empty<unit -> 'b> in
            let proof0 = teq_ofsl tf in
            let emt0: OCamlFormatSegmentList<'tl> = Teq.cast proof0 emt in
            let nextFs = Segments.cons fmtSeg emt0


    let cons (fmtSeg: OCamlFormatSegment<'newHd>) (fmt: OCamlFormat<'hd,'b,'r,'tl>) =
        let nextCore = cons_core fmt.Core fmtSeg in
        { Core = nextCore; Drainer = fmt.Drainer }
            
    let toWriter (fmt: OCamlFormat<'hd,'b,'r,'tl>) = fmt.Core |> toWriter_core

    let tryGetHeadSegment (fmt: OCamlFormat<'hd,'b,'r,'tl>) =
        match fmt.Core with
        | OCamlFormatCore.Empty(_,_,_) -> ValueNone
        | OCamlFormatCore.Cons(_,fs) -> ValueSome fs


        
    [<Struct>]
    [<NoEquality; NoComparison>]
    [<RequireQualifiedAccess>]
    type OCamlFormat4<'TSources,'TDest,'TOk,'TError> =  
        internal
        | OCamlFormat4 of ('TDest -> (Result<'TOk,'TError> ref) -> 'TSources)

    type OCamlFormat<'TSource,'TBuffer,'TResult> = OCamlFormat4<'TSource,'TBuffer,'TResult,'TResult>


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

[<NoEquality; NoComparison>]
type OCamlFormat<'TWriter, 'TBufferSource, 'TResult> =
    internal
    | OCamlFormat of 
        factory: ScopedFormats.IBufferFactory<'TBufferSource> * 
        disposer: ScopedFormats.IBufferDisposer<'TResult> *
        scopedFormat: OCamlScopedFormat<'TWriter, 
#endif

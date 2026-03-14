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
module Bs = Nemonuri.OCamlDotNet.Primitives.ByteSpans
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources


[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormatSegmentString = | OCamlFormatSegmentString of leading:OCamlString * format:OCamlString * trailing:OCamlString

[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormatSegment<'TSource> =
    | OCamlFormatSegment of string:OCamlFormatSegmentString * transcoder:TranscoderHandle<'TSource,byte,OCamlString>


[<NoEquality; NoComparison>]
[<RequireQualifiedAccess>]
type internal OCamlFormatCore<'THead, 'TBuffer, 'TTail when 'TBuffer :> IBufferWriter<byte>> =  
    private
    | Empty of writer:('TBuffer -> unit -> 'TBuffer) * headTypeProof:Teq<unit,'THead> * tailTypeProof:Teq<'TBuffer,'TTail>
    | Cons of writer:('TBuffer -> 'THead -> 'TTail) * formatSegment:OCamlFormatSegment<'THead>

[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormat<'THead, 'TBuffer, 'TResult, 'TTail when 'TBuffer :> IBufferWriter<byte>> =
    internal {
        Core: OCamlFormatCore<'THead, 'TBuffer, 'TTail>;
        Drainer: 'TBuffer -> 'TResult
    }

module Formats =

    [<RequireQualifiedAccess>]
    module Segments = begin

        open Microsoft.FSharp.NativeInterop
        open System.Runtime.CompilerServices
        open Nemonuri.PureTypeSystems.Primitives
        open Nemonuri.PureTypeSystems.TypeShadowing
        open DotNetNativeInts

        [<NoEquality; NoComparison>]
        [<Struct>]
        type private Data = | Data of OCamlFormatSegmentString * transcoder:nativeint * tailExtractor:nativeint
        type private datas = list<Data>

        [<NoEquality; NoComparison>]
        [<Struct>]
        type private CurriedExtractor<'TContext> = | CurriedExtractor of nativeint

        type private TranscoderHandle<'TSource> = TranscoderHandle<'TSource,byte,OCamlString>

        let private nativeIntOfTranscoderHandle (tch: TranscoderHandle<'s>) : nativeint =
            let mutable tch' = tch in
            let r = Unsafe.As<_,_>(&tch') in
            r

        let (|Flatten|) (OCamlFormatSegment(OCamlFormatSegmentString(le, fo, tr), tc)) = le,fo,tr,tc

        let nullSegment: OCamlFormatSegment<invalid> = Unchecked.defaultof<_>

        let inline (|NullSegment|HasValueSegment|) (fs: OCamlFormatSegment<'s>) =
            match fs with 
            | Flatten(_,_,_,tc) -> 
            match tc.HasValue with
            | true -> HasValueSegment fs
            | false -> NullSegment nullSegment

        [<NoEquality; NoComparison>]
        [<Struct>]
        type SegmentList<'ctx> = private { Extractor: CurriedExtractor<'ctx>; Datas: datas }

        type private extData<'s> = System.ValueTuple<OCamlFormatSegmentString, TranscoderHandle<'s>>
        type private extResult<'s,'tctx> = System.ValueTuple<extData<'s>, SegmentList<'tctx>>

        let private unsafeApply<'ctx,'s,'tctx> (tf: CurriedExtractor<'ctx>) : CurriedTypeFuncHandle<'s, SegmentList<'s -> 'tctx>, extResult<'s,'tctx>> =
            let mutable tf' = match tf with | CurriedExtractor v -> v in
            let r = Unsafe.As<_,_>(&tf') in
            r

        let empty : SegmentList<invalid> = { Extractor = CurriedExtractor 0; Datas = [] }

        type private ExtractorPremise<'s,'tctx> =
            struct
                interface ICurriedTypeFuncPremise<'s, SegmentList<'s -> 'tctx>> with
                    member _.Invoke (state: SegmentList<'s -> 'tctx>): 'T = 
                        match typeof<extResult<'s,'tctx>> = typeof<'T> with
                        | false -> invalidOp $"Invalid type. Expected = {typeof<extResult<'s,'tctx>>}, Actual = {typeof<'T>}"
                        | true ->
                        match state.Datas with
                        | [] -> invalidOp $"Invalid deconstruction. {nameof(state)} is empty."
                        | (Data(ofss, tc, te))::tailDatas ->
                            let handle: TranscoderHandle<'s> = ofNativeInt tc in
                            let extData : extData<'s> = struct ( ofss, handle ) in
                            let tail : SegmentList<'tctx> = { Extractor = CurriedExtractor te; Datas = tailDatas } in
                            let result : extResult<'s,'tctx> = struct ( extData, tail ) in
                            result |> unbox
            end

        let cons (hd: OCamlFormatSegment<'s>) (tl: SegmentList<'ctx>) : SegmentList<'s -> 'ctx> = 
            let headExtractor: CurriedTypeFuncHandle<'s,SegmentList<'s -> 'ctx>,extResult<'s,'ctx>> = CurriedTypeFuncTheory.ToHandle<'s,SegmentList<'s->'ctx>,ExtractorPremise<'s,'ctx>,extResult<'s,'ctx>>() in
            let (OCamlFormatSegment(ofss: OCamlFormatSegmentString, tch: TranscoderHandle<'s>)) = hd in
            let headData = Data(ofss, toNativeInt tch, toNativeInt tl.Extractor) in
            { Extractor = CurriedExtractor (toNativeInt headExtractor); Datas = headData::(tl.Datas) }

        let decons (l: SegmentList<'s -> 'ctx>) : System.ValueTuple<OCamlFormatSegment<'s>, SegmentList<'ctx>> =
            let (CurriedExtractor n) = l.Extractor in
            let ext : CurriedTypeFuncHandle<'s,SegmentList<'s -> 'ctx>,extResult<'s,'ctx>> = ofNativeInt n in
            let (struct (struct (ofss: OCamlFormatSegmentString, tch: TranscoderHandle<'s>), tl: SegmentList<'ctx>)) = ext.Invoke(l) in
            let hd = OCamlFormatSegment(ofss, tch) in
            struct ( hd, tl )

        type TryDeconsPremise =
            struct
                static member TryDecons(l: SegmentList<invalid>) = ValueNone
                static member TryDecons<'T1,'T2>(l: SegmentList<'T1 -> 'T2>) = ValueSome (decons l)
            end

        let inline (|Nil|Cons|) l =
            let inline call (p: ^p) (l': ^l) = ((^p or ^l) : (static member TryDecons: ^l -> _) l') in
            let r = call Unchecked.defaultof<TryDeconsPremise> l in
            match r with
            | ValueNone -> Nil
            | ValueSome (struct (hd, tl)) -> Cons (hd, tl)
        
        type IListFolder<'TState> =
            interface
                abstract member Step<'T> : 'TState -> OCamlFormatSegment<'T> -> 'TState
            end

        let rec inline fold (folder: #IListFolder<'state>) (seed: 'state) l =
            match l with
            | Nil -> seed
            | Cons (hd, tl) ->
                let nextSeed = folder.Step<_> seed hd in
                fold folder nextSeed tl


        


        
        

    end



#if false
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
#endif
    
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

    let private cons_core (prev: OCamlFormatCore<'hd,'b,'tl>) (fmtSeg: OCamlFormatSegment<'newHd>) : OCamlFormatCore<'newHd,'b,'hd->'tl> =
        let accImpl = toWriter_core prev in
        let nextImpl bws (newHd: 'newHd) (hd: 'hd) =
            // last pushed, first written.
            let nextB = write_core bws fmtSeg newHd in
            accImpl nextB hd
        in
        OCamlFormatCore.Cons (nextImpl,fmtSeg)

    let cons (fmtSeg: OCamlFormatSegment<'newHd>) (fmt: OCamlFormat<'hd,'b,'r,'tl>) =
        let nextCore = cons_core fmt.Core fmtSeg in
        { Core = nextCore; Drainer = fmt.Drainer }
            
    let toWriter (fmt: OCamlFormat<'hd,'b,'r,'tl>) = fmt.Core |> toWriter_core

    let tryGetHeadSegment (fmt: OCamlFormat<'hd,'b,'r,'tl>) =
        match fmt.Core with
        | OCamlFormatCore.Empty(_,_,_) -> ValueNone
        | OCamlFormatCore.Cons(_,fs) -> ValueSome fs


        

#if false
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

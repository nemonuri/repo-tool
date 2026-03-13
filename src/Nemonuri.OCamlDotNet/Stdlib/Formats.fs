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
type OCamlFormatSegment<'TSource> =
    | OCamlFormatSegment of leading:OCamlString * format:OCamlString * trailing:OCamlString * transcoder:TranscoderHandle<'TSource,byte,OCamlString>


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

        open Nemonuri.PureTypeSystems.TypeShadowing

        let nullSegment: OCamlFormatSegment<invalid> = Unchecked.defaultof<_>

        let inline (|NullSegment|HasValueSegment|) (fs: OCamlFormatSegment<'s>) =
            match fs with 
            | OCamlFormatSegment(_,_,_,tc) -> 
            match tc.HasValue with
            | true -> HasValueSegment fs
            | false -> NullSegment nullSegment

        [<NoEquality; NoComparison>]
        type List<'hd, 'tl> = 
            private
            | Empty of headProof:Teq<'hd, invalid> * tailProof:Teq<'tl, invalid>
            | Cons of head:OCamlFormatSegment<'hd> * tail:ITail<'tl>
            with
                interface ITail<List<'hd, 'tl>> with
                    member x.Value = x
            end
        and private ITail<'tl> =
            interface
                abstract member Value: 'tl
            end

        let empty : List<invalid, invalid> = Empty (Teq.refl, Teq.refl)

        let cons (nhd: OCamlFormatSegment<'nhd>) (tl: List<'hd, 'tl>) : List<'nhd,List<'hd,'tl>> = Cons (nhd, tl)

        


        
        

    end

    let inline (|NullFormatSegment|HasValueFormatSegment|) (fs: OCamlFormatSegment<'s>) =
        match fs with 
        | OCamlFormatSegment(_,_,_,tc) -> 
        match tc.HasValue with
        | true -> HasValueFormatSegment fs
        | false -> NullFormatSegment

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

    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (OCamlFormatSegment(le, fo, tr, tc)) (s: 's) : 'b =
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

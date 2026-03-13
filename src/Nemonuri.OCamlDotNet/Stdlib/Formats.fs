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

type OCamlFormatter = internal | OCamlFormatter of fp:nativeint * sourceType:System.Type


[<Struct>]
type OCamlFormatStringSegment = 
    | OCamlFormatStringSegment of leading:OCamlString * format:OCamlString * trailing:OCamlString




[<NoEquality; NoComparison>]
[<RequireQualifiedAccess>]
type internal OCamlFormatCore<'THead, 'TTail> =  
    private
    | Empty of writer:(BufferWriterShim -> unit -> BufferWriterShim) * headTypeProof:Teq<unit,'THead> * tailTypeProof:Teq<BufferWriterShim,'TTail>
    | Cons of writer:(BufferWriterShim -> 'THead -> 'TTail)

[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormat<'THead, 'TBuffer, 'TResult, 'TTail> =  
    internal {
        Factory: 'TBuffer -> BufferWriterShim;
        Core: OCamlFormatCore<'THead, 'TTail>;
        Disposer: BufferWriterShim -> 'TResult;
    }

module Formats =

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
    
    let init (factory: 'b -> BufferWriterShim) (disposer: BufferWriterShim -> 'r) : OCamlFormat<unit,'b,'r,BufferWriterShim> = 
        {
            Factory = factory;
            Core = OCamlFormatCore.Empty((fun bws _ -> bws), Teq.refl<unit>, Teq.refl<BufferWriterShim>)
            Disposer = disposer;
        }
    

    let emptyOfOutChannel = 
        let factory (oc: OCamlOutChannel) = oc.FileDescriptor |> OCamlFileDescriptors.Writers.toBufferWriterShim in
        let disposer shim =
            match shim with
            | BufferWriterShim.BinaryWriterWithPool b -> b.Dispose()
            | BufferWriterShim.StreamWithByteArrayPool b -> b.Dispose()
            | BufferWriterShim.Boxed b -> ()
        init factory disposer

    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (ofsl: OCamlFormatStringSegment) (s: 's) (tc: transcoder<'s>) : 'b =
        let mutable b' = b in
        let mutable placeHolder: int = 0 in
        
        let inline writePlane src = TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,'b>(Obs.stringToReadOnlySpan src, &b', &placeHolder); 
        let inline writeFormatted src fmt = tc.TranscodeSingletonWhileDestinationTooSmall(src,&b',fmt,&placeHolder);

        match ofsl with
        | OCamlFormatStringSegment(le, fo, tr) ->
            writePlane le;
            writeFormatted s fo;
            writePlane tr;
        
        b'

    let internal extractWriter (ofc: OCamlFormatCore<'hd,'tl>) : BufferWriterShim -> 'hd -> 'tl =
        match ofc with
        | OCamlFormatCore.Empty(writer, hf, tf) -> 
            let proof0 = Teq.Cong.func hf tf in
            let proof1 = Teq.Cong.func Teq.refl proof0 in
            Teq.cast proof1 writer
        | OCamlFormatCore.Cons writer -> writer

    let private cons_core (prev: OCamlFormatCore<'hd,'tl>) (ofsl: OCamlFormatStringSegment) (tc: transcoder<'newHd>) : OCamlFormatCore<'newHd,'hd->'tl> =
        let accImpl = extractWriter prev in
        let nextImpl bws (newHd: 'newHd) (hd: 'hd) =
            // last pushed, first written.
            let nextB = write_core bws ofsl newHd tc in
            accImpl nextB hd
        in
        OCamlFormatCore.Cons nextImpl
    
    let cons (prev: OCamlFormat<'hd,'b,'r,'tl>) (ofsl: OCamlFormatStringSegment, tc: transcoder<'newHd>) =
        let nextCore = cons_core prev.Core ofsl tc in
        { Factory = prev.Factory; Core = nextCore; Disposer = prev.Disposer }
            
    type internal IBufferFactory<'TBufferSource> =
        interface
            abstract member TryCreate<'TBuffer when 'TBuffer :> IBufferWriter<byte>> : source:'TBufferSource -> voption<'TBuffer>
        end
        
    type internal IBufferDisposer<'TResult> =
        interface
            abstract member FlushAndDispose<'TBuffer when 'TBuffer :> IBufferWriter<byte>> : buffer:byref<'TBuffer> -> 'TResult
        end

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

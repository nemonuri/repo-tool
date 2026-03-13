namespace Nemonuri.OCamlDotNet.Primitives

open System
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
| InvalidSegemnt
| PlaneSegemnt of s:OCamlString
| FormatSegment of s:OCamlString * formatter:OCamlFormatter


[<NoEquality; NoComparison>]
type OCamlScopedFormat<'TWriter, 'TBuffer when 'TBuffer :> IBufferWriter<byte>> =  
    internal
    | OCamlScopedFormat of (ref<'TBuffer> -> 'TWriter)






module ScopedFormats =

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
    
    let empty : OCamlScopedFormat<unit,'b> = 
        OCamlScopedFormat (fun _ -> ())
    
(*
    type private Aux =

        static member WriteCoreAux<'b, 's when 'b :> IBufferWriter<byte>>(b: byref<'b>, ph: byref<int>, ofsl: list<OCamlFormatStringSegment>, s: 's) : (list<OCamlFormatStringSegment> * bool) =
            match ofsl with
            
*)


    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (ofsl: list<OCamlFormatStringSegment>) (s: 's) : 'b =
        let mutable b' = b in
        let mutable placeHolder: int = 0 in
        let rec aux stepOfsl sourceConsumed =
            match stepOfsl with
            | [] -> [], sourceConsumed
            | ofs::nextOfsl ->
            match ofs with
            | InvalidSegemnt -> aux nextOfsl sourceConsumed (* Temporarily, ignore *)
            | PlaneSegemnt str -> 
                TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,'b>(Obs.stringToReadOnlySpan str, &b', &placeHolder); 
                aux nextOfsl sourceConsumed
            | FormatSegment (str, fmtr) ->
                if sourceConsumed then 
                    aux nextOfsl sourceConsumed (* Temporarily, ignore *)
                else
                match tryOfFormatter<'s> fmtr with
                | ValueNone -> aux nextOfsl sourceConsumed (* Temporarily, ignore *)
                | ValueSome tc -> 
                    tc.TranscodeSingletonWhileDestinationTooSmall(s,&b',str,&placeHolder);
                    aux nextOfsl true
        in
        aux ofsl false |> ignore (* Temporarily, ignore *)
        b'

    let cons (prev: OCamlScopedFormat<'s,'b>) (ofsl: list<OCamlFormatStringSegment>) : OCamlScopedFormat<'ns -> 's,'b> =
        let (OCamlScopedFormat accImpl) = prev in
        let nextImpl (refB: ref<'b>) (ns: 'ns) =
            // last pushed, first written.
            refB.Value <- write_core refB.Value ofsl ns
            accImpl refB
        in
        OCamlScopedFormat nextImpl
            
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

module Hmm =

    let goal1_typeChange
        (factory: 'TBufferSource -> 'TBuffer) 
        (scopedFormat: ref<'TBuffer> -> 'TWriter) 
        (sourceRef: ref<'TBufferSource>) 
        : 'TWriter // * (unit -> 'TResult) 
        =
        raise (NotImplementedException())

    let goal2_disposerThunk
        (disposer: 'TBuffer -> 'TResult)
        (scopedFormatResult: ref<'TBuffer>)
        (_ :unit) 
        : 'TWriter
        =
        raise (NotImplementedException())
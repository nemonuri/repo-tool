namespace Nemonuri.OCamlDotNet.Primitives.FormatBasics

open System
open System.Buffers
open Nemonuri.Transcodings
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.OCamlDotNet.Primitives.DotNetNativeInts
open type Nemonuri.Transcodings.TranscoderTheory
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources


type TranscoderHandle<'TSource> = TranscoderHandle<'TSource,byte,OCamlString>


module Transcoders = begin

    open Nemonuri.FixedSizes
    open Nemonuri.Transcodings.Utf8Encodings
    open Microsoft.FSharp.Core.Operators.Unchecked

    type private IUtf8Formatter<'s> = IFormatterPremise<'s, OCamlString>
    type private U8 = System.Buffers.Text.Utf8Formatter

    let private stringEqual (o: OCamlString) (bytes: byte array) = Obs.stringEqual o (Obs.Unsafe.stringOfArray bytes)
    let private (|L|_|) (bytes: byte array) (o: OCamlString) = stringEqual o bytes

    let ofUtf8Formatter (_: TypeHint<'src * 'fmt * 'len>) =
        ToHandle<'src, byte, OCamlString, FormatterBasedTranscoder<'src, OCamlString, 'fmt, 'len>>()

    [<RequireQualifiedAccess>]
    module Utf8Formatters = begin

        type private Sf = StandardFormat

        let toStandardFormat o =
            match o with
            | (L "I"B) -> Sf('I')
            | (L "D"B) | (L "d"B) -> Sf('D')
            | (L "N"B) | (L "n"B) -> Sf('N')
            | (L "X"B) | (L "x"B) -> Sf('X')
            | _ -> Sf()

        type private ByteChar = struct // Identity
            interface IUtf8Formatter<byte> with
                member _.TryFormat (value: byte, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    if destination.Length > 0 then
                        destination[0] <- value; true
                    else
                        false
        end

        type private ByteCharLen = struct
            interface IFixedSizePremise with member _.FixedSize = 1
        end

        let ofChar = ofUtf8Formatter defaultof<TypeHint<byte * ByteChar * ByteCharLen>>

        type private Bool = struct

            interface IUtf8Formatter<bool> with
                member _.TryFormat (value: bool, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    U8.TryFormat(value, destination, &bytesWritten, toStandardFormat format)
        end

        type private BoolLen = struct
            interface IFixedSizePremise with member _.FixedSize = 8
        end
        
        let ofBool = ofUtf8Formatter defaultof<TypeHint<bool * Bool * BoolLen>>

        type private Int = struct

            interface IUtf8Formatter<int> with
                member _.TryFormat (value: int, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    U8.TryFormat (value, destination, &bytesWritten, toStandardFormat format)
        end

        type private IntLen = struct
            interface IFixedSizePremise with member _.FixedSize = 16
        end

        let ofInt = ofUtf8Formatter defaultof<TypeHint<int * Int * IntLen>>

        type private Int64 = struct

            interface IUtf8Formatter<int64> with
                member _.TryFormat (value: int64, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    U8.TryFormat (value, destination, &bytesWritten, toStandardFormat format)
        end

        type private Int64Len = struct
            interface IFixedSizePremise with member _.FixedSize = 32
        end

        let ofInt64 = ofUtf8Formatter defaultof<TypeHint<int64 * Int64 * Int64Len>>

    end

end


[<NoEquality; NoComparison>]
[<Struct>]
type SegmentString = | SegmentString of leading:OCamlString * format:OCamlString * trailing:OCamlString


[<NoEquality; NoComparison>]
[<Struct>]
type Segment<'TSource> = | Segment of string:SegmentString * transcoder:TranscoderHandle<'TSource>


module Segments = begin

    open Transcoders
    module U = Utf8Formatters

    let (|FlattenSegment|) (Segment(SegmentString(le, fo, tr), tc)) = le,fo,tr,tc

    let private b2f format = SegmentString(Obs.emptyString, Obs.Unsafe.stringOfArray format, Obs.emptyString)

    let ``%I``  = Segment(b2f "I"B, U.ofBool)

    let ``%d``= Segment(b2f "d"B, U.ofInt)

    let ``%ld``= ``%d``

    let ``%Ld``= Segment(b2f "d"B, U.ofInt)

end

[<RequireQualifiedAccess>]
module private SegmentListBasics =

    [<NoEquality; NoComparison>]
    [<Struct>]
    type Decons<'TContext> = | Decons of nativeint

    [<NoEquality; NoComparison>]
    [<Struct>]
    type Data = | Data of SegmentString * transcoder:nativeint

    type Datas = list<Data>


module L = SegmentListBasics

[<NoEquality; NoComparison>]
[<Struct>]
type SegmentList<'TContext> = private | SegmentList of L.Datas * L.Decons<'TContext>

module SegmentLists = begin

    let empty : SegmentList<unit> = SegmentList([], L.Decons(0))

    [<NoEquality; NoComparison>]
    [<Struct>]
    type private DeconsResult<'THead,'TTail> = | DeconsResult of Segment<'THead> * SegmentList<'TTail>

    type private DeconsPremise<'hd,'tl> =
        struct
            interface ICurriedTypeFuncPremise<'hd, SegmentList<'hd -> 'tl>> with
                member _.Invoke (state: SegmentList<'hd -> 'tl>): 'T = 
                    match typeof<DeconsResult<'hd,'tl>> = typeof<'T> with
                    | false -> invalidOp $"Invalid type. Expected = {typeof<DeconsResult<'hd,'tl>>}, Actual = {typeof<'T>}"
                    | true ->
                    let (SegmentList(datas, L.Decons dcn)) = state in
                    match datas with
                    | [] -> invalidOp $"Invalid deconstruction. {nameof(state)} is empty."
                    | L.Data(ss, tcn)::dtl ->
                        let tch: TranscoderHandle<'hd> = ofNativeInt tcn in
                        let dch: L.Decons<'tl> = L.Decons dcn in
                        let result: DeconsResult<'hd,'tl> = DeconsResult(Segment(ss,tch), SegmentList(dtl,dch)) in
                        result |> unbox
        end
    
    type private DeconsHandle<'hd,'tl> = CurriedTypeFuncHandle<'hd,SegmentList<'hd->'tl>,DeconsResult<'hd,'tl>>

    let cons (hd: Segment<'hd>) (tl: SegmentList<'tl>) : SegmentList<'hd -> 'tl> = 
        let dch : DeconsHandle<'hd,'tl> = CurriedTypeFuncTheory.ToHandle<'hd,SegmentList<'hd->'tl>,DeconsPremise<'hd,'tl>,DeconsResult<'hd,'tl>>() in
        let (Segment(ss, tch)) = hd in
        let (SegmentList(datas, _)) = tl in
        SegmentList(L.Data(ss, toNativeInt tch)::datas, L.Decons(toNativeInt dch))

    let decons (l: SegmentList<'hd -> 'tl>) =
        let (SegmentList(_, L.Decons dcn)) = l in
        let dch : DeconsHandle<'hd,'tl> = ofNativeInt dcn in
        let (DeconsResult(hd, tl)) = dch.Invoke(l) in
        hd, tl

    type TryDeconsPremise =
        struct
            static member TryDecons(_: SegmentList<unit>) = None

            static member TryDecons<'THead,'TList>(l: SegmentList<'THead -> 'TList>) = Some (decons l)
        end

    let inline (|Nil|Cons|) l =
        let inline call (p: ^p) (l': ^l) = ((^p or ^l) : (static member TryDecons: _ -> _) l') in
        let r = (call Unchecked.defaultof<TryDeconsPremise> l) in
        match r with
        | None -> Nil
        | Some (hd,tl) -> Cons (hd, tl)

end

type IFormatter<'TBuffer> =
    interface
        abstract member Format<'TSource> : 'TBuffer -> Segment<'TSource> -> 'TSource -> 'TBuffer
    end

[<NoEquality; NoComparison>]
[<Struct>]
type WriterFactoryConfig<'TBuffer, 'TResult> = {
    BufferWriter: IFormatter<'TBuffer> voption;
    ResultWriter: IFormatter<'TResult> voption;
}

[<NoEquality; NoComparison>]
type WriterFactoryEnvironment<'TBuffer, 'TResult> = {
    Config: WriterFactoryConfig<'TBuffer, 'TResult>;
    mutable BufferState: 'TBuffer;
    mutable ResultState: 'TResult;
}

[<NoEquality; NoComparison>]
[<Struct>]
type LegacyFormat<'TContext, 'TBuffer, 'TResult> =
    private
    | LegacyFormat of 
        segments: SegmentList<'TContext> * 
        writerFactory: (WriterFactoryEnvironment<'TBuffer, 'TResult> -> 'TContext) (* '타입'을 맞추기 위해, TBuffer 와 TResult 를 뒤로 뺐다. *)


module Legacies = begin

    module S = SegmentLists

    let empty : LegacyFormat<unit,'b,'r> = LegacyFormat(S.empty, (fun _ -> ()))

    let private format (f: IFormatter<'b> voption) (b: byref<'b>) hd (source: 's) =
        match f with
        | ValueNone -> ()
        | ValueSome f -> b <- f.Format b hd source

    let cons (hd: Segment<'hd>) (tl: LegacyFormat<'tl,'b,'r>) : LegacyFormat<'hd -> 'tl,'b,'r> =
        match tl with
        | LegacyFormat(slTail, factoryTail) ->
            let nextSl = S.cons hd slTail in
            let nextFactory (env: WriterFactoryEnvironment<'b,'r>) (source: 'hd) : 'tl =
                format env.Config.BufferWriter &env.BufferState hd source;
                format env.Config.ResultWriter &env.ResultState hd source;
                factoryTail env
            in
            LegacyFormat(nextSl, nextFactory)

    // decons 는...없음!

    let toWriterFactory (LegacyFormat(_, factory)) = factory

end

module WriterFactories = begin

    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (Segments.FlattenSegment(le, fo, tr, tc)) (s: 's) : 'b =
        let mutable b' = b in
        let mutable placeHolder: int = 0 in
        
        let inline writePlane src = TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,'b>(Obs.stringToReadOnlySpan src, &b', &placeHolder); 
        let inline writeFormatted src fmt = tc.TranscodeSingletonWhileDestinationTooSmall(src,&b',fmt,&placeHolder);

        writePlane le;
        writeFormatted s fo;
        writePlane tr;

        b'
    
    type private BufferWriterBasedFormatter<'b when 'b :> IBufferWriter<byte>>() =
        class
            static member Instance = Bw<'b>()

            static member Format<'s>(buffer: 'b) (segment: Segment<'s>) (source: 's) : 'b = write_core<'b,'s> buffer segment source
                

            interface IFormatter<'b> with

                member _.Format<'s> b se so = Bw<'b>.Format<'s> b se so
        end
    and private Bw<'b when 'b :> IBufferWriter<byte>> = BufferWriterBasedFormatter<'b>


    let bufferWriterToFormatter (_: 'b) : IFormatter<'b> = Bw<'b>.Instance

    type TryToFormatterPremise =
        struct
            static member TryToFormatter(_ :unit) = ValueNone

            static member TryToFormatter<'TBuffer when 'TBuffer :> IBufferWriter<byte>>(b: 'TBuffer) = ValueSome (bufferWriterToFormatter b)
        end

    let inline tryToFormatter b =
        let inline call (p: ^p) (b': ^b) = ((^p or ^b) : (static member TryToFormatter: _->_) b') in
        call Unchecked.defaultof<TryToFormatterPremise> b

    let inline toConfig b r = { BufferWriter = tryToFormatter b; ResultWriter = tryToFormatter r }

    let toEnv b r (config: WriterFactoryConfig<'b,'r>) = { Config = config; BufferState = b; ResultState = r }
        
end

type IFinalizer<'TBuffer, 'TResult, 'TFinal> =
    interface
        abstract member Finalize: byref<'TBuffer> * byref<'TResult> * exn option -> 'TFinal
    end

module internal Finalizers = begin

    open Unchecked

    type ITapPremise<'T> = 
        interface
            abstract member Tap: byref<'T> -> unit
        end
    
    type NoneTap<'T> = 
        struct
            static member Tap (arg: byref<'T>) = ()

            interface ITapPremise<'T> with
                member _.Tap (arg: byref<'T>): unit = NoneTap<'T>.Tap(&arg)
        end
    
    type DisposeTap<'T when 'T :> System.IDisposable> =
        struct
            static member Tap (arg: byref<'T>) = arg.Dispose()

            interface ITapPremise<'T> with
                member _.Tap (arg: byref<'T>): unit = DisposeTap<'T>.Tap(&arg)
        end

    type FlushTap<'T when 'T :> Nemonuri.Buffers.IFlushable> =
        struct
            static member Tap (arg: byref<'T>) = arg.Flush()

            interface ITapPremise<'T> with
                member _.Tap (arg: byref<'T>): unit = FlushTap<'T>.Tap(&arg)
        end

    type TapFinalizer<'b,'r,'bt,'rt 
                        when 'bt :> ITapPremise<'b> 
                        and 'bt : unmanaged
                        and 'rt :> ITapPremise<'r>
                        and 'rt : unmanaged>() =
        class
            static member Instance = TapFinalizer<'b,'r,'bt,'rt>()

            static member Finalize(b: byref<'b>, r: byref<'r>, e: exn option): 'r = defaultof<'bt>.Tap(&b); defaultof<'rt>.Tap(&r); r

            interface IFinalizer<'b,'r,'r> with
                member _.Finalize (b: byref<'b>, r: byref<'r>, e: exn option): 'r = TapFinalizer<'b,'r,'bt,'rt>.Finalize(&b,&r,e)
        end

end


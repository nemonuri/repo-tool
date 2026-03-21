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
    type private Cc = Nemonuri.ByteChars.ByteCharConstants

    let private stringEqual (o: OCamlString) (bytes: byte array) = Obs.stringEqual o (Obs.Unsafe.stringOfArray bytes)
    let private (|L|_|) (bytes: byte array) (o: OCamlString) = stringEqual o bytes

    let ofFixedSizeUtf8Formatter (_: TypeHint<'src * 'fmt * 'fix>) =
        ToHandle<'src, byte, OCamlString, FormatterBasedFixedSizeTranscoder<'src, OCamlString, 'fmt, 'fix>>()

    let ofUtf8Formatter (_: TypeHint<'src * 'fmt * 'len>) =
        ToHandle<'src, byte, OCamlString, FormatterBasedTranscoder<'src, OCamlString, 'fmt, 'len>>()

    [<RequireQualifiedAccess>]
    module Utf8Formatters = begin

        type private Sf = StandardFormat

        let toStandardFormat o =
            match o with
            | (L "B"B) -> Sf('I')
            | (L "d"B) | (L "ld"B) | (L "Ld"B) | (L "nd"B) -> Sf('D')
            | (L "x"B) -> Sf('X')
            | _ -> Sf()

        type private Unit<'T> = struct

            static member TryFormat (value: 'T, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = bytesWritten <- 0; true

            interface IUtf8Formatter<'T> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    Unit<'T>.TryFormat(value, destination, format, &bytesWritten)

            interface IFixedSizePremise with
                member _.FixedSize = 0
        end

        let ofUnit<'u> = ofFixedSizeUtf8Formatter defaultof<TypeHint<'u * Unit<'u> * Unit<'u>>>

        type private Bool = struct

            static member TryFormat (value: bool, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                U8.TryFormat(value, destination, &bytesWritten, toStandardFormat format)

            interface IUtf8Formatter<bool> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    Bool.TryFormat(value, destination, format, &bytesWritten)
        end

        type private BoolLen = struct
            interface IFixedSizePremise with member _.FixedSize = 8
        end
        
        let ofBool = ofFixedSizeUtf8Formatter defaultof<TypeHint<bool * Bool * BoolLen>>

        type private Int = struct

            static member TryFormat (value: int, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    U8.TryFormat (value, destination, &bytesWritten, toStandardFormat format)

            interface IUtf8Formatter<int> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    Int.TryFormat(value, destination, format, &bytesWritten)
        end

        type private IntLen = struct
            interface IFixedSizePremise with member _.FixedSize = 16
        end

        let ofInt = ofFixedSizeUtf8Formatter defaultof<TypeHint<int * Int * IntLen>>

        type private Int64 = struct

            static member TryFormat (value: int64, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                    U8.TryFormat (value, destination, &bytesWritten, toStandardFormat format)


            interface IUtf8Formatter<int64> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    Int64.TryFormat(value, destination, format, &bytesWritten)
        end

        type private Int64Len = struct
            interface IFixedSizePremise with member _.FixedSize = 32
        end

        let ofInt64 = ofFixedSizeUtf8Formatter defaultof<TypeHint<int64 * Int64 * Int64Len>>

        type private IntPtr = struct
            static member TryFormat (value: nativeint, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                if sizeof<nativeint> = sizeof<int> then
                    Int.TryFormat((int value), destination, format, &bytesWritten)
                else
                    Int64.TryFormat((int64 value), destination, format, &bytesWritten)

            interface IUtf8Formatter<nativeint> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    IntPtr.TryFormat(value, destination, format, &bytesWritten)
        end

        type private IntPtrLen = struct
            interface IFixedSizePremise with 
                member _.FixedSize = 
                    if sizeof<nativeint> = sizeof<int> then
                        FixedSizeTheory.GetFixedSize<IntLen>()
                    else
                        FixedSizeTheory.GetFixedSize<Int64Len>()
        end

        let ofIntPtr = ofFixedSizeUtf8Formatter defaultof<TypeHint<nativeint * IntPtr * IntPtrLen>>


        type private CharPremise = struct

            static member EscapeRequired (format: OCamlString) = 
                (* C: convert a character argument to OCaml syntax (single quotes, escapes). *)
                stringEqual format "C"B

            static member GetMaxLength (c: inref<OCamlChar>, format: inref<OCamlString>): int =
                if CharPremise.EscapeRequired format then
                    1 + (ByteSpans.charToEscapedLength c) + 1
                else
                    1

            static member TryFormat (value: OCamlChar, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                if destination.Length < CharPremise.GetMaxLength(&value, &format) then
                    bytesWritten <- 0;
                    false
                else if CharPremise.EscapeRequired format then
                    destination[0] <- Cc.AsciiSingleQuote;
                    let escapedCount = ByteSpans.escapeCharAndCount(destination.Slice(1)) value in
                    destination[1 + escapedCount] <- Cc.AsciiSingleQuote;
                    bytesWritten <- 1 + escapedCount + 1;
                    true
                else
                    destination[0] <- value;
                    bytesWritten <- 1;
                    true

            interface IUtf8Formatter<OCamlChar> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    CharPremise.TryFormat(value, destination, format, &bytesWritten)

            interface IMaxLengthPremise<OCamlChar, OCamlString> with
                member _.GetMaxLength (source, format): int = 
                    CharPremise.GetMaxLength(&source, &format)
        end

        let ofChar = ofUtf8Formatter defaultof<TypeHint<byte * CharPremise * CharPremise>>

        let rec private stringToEscaped_aux (source: ReadOnlySpan<byte>) (destination: Span<byte>) (acc: int) : int =
            if source.IsEmpty then acc else
            let escapedCount = ByteSpans.escapeCharAndCount destination source[0] in
            let nextAcc = acc + escapedCount in
            stringToEscaped_aux (source.Slice(1)) (destination.Slice(escapedCount)) nextAcc

        type private StringPremise = struct

            static member EscapeRequired (format: OCamlString) = 
                (* S: convert a string argument to OCaml syntax (double quotes, escapes). *)
                stringEqual format "S"B
            
            static member GetMaxLength (s: inref<OCamlString>, format: inref<OCamlString>): int =
                if StringPremise.EscapeRequired format then
                    1 + (ByteSpans.toEscapedLength (Obs.stringToReadOnlySpan s)) + 1
                else
                    (Obs.stringToReadOnlySpan s).Length

            static member TryFormat (value: OCamlString, destination: Span<byte>, format: OCamlString, bytesWritten: byref<int>): bool = 
                if destination.Length < StringPremise.GetMaxLength(&value, &format) then
                    bytesWritten <- 0;
                    false
                else 
                    let span = (Obs.stringToReadOnlySpan value) in
                    if StringPremise.EscapeRequired format then
                        destination[0] <- Cc.AsciiDoubleQuote;
                        let escapedCount = stringToEscaped_aux span (destination.Slice(1)) 0 in
                        destination[1 + escapedCount] <- Cc.AsciiDoubleQuote;
                        bytesWritten <- 1 + escapedCount + 1;
                        true
                    else
                        span.CopyTo(destination);
                        bytesWritten <- span.Length;
                        true

            interface IUtf8Formatter<OCamlString> with
                member _.TryFormat (value, destination, format, bytesWritten): bool = 
                    StringPremise.TryFormat(value, destination, format, &bytesWritten)

            interface IMaxLengthPremise<OCamlString, OCamlString> with
                member _.GetMaxLength (source, format): int = 
                    StringPremise.GetMaxLength(&source, &format)
        end

        let ofString = ofUtf8Formatter defaultof<TypeHint<OCamlString * StringPremise * StringPremise>>

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
    module T8 = Utf8Formatters

    let (|FlattenSegment|) (Segment(SegmentString(le, fo, tr), tc)) = le,fo,tr,tc

    let ofFlatten le fo tr tc = (Segment(SegmentString(le, fo, tr), tc))

    let private b2f format = SegmentString(Obs.emptyString, Obs.Unsafe.stringOfArray format, Obs.emptyString)

    let toIgnore<'u> (b: byte array) = ofFlatten (Obs.Unsafe.stringOfArray b) Obs.emptyString Obs.emptyString (T8.ofUnit<'u>)

    let toUnit b = toIgnore<unit> b

    [<RequireQualifiedAccess>]
    module Literals = begin

        let B  = Segment(b2f "B"B, T8.ofBool)
        
        let d= Segment(b2f "d"B, T8.ofInt)
        
        let ld = Segment(b2f "ld"B, T8.ofInt)
        
        let Ld = Segment(b2f "Ld"B, T8.ofInt64)

        let nd = Segment(b2f "nd"B, T8.ofIntPtr)

        let c = Segment(b2f "c"B, T8.ofChar)

        let C = Segment(b2f "C"B, T8.ofChar)

        let s = Segment(b2f "s"B, T8.ofString)

        let S = Segment(b2f "S"B, T8.ofString)


    end

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

    let head l = match decons l with | hd, tl -> hd

    let replaceHead (hd: Segment<'hd>) (l: SegmentList<'hd -> 'tl>) =
        let (_, tl) = decons l in
        cons hd tl

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
    InitialBuffer: 'TBuffer;
    InitialResult: 'TResult;
}

[<NoEquality; NoComparison>]
[<Struct>]
type WriterFactoryEnvironment<'TContext, 'TBuffer, 'TResult> = {
    Config: WriterFactoryConfig<'TBuffer, 'TResult>;
    Segments: SegmentList<'TContext>;
    BufferState: 'TBuffer;
    ResultState: 'TResult;
}

[<NoEquality; NoComparison>]
[<Struct>]
type LegacyFormat<'TContext, 'TBuffer, 'TResult> =
    private
    | LegacyFormat of 
        segments: SegmentList<'TContext> * 
        writerFactory: (WriterFactoryEnvironment<'TContext, 'TBuffer, 'TResult> -> 'TContext) (* '타입'을 맞추기 위해, TBuffer 와 TResult 를 앞으로 뺐다. *)


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
            let nextFactory (env: WriterFactoryEnvironment<'hd -> 'tl,'b,'r>) (source: 'hd) : 'tl =
                let slHd,slTl = SegmentLists.decons env.Segments in
                let mutable bs = env.BufferState in
                let mutable rs = env.ResultState in
                format env.Config.BufferWriter &bs slHd source;
                format env.Config.ResultWriter &rs slHd source;
                let nextEnv = { Config = env.Config; Segments = slTl; BufferState = bs; ResultState = rs } in
                factoryTail nextEnv
            in
            LegacyFormat(nextSl, nextFactory)

    // decons 는...없음!

    let head (LegacyFormat(sl, _)) = S.head sl

    let replaceHead (hd: Segment<'hd>) (l: LegacyFormat<'hd->'tl,'b,'r>) =
        match l with
        | LegacyFormat(sl, factoryTail) ->
            LegacyFormat(S.replaceHead hd sl, factoryTail)

    let toWriterFactory (LegacyFormat(_, factory)) = factory

end

module WriterFactories = begin

    type private Id<'a> = Nemonuri.Transcodings.Identity<'a>

    let private write_core<'b, 's when 'b :> IBufferWriter<byte>> (b: 'b) (Segments.FlattenSegment(le, fo, tr, tc)) (s: 's) : 'b =
        let mutable b' = b in
        let mutable placeHolder: int = 0 in
        
        let inline writePlane src = TranscodeWhileDestinationTooSmall<byte,byte,Id<byte>,'b>(Obs.stringToReadOnlySpan src, &b', &placeHolder); 
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

    let inline toConfigAuto b r = { BufferWriter = tryToFormatter b; ResultWriter = tryToFormatter r; InitialBuffer = b; InitialResult = r }

    let segmentListToEnv (config: WriterFactoryConfig<'b,'r>) (sl: SegmentList<'ctx>) = { Config = config; Segments = sl; BufferState = config.InitialBuffer; ResultState = config.InitialResult }

    let legacyFormatToEnv (config: WriterFactoryConfig<'b,'r>) (fmt: LegacyFormat<'ctx,'b,'r>) = 
        match fmt with
        | LegacyFormat(sl,_) -> segmentListToEnv config sl
        
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


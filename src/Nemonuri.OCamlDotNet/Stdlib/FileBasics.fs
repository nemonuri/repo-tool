namespace Nemonuri.OCamlDotNet.Primitives.FileBasics


open System
open System.IO
open System.Buffers
open Nemonuri.Posix
open Nemonuri.Buffers
open Nemonuri.ByteChars
open Nemonuri.Transcodings
open Nemonuri.PureTypeSystems
open Nemonuri.PureTypeSystems.Primitives
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module Ds = Nemonuri.OCamlDotNet.Primitives.DotNetStreams
module Fd = Nemonuri.OCamlDotNet.Primitives.FileDiscriptors

type internal OCamlStandardWriterKind =
| Output
| Error

[<NoComparison; NoEquality; Struct>]
type internal BinaryModeWriter =
    | BinaryModeWriter of streamWithByteArrayPool:StreamWithByteArrayPool
    with

        member x.Advance (count: int) = 
            match x with
            | BinaryModeWriter b -> b.Advance(count)
        
        member x.GetMemory (sizeHint: int) = 
            match x with
            | BinaryModeWriter b -> b.GetMemory(sizeHint)

        member x.GetSpan (sizeHint: int) = 
            match x with
            | BinaryModeWriter b -> b.GetSpan(sizeHint)

        member x.Dispose (): unit = 
            match x with
            | BinaryModeWriter b -> b.Dispose();

        interface IBufferWriter<byte> with

            member x.Advance (count: int) = x.Advance count
            
            member x.GetMemory (sizeHint: int) = x.GetMemory sizeHint

            member x.GetSpan (sizeHint: int) = x.GetSpan sizeHint
        
        interface IDisposable with

            member x.Dispose (): unit = x.Dispose()
    end


type internal TextModeWriter = DisposableByteBufferWriterWithTranscoder<BinaryModeWriter,UncheckedUtf8UnixToWindowsNewLine>

type Writable = Refined<FileDescriptor, Predicates.Refiner<FileDescriptor, Fd.Predicates.CanWrite>>

type Readable = Refined<FileDescriptor, Predicates.Refiner<FileDescriptor, Fd.Predicates.CanRead>>

module Writables = begin

    let tryOfFileDescriptor fd = Fd.tryRefineToCanWrite fd

    let toStream (w: Writable) = Fd.toStream w.Value

    let internal createBinaryModeWriter (w: Writable) = 
        new StreamWithByteArrayPool(null, toStream w, Unchecked.defaultof<_>)
        |> BinaryModeWriter

    let internal createTextModeWriter (w: Writable) = new TextModeWriter(createBinaryModeWriter w)

end


module Files = begin

    let internal createNullBinaryModeWriter() = 
        new StreamWithByteArrayPool(null, Stream.Null, Unchecked.defaultof<_>)
        |> BinaryModeWriter

    let internal createWriter (fd: FileDescriptor) =
        Fd.tryRefineToCanWrite fd 
        |> ValueOption.map Writables.createBinaryModeWriter
        |> ValueOption.defaultWith createNullBinaryModeWriter


end

#if false
    internal
    | StandardWriter of writer: BinaryWriter * kind: OCamlStandardWriterKind
    | RegularFile of fileStream: WritableStream<FileStream>
    | Other of stream: WritableStream<Stream> * aux: objnull
    with

        member x.Flush() =
            match x with
            | StandardWriter(w,_) -> w.Flush()
            | RegularFile(fs) -> fs.Value.Flush()
            | Other(s,_) -> s.Value.Flush()

        interface IFlushable with
            member x.Flush (): unit = x.Flush()

    end

module Writables = begin

    let private tryToWritableStream (s: 's) = Ds.tryRefineToCanWrite s

    let tryOfStream (s: Stream) =
        if LanguagePrimitives.PhysicalEquality s Sio.Output.BaseStream then
            StandardWriter (Sio.Output, Output) |> Some
        else if LanguagePrimitives.PhysicalEquality s Sio.Error.BaseStream then
            StandardWriter (Sio.Error, Error) |> Some
        else 
        match s with
        | :? FileStream as fs -> 
            match tryToWritableStream fs with
            | ValueNone -> None
            | ValueSome vs -> vs |> RegularFile |> Some
        | s -> 
            match tryToWritableStream s with
            | ValueNone -> None
            | ValueSome s -> Other(s,null) |> Some

    let toStream (w: Writable) : Stream =
        match w with
        | StandardWriter(w,_) -> w.BaseStream
        | RegularFile(fs) -> fs.Value
        | Other(s,_) -> s.Value

    let ofStdOut = tryOfStream Sio.Output.BaseStream |> Option.get

    let ofStdError = tryOfStream Sio.Error.BaseStream |> Option.get


    module internal Internals = begin

        let toBinaryModeWriter x =
            match x with
            | StandardWriter(w, _) -> BinaryWriterWithPool (new BinaryWriterWithPool(null, w, Unchecked.defaultof<_>))
            | s -> BinaryModeWriter (new StreamWithByteArrayPool(null, toStream s, Unchecked.defaultof<_>))

        let toEnsuredBinaryModeWriter (maybe: voption<BinaryModeWriter>) x = defaultValueArg maybe (toBinaryModeWriter x)

        let toTextModeWriter x = new TextModeWriter(toBinaryModeWriter x)

        let toEnsuredTextModeWriter (maybe: voption<TextModeWriter>) x = defaultValueArg maybe (toTextModeWriter x)

    end

end

type ReadableStream<'s when 's :> Stream> = Refined<'s,Ds.CanRead<'s>>

type Readable = 
    internal 
    | StandardReader of reader: BinaryReader
    | RegularFile of fileStream: ReadableStream<FileStream>
    | Other of stream: ReadableStream<Stream> * aux: objnull

module Readables = begin

    let private tryToReadableStream (s: 's) = Ds.tryRefineToCanRead s

    let tryOfStream (s: Stream) =
        if LanguagePrimitives.PhysicalEquality s Sio.Input.BaseStream then
            StandardReader (Sio.Input) |> Some
        else 
        match s with
        | :? FileStream as fs -> 
            match tryToReadableStream fs with
            | ValueNone -> None
            | ValueSome vs -> vs |> RegularFile |> Some
        | s -> 
            match tryToReadableStream s with
            | ValueNone -> None
            | ValueSome s -> Other(s,null) |> Some

    let toStream (w: Readable) : Stream =
        match w with
        | StandardReader(w) -> w.BaseStream
        | RegularFile(fs) -> fs.Value
        | Other(s,_) -> s.Value

    let ofStdIn = tryOfStream Sio.Input.BaseStream |> Option.get

end
#endif
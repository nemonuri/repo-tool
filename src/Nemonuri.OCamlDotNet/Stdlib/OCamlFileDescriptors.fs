namespace Nemonuri.OCamlDotNet.Primitives

open System
open System.IO
open System.Buffers
open Nemonuri.Buffers
open Nemonuri.Transcodings
open Nemonuri.ByteChars
open Nemonuri.ByteChars.IO
open Microsoft.FSharp.NativeInterop
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

type internal OCamlStandardWriterKind =
| Output
| Error

[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
[<Struct>]
type internal BufferWriterShim =
    // | Boxed of boxed:IBufferWriter<byte> * flusher:(IBufferWriter<byte> -> unit)
    | BinaryWriterWithPool of binaryWriterWithPool:BinaryWriterWithPool
    | StreamWithByteArrayPool of streamWithByteArrayPool:StreamWithByteArrayPool
    with

        member x.Advance (count: int) = 
            match x with
            // | Boxed (b, _) -> b.Advance(count)
            | BinaryWriterWithPool b -> b.Advance(count)
            | StreamWithByteArrayPool b -> b.Advance(count)
        
        member x.GetMemory (sizeHint: int) = 
            match x with
            // | Boxed (b, _) -> b.GetMemory(sizeHint)
            | BinaryWriterWithPool b -> b.GetMemory(sizeHint)
            | StreamWithByteArrayPool b -> b.GetMemory(sizeHint)

        member x.GetSpan (sizeHint: int) = 
            match x with
            // | Boxed (b, _) -> b.GetSpan(sizeHint)
            | BinaryWriterWithPool b -> b.GetSpan(sizeHint)
            | StreamWithByteArrayPool b -> b.GetSpan(sizeHint)

        member x.Flush (): unit = 
            match x with
            // | Boxed (b, f) -> f b;
            | BinaryWriterWithPool b -> b.Dispose();
            | StreamWithByteArrayPool b -> b.Dispose();

        interface IBufferWriter<byte> with

            member x.Advance (count: int) = x.Advance count
            
            member x.GetMemory (sizeHint: int) = x.GetMemory sizeHint

            member x.GetSpan (sizeHint: int) = x.GetSpan sizeHint
        
        interface IFlushable with

            member x.Flush (): unit = x.Flush()
    end

[<RequireQualifiedAccess>]
type internal OCamlWritableFileDescriptor =
    | StandardWriter of writer: BinaryWriter * kind: OCamlStandardWriterKind
    | RegularFile of fileStream: FileStream
    | Other of stream: Stream * aux: objnull


module internal OCamlWritableFileDescriptors =

    type t = OCamlWritableFileDescriptor
    type B = BufferWriterShim

    let toStream (x: t) =
        match x with
        | t.StandardWriter (bw, _) -> bw.BaseStream
        | t.RegularFile s -> s
        | t.Other (s, _) -> s

    let toBufferWriter (x: t) : BufferWriterShim =
        match x with
        | t.StandardWriter(w, _) -> B.BinaryWriterWithPool (new BinaryWriterWithPool(null, w, Unchecked.defaultof<_>))
        | s -> B.StreamWithByteArrayPool (new StreamWithByteArrayPool(null, toStream s, Unchecked.defaultof<_>))
    
    let ensureBufferWriter (maybe: voption<BufferWriterShim>) (x: t) =
        match maybe with
        | ValueSome b -> b
        | ValueNone -> toBufferWriter x

module Wfd = OCamlWritableFileDescriptors

type OCamlOutChannel =
    class
        val internal FileDescriptor : OCamlWritableFileDescriptor;
        val mutable internal BinaryMode: bool;
        val mutable private InnerBufferWriter: voption<BufferWriterShim>;
        internal new(fd: OCamlWritableFileDescriptor, [<Struct>] ?binaryMode : bool) =
            { FileDescriptor = fd; BinaryMode = defaultValueArg binaryMode false; InnerBufferWriter = ValueNone }
        
        member private x.EnsuredBufferWriter = 
            match x.InnerBufferWriter with
            | ValueSome v -> v
            | ValueNone -> 
                let v = Wfd.ensureBufferWriter x.InnerBufferWriter x.FileDescriptor in
                x.InnerBufferWriter <- ValueSome v; 
                v

        member x.Advance (count: int) = x.EnsuredBufferWriter.Advance count

        member x.GetMemory (sizeHint: int) = x.EnsuredBufferWriter.GetMemory sizeHint

        member x.GetSpan (sizeHint: int) = x.EnsuredBufferWriter.GetSpan sizeHint

        member x.Flush() = //x.EnsuredBufferWriter.Flush(); x.InnerBufferWriter <- ValueNone
            match x.InnerBufferWriter with
            | ValueSome v -> v.Flush(); x.InnerBufferWriter <- ValueNone
            | ValueNone -> ()
        
        interface IBufferWriter<byte> with

            member x.Advance (count: int) = x.Advance count
            
            member x.GetMemory (sizeHint: int) = x.GetMemory sizeHint

            member x.GetSpan (sizeHint: int) = x.GetSpan sizeHint
        
        interface IFlushable with

            member x.Flush (): unit = x.Flush()
    end


[<RequireQualifiedAccess>]
type OCamlFileDescriptor = 
    internal
    | OCamlWritableFileDescriptor of OCamlWritableFileDescriptor
    | StandardReader of reader: BinaryReader

    with
        interface IFlushable with
            member x.Flush (): unit = 
                match x with
                | OCamlWritableFileDescriptor fd -> OCamlWritableFileDescriptors.toStream fd |> _.Flush()
                | StandardReader _ -> ()
    end

module Flushables = begin

    let flush (x: #IFlushable) = x.Flush()

end


module internal OCamlFileDescriptors =

    type t = OCamlFileDescriptor
    type private tw = OCamlWritableFileDescriptor

//--- mock OCamlFileDescriptor cons/decons ---
    let inline (|StandardWriter|StandardReader|RegularFile|Other|) (x: t) =
        match x with
        | t.OCamlWritableFileDescriptor v ->
            match v with
            | tw.StandardWriter (w, k) -> StandardWriter (w,k)
            | tw.RegularFile fs -> RegularFile fs
            | tw.Other (s, a) -> Other (s,a)
        | t.StandardReader v -> StandardReader v

    let private wfd v = t.OCamlWritableFileDescriptor v

    let StandardWriter v = wfd (tw.StandardWriter v)
    let RegularFile v = wfd (tw.RegularFile v)
    let Other v = wfd (tw.Other v)
    let StandardReader v = t.StandardReader v
//---|

    let tryToWritable = function
    | t.OCamlWritableFileDescriptor fd -> Some fd
    | _ -> None

    let toWritable fd = tryToWritable fd |> Option.get

    type out_channel = OCamlOutChannel

    type WriterOptions =
    | None
    | UseTranscoder

    let toStream (fd: t) = 
        match fd with
        | StandardWriter (bw, _) -> bw.BaseStream
        | StandardReader br -> br.BaseStream
        | RegularFile s -> s
        | Other (s, _) -> s

    let canWrite (fd: t) = 
        match fd with
        | StandardWriter _ -> true
        | StandardReader _ -> false
        | s -> (toStream s).CanWrite
    
    let flush (fd: t) =
        match fd with
        | StandardWriter (bw, _) -> bw.Flush()
        | StandardReader _ -> ()
        | s -> (toStream s).Flush()

#if false   
    let writeByteIfNotStdIn (fd: t) (b: byte) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) -> bw.Write(b)
        | s -> (toStream s).WriteByte(b)
#endif

    module Writers =

        type private O = OCamlWritableFileDescriptor
        type private B = BufferWriterShim

        let toTotal (fd: OCamlWritableFileDescriptor) = OCamlFileDescriptor.OCamlWritableFileDescriptor fd
        
        let tryOfTotal (fd: OCamlFileDescriptor) : voption<OCamlWritableFileDescriptor> =
            match fd with
            | StandardWriter(w,k) -> O.StandardWriter(w, k) |> ValueSome
            | RegularFile(fs) -> O.RegularFile fs |> ValueSome
            | Other(st, aux) -> O.Other(st, aux) |> ValueSome
            | _ -> ValueNone
        
        let ofTotal fd = tryOfTotal fd |> ValueOption.get

        let toBufferWriterShim (fd: OCamlWritableFileDescriptor) =
            match fd with
            | O.StandardWriter(w, k) -> B.BinaryWriterWithPool (new BinaryWriterWithPool(null, w, Unchecked.defaultof<_>))
            | s -> B.StreamWithByteArrayPool (new StreamWithByteArrayPool(null, (s |> toTotal |> toStream), Unchecked.defaultof<_>))
            

    let writeByteSpanIfNotStdIn (fd: t) (bs: ReadOnlySpan<byte>) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) -> BinaryWriterTheory.WriteByteSpan(bw, bs)
        | s -> StreamTheory.WriteByteSpan(toStream s, bs)
    
    let private writeByteSpanWithTranscodingIfNotStdIn (fd: t) (bs: ReadOnlySpan<byte>) (tcp: 'tcp) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) ->
            use mutable bwp = new BinaryWriterWithPool(null, bw, bs.Length) in
            let _ = TranscoderTheory.TranscodeWhileDestinationTooSmall<byte,byte,'tcp,BinaryWriterWithPool>(bs,&bwp) in
            ()
        | s -> 
            use mutable swp = new StreamWithByteArrayPool(null, (toStream s), bs.Length) in
            let _ = TranscoderTheory.TranscodeWhileDestinationTooSmall<byte,byte,'tcp,StreamWithByteArrayPool>(bs, &swp) in
            ()

    let writeByteSpanWithOptionsIfNotStdIn (fd: t) (bs: ReadOnlySpan<byte>) (opt: WriterOptions) =
        match opt with
        | None -> writeByteSpanIfNotStdIn fd bs
        | UseTranscoder -> writeByteSpanWithTranscodingIfNotStdIn fd bs Unchecked.defaultof<UncheckedUtf8UnixToWindowsNewLine>

    let writeByteWithOptionsIfNotStdIn (fd: t) (b: byte) (opt: WriterOptions) =
        let sgt: Span<byte> = DotNetSpans.NativePtrToSpan(NativePtr.stackalloc 1, 1) in
        sgt[0] <- b
        writeByteSpanWithOptionsIfNotStdIn fd sgt opt

    let writeOCamlBytesWithOptionsIfNotStdIn (fd: t) (bs: OCamlBytes) (pos: OCamlInt) (len: OCamlInt) (opt: WriterOptions) =
        let rbs = Obs.bytesToReadOnlySpan (Obs.bytesSlice bs pos len) in
        writeByteSpanWithOptionsIfNotStdIn fd rbs opt



    let isTranscoderRequiredInTextMode = Environment.NewLine <> "\n"

    let outChannelToWriterOptions (oc: out_channel) =
        match (not oc.BinaryMode) && isTranscoderRequiredInTextMode with
        | true -> UseTranscoder
        | false -> None

    let outChannelToFileDescriptor (oc: out_channel) = oc.FileDescriptor |> Writers.toTotal

    module OutChannelMonads =

        [<RequireQualifiedAccess>]
        [<Struct>]
        [<NoEquality; NoComparison>]
        type Repr = { fd: OCamlFileDescriptor; opt: WriterOptions }

        let extract oc = { Repr.fd = outChannelToFileDescriptor oc; Repr.opt = outChannelToWriterOptions oc }

        [<NoEquality; NoComparison>]
        type Monad =
            struct
                member inline _.Bind(oc: out_channel, [<InlineIfLambda>] f: Repr -> 't) : 't = extract oc |> f
                member inline _.ReturnFrom(x: 'a) = x
            end
        
        let monad = Monad()

    module O = OutChannelMonads

    let writeByteSpanToOutChannel (oc: out_channel) (bs: ReadOnlySpan<byte>) = 
        let oc' = O.extract oc in
        writeByteSpanWithOptionsIfNotStdIn oc'.fd bs oc'.opt
    
    let writeByteToOutChannel (oc: out_channel) b = 
        O.monad { let! oc' = oc in return! writeByteWithOptionsIfNotStdIn oc'.fd b oc'.opt }

    let writeOCamlBytesToOutChannel (oc: out_channel) bs pos len =
        O.monad { let! oc' = oc in return! writeOCamlBytesWithOptionsIfNotStdIn oc'.fd bs pos len oc'.opt }
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

type OCamlFileDescriptor = 
    internal
    | StandardWriter of writer: BinaryWriter * kind: OCamlStandardWriterKind
    | StandardReader of reader: BinaryReader
    | RegularFile of fileStream: FileStream
    | Other of stream: Stream * aux: objnull

[<RequireQualifiedAccess>]
type internal OCamlWritableFileDescriptor =
    | StandardWriter of writer: BinaryWriter * kind: OCamlStandardWriterKind
    | RegularFile of fileStream: FileStream
    | Other of stream: Stream * aux: objnull

type OCamlOutChannel = internal { FileDescriptor: OCamlWritableFileDescriptor; mutable BinaryMode: bool }


[<Struct>]
[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
type BufferWriterShim =
    internal
    | Boxed of boxed:IBufferWriter<byte>
    | BinaryWriterWithPool of binaryWriterWithPool:BinaryWriterWithPool
    | StreamWithByteArrayPool of streamWithByteArrayPool:StreamWithByteArrayPool
    with

        interface IBufferWriter<byte> with

            member this.Advance (count: int) = 
                match this with
                | Boxed b -> b.Advance(count)
                | BinaryWriterWithPool b -> b.Advance(count)
                | StreamWithByteArrayPool b -> b.Advance(count)
            
            member this.GetMemory (sizeHint: int) = 
                match this with
                | Boxed b -> b.GetMemory(sizeHint)
                | BinaryWriterWithPool b -> b.GetMemory(sizeHint)
                | StreamWithByteArrayPool b -> b.GetMemory(sizeHint)

            member this.GetSpan (sizeHint: int) = 
                match this with
                | Boxed b -> b.GetSpan(sizeHint)
                | BinaryWriterWithPool b -> b.GetSpan(sizeHint)
                | StreamWithByteArrayPool b -> b.GetSpan(sizeHint)
    end

module internal OCamlFileDescriptors =

    type t = OCamlFileDescriptor

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

        let toTotal (fd: OCamlWritableFileDescriptor) =
            match fd with
            | O.StandardWriter(w, k) -> StandardWriter(w,k)
            | O.RegularFile(fs) -> RegularFile fs
            | O.Other(st, aux) -> Other(st, aux)
        
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
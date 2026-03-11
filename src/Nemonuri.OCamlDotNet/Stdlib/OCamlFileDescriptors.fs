namespace Nemonuri.OCamlDotNet.Primitives

open System
open System.IO
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

type OCamlOutChannel = internal { FileDescriptor: OCamlFileDescriptor; mutable BinaryMode: bool }

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

    // let toTextWriter (fd: t) : TextWriter = new StreamWriter(toStream fd, null, -1, true)

#if false   
    let writeByteIfNotStdIn (fd: t) (b: byte) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) -> bw.Write(b)
        | s -> (toStream s).WriteByte(b)
#endif

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
            let _ = TranscodingTheory.TranscodeWhileDestinationTooSmall<byte,byte,'tcp,BinaryWriterWithPool>(bs,&bwp) in
            ()
        | s -> 
            use mutable swp = new StreamWithByteArrayPool(null, (toStream s), bs.Length) in
            let _ = TranscodingTheory.TranscodeWhileDestinationTooSmall<byte,byte,'tcp,StreamWithByteArrayPool>(bs, &swp) in
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

#if false    
    let isStdIn (fd: t) = match fd with | StandardReader _ -> true | _ -> false

    let writeByteCharWithEncodingIfNotStdIn (fd: t) (b: byte) (dstEncoding: Encoding) =
        if Encodings.isUtf8 dstEncoding then
            writeByteIfNotStdIn fd b
        else
            if isStdIn fd then () else
            let converted = Encoding.Convert(Encodings.utf8NoBom, dstEncoding, [|b|]) in
            writeByteSpanIfNotStdIn fd (converted.AsSpan())

    let writeByteCharSpanWithEncodingIfNotStdIn (fd: t) (bs: ReadOnlySpan<byte>) (dstEncoding: Encoding) =
        if Encodings.isUtf8 dstEncoding then
            writeByteSpanIfNotStdIn fd bs
        else
            if isStdIn fd then () else
            let converted = Encoding.Convert(Encodings.utf8NoBom, dstEncoding, bs.ToArray()) in
            writeByteSpanIfNotStdIn fd (converted.AsSpan())

    
    let outChannelToStream (oc: out_channel) = oc.FileDescriptor |> toStream

    let outChannelToTextWriter (oc: out_channel) = oc.FileDescriptor |> toTextWriter
#endif

    let isTranscoderRequiredInTextMode = Environment.NewLine <> "\n"

    let outChannelToWriterOptions (oc: out_channel) =
        match (not oc.BinaryMode) && isTranscoderRequiredInTextMode with
        | true -> UseTranscoder
        | false -> None

    module OutChannelMonads =

        [<RequireQualifiedAccess>]
        [<Struct>]
        [<NoEquality; NoComparison>]
        type Repr = { fd: OCamlFileDescriptor; opt: WriterOptions }

        let extract oc = { Repr.fd = oc.FileDescriptor; Repr.opt = outChannelToWriterOptions oc }

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
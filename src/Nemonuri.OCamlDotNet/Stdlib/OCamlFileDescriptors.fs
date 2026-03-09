namespace Nemonuri.OCamlDotNet.Primitives

open System
open System.IO
open System.Text
open Nemonuri.ByteChars.IO

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

module OCamlFileDescriptors =

    type t = OCamlFileDescriptor

    type out_channel = OCamlOutChannel

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

    let toTextWriter (fd: t) : TextWriter = new StreamWriter(toStream fd, null, -1, true)

    let writeByteIfNotStdIn (fd: t) (b: byte) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) -> bw.Write(b)
        | s -> (toStream s).WriteByte(b)
    
    let writeByteSpanIfNotStdIn (fd: t) (bs: ReadOnlySpan<byte>) =
        match fd with
        | StandardReader _ -> ()
        | StandardWriter (bw, _) -> BinaryWriterTheory.WriteByteSpan(bw, bs)
        | s -> StreamTheory.WriteByteSpan(toStream s, bs)
    
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



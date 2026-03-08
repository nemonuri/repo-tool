namespace Nemonuri.OCamlDotNet.Primitives

open System.IO

type internal OCamlConsoleStreamKind =
| In
| Out
| Error

type OCamlFileDescriptor = 
    internal
    | Console of stream: Stream * kind: OCamlConsoleStreamKind
    | RegularFile of fileStream: FileStream
    | Other of stream: Stream * aux: objnull

type OCamlOutChannel = internal { FileDescriptor: OCamlFileDescriptor; mutable BinaryMode: bool }

module OCamlFileDescriptors =

    type t = OCamlFileDescriptor

    type out_channel = OCamlOutChannel

    let toStream (fd: t) = 
        match fd with
        | Console (s, _) -> s
        | RegularFile s -> s
        | Other (s, _) -> s

    let toTextWriter (fd: t) : TextWriter = new StreamWriter(toStream fd, null, -1, true)
    
    let outChannelToStream (oc: out_channel) = oc.FileDescriptor |> toStream

    let outChannelToTextWriter (oc: out_channel) = oc.FileDescriptor |> toTextWriter



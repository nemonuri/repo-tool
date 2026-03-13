namespace Nemonuri.OCamlDotNet.Primitives

open Nemonuri.OCamlDotNet.Primitives.TypeShadowing

module Printf =

    let fprintf (outchan: out_channel) (format: format<'a, out_channel, unit>) (arg: 'a) =
        (Formats.toWriter_core format.Core) (outchan.FileDescriptor |> OCamlFileDescriptors.Writers.toBufferWriterShim) arg

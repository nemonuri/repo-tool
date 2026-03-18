namespace Nemonuri.OCamlDotNet.Forwarded

open System
open Nemonuri.Transcodings
open type Nemonuri.Transcodings.TranscoderTheory
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.FileBasics
open Nemonuri.OCamlDotNet.Primitives.FormatBasics
module Unix = Nemonuri.OCamlDotNet.Forwarded.Unix
module Ofd = Nemonuri.OCamlDotNet.Primitives.OCamlFileDescriptors
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module T8 = Nemonuri.OCamlDotNet.Primitives.FormatBasics.Transcoders.Utf8Formatters

/// reference: https://ocaml.org/manual/5.4/api/Out_channel.html
module Out_channel =

    type t = OCamlOutChannel

    type open_flag =  
    | Open_rdonly  (* open for reading. *)
    | Open_wronly  (* open for writing. *)
    | Open_append  (* open for appending: always write at end of file. *)
    | Open_creat  (* create the file if it does not exist. *)
    | Open_trunc  (* empty the file if it already exists. *)
    | Open_excl  (* fail if Open_creat and the file already exists. *)
    | Open_binary  (* open in binary mode (no conversion). *)
    | Open_text  (* open in text mode (may perform conversions). *)
    | Open_nonblock (* open in non-blocking mode. *)


    let stdout = OCamlOutChannel(Ofd.toFileDescriptor Unix.stdout |> Writables.tryOfFileDescriptor |> ValueOption.get, binaryMode = false)

    let stderr = OCamlOutChannel(Ofd.toFileDescriptor Unix.stderr |> Writables.tryOfFileDescriptor |> ValueOption.get, binaryMode = false)

    let set_binary_mode (oc: t) (b: bool) = oc.BinaryMode <- b

    let is_binary_mode (oc: t) = oc.BinaryMode

    let flush (oc: t) = oc.Flush()

    let output_char (oc: t) (c: OCamlChar) = 
        let mutable oc' = oc in
        let _ = T8.ofChar.TranscodeSingletonWhileDestinationTooSmall(c,&oc',Unchecked.defaultof<_>) in
        ()

    let output_byte (oc: t) (n: OCamlInt) = output_char oc (byte n)

    let output (oc: t) (b: OCamlBytes) (pos: OCamlInt) (len: OCamlInt) = 
        let mutable oc' = oc: t in
        let sliced = Obs.bytesSlice b pos len in
        let _  = TranscodeWhileDestinationTooSmall<byte,byte,Identity<byte>,t>((Obs.bytesToReadOnlySpan sliced), &oc') in
        ()

    let output_bytes (oc: t) (b: OCamlBytes) = output oc b 0 (Bytes.length b)
        
    let output_substring oc (s: OCamlString) pos len = String.mnd { let! s' = s in return! output oc s' pos len }

    let output_string (oc: t) (s: OCamlString) = String.mnd { let! s' = s in return! output_bytes oc s' }
    
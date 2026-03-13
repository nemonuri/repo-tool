namespace Nemonuri.OCamlDotNet.Forwarded

open System
open Nemonuri.OCamlDotNet.Primitives
module Unix = Nemonuri.OCamlDotNet.Forwarded.Unix
module Ofd = Nemonuri.OCamlDotNet.Primitives.OCamlFileDescriptors
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

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


    let stdout = { FileDescriptor = Unix.stdout |> Ofd.Writers.ofTotal; BinaryMode = false }

    let stderr = { FileDescriptor = Unix.stderr |> Ofd.Writers.ofTotal; BinaryMode = false }

    let set_binary_mode (oc: t) (b: bool) = oc.BinaryMode <- b

    let is_binary_mode (oc: t) = oc.BinaryMode

    let flush (oc: t) = Ofd.flush (oc.FileDescriptor |> Ofd.Writers.toTotal)

    let output_char (oc: t) (c: OCamlChar) = Ofd.writeByteToOutChannel oc c

    let output_byte (oc: t) (n: OCamlInt) = output_char oc (byte n)

    let output (oc: t) (b: OCamlBytes) (pos: OCamlInt) (len: OCamlInt) = Ofd.writeOCamlBytesToOutChannel oc b pos len

    let output_bytes (oc: t) (b: OCamlBytes) = output oc b 0 (Bytes.length b)
        
    let output_substring oc (s: OCamlString) pos len = String.mnd { let! s' = s in return! output oc s' pos len }

    let output_string (oc: t) (s: OCamlString) = String.mnd { let! s' = s in return! output_bytes oc s' }
    
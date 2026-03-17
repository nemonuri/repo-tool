namespace Nemonuri.OCamlDotNet.Primitives


module TypeShadowing =

    type char = OCamlChar

    type int = OCamlInt

    type string = OCamlString

    type out_channel = OCamlOutChannel

    type file_descr = OCamlFileDescriptor

    type format4<'a,'b,'c,'d> = OCamlFormat<'a,'b,'c,'d>

    type format<'a,'b,'c> = OCamlFormat<'a,'b,'c,'c>

/// - Reference: https://ocaml.org/manual/5.4/api/Sys.html
module Nemonuri.OCamlDotNet.Sys
open Nemonuri.OCamlDotNet
open System.IO

type private Ia = System.Collections.Immutable.ImmutableArray
//let private utf8Encoding = System.Text.UTF8Encoding(false, true)

let private defaultEncoding = System.Text.Encoding.Default

let private dotnetToOCaml (dotnetString: Microsoft.FSharp.Core.string) : string = Ia.ToImmutableArray(System.Span<char>(defaultEncoding.GetBytes(dotnetString)))

/// The command line arguments given to the process. 
/// The first element is the command name used to invoke the program. 
/// The following elements are the command-line arguments given to the program.
let argv = System.Environment.GetCommandLineArgs() |> Array.map dotnetToOCaml
    

/// The name of the file containing the executable currently running. 
/// This name may be absolute or relative to the current directory, depending on the platform and whether the program was compiled to bytecode or a native executable.
let executable_name = Path.Combine [|System.AppContext.BaseDirectory; System.AppDomain.CurrentDomain.FriendlyName|] |> dotnetToOCaml

(**
https://learn.microsoft.com/en-us/dotnet/api/system.array?view=netstandard-2.0

The array size is limited to a total of 4 billion elements, and to a maximum index of 0X7FEFFFFF in any given dimension 
(__0X7FFFFFC7 for byte arrays__ and arrays of single-byte structures).
*)
/// Maximum length of strings and byte sequences.
let max_string_length = 0x7FFFFFC7


#nowarn "62" // ML compatibility

module Nemonuri.OCamlDotNet.Stdlib

open FSharp.Compatibility


// Reference: https://ocaml.org/manual/5.4/api/Stdlib.htm

[<AutoOpen>]
module Exceptions =

    /// Raise the given exception value
    let raise = Core.Operators.raise

    /// Exception raised by library functions to signal that the given arguments do not make sense. 
    /// The string gives some information to the programmer. 
    /// As a general rule, this exception should not be caught, it denotes a programming error and the code should be modified not to trigger it.
    exception Invalid_argument = System.ArgumentException

    [<CompiledNameAttribute("TryGetMessageOfInvalidArgument")>]
    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? Invalid_argument as inv -> Some inv.Message
        | _ -> None

    /// <summary>
    /// Raise exception <see cref="Invalid_argument"/> with the given string.
    /// </summary>
    let invalid_arg message = System.ArgumentException message

    /// Exception raised by library functions to signal that they are undefined on the given arguments. 
    /// The string is meant to give some information to the programmer; 
    /// you must not pattern match on the string literal because it may change in future versions (use Failure _ instead).
    exception Failure = System.Exception

    [<CompiledNameAttribute("TryGetMessageOfFailure")>]
    let (|Failure|_|) = Core.Operators.(|Failure|_|)

    /// <summary>
    /// Raise exception <see cref="Failure"/> with the given string.
    /// </summary>
    let failwith = Core.Operators.failwith

[<AutoOpen>]
module StringOperations =

    let inline (^) (left: string) (right: string) = (^) left right

[<AutoOpen>]
module CharacterOperations =

    let int_of_char = OCaml.Char.code

    let int_to_char = OCaml.Char.chr


let inline (.()) (arr: 'a array) (index: Core.int) = arr[index]

[<AutoOpen>]
module Comparisons =

    let inline (==) left right = Core.LanguagePrimitives.PhysicalEquality left right

    let inline (!=) left right = not (left == right)


module String =

    let sub = OCaml.String.sub

    let index = OCaml.String.index

module Array =

    let of_list = OCaml.Array.of_list

    let to_list = OCaml.Array.to_list

module Sys =
// Reference: https://github.com/fsprojects/FSharp.Compatibility/blob/master/FSharp.Compatibility.OCaml.System/Sys.fs

    let argv = System.Environment.GetCommandLineArgs ()


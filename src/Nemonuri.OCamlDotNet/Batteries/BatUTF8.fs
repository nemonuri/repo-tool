#nowarn "9" // Unverifiable .NET IL code

/// UTF-8 encoded Unicode strings. The type is normal string.
/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUTF8.html
module Nemonuri.OCamlDotNet.Batteries.BatUTF8
open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Batteries

open System.Text.Unicode
open FSharp.NativeInterop
open System.Collections.Immutable

/// UTF-8 encoded Unicode strings. The type is normal string.
type t = String.t

exception Malformed_code

/// `length s` returns the number of Unicode characters contained in s
let inline length (s: t) : int = s.Length

/// `validate s` successes if s is valid UTF-8, otherwise raises Malformed_code. 
/// Other functions assume strings are valid UTF-8, so it is prudent to test their validity for strings from untrusted origins.
let validate (s: t) : unit =
    match Utf8.IsValid(s.AsSpan()) with
    | true -> ()
    | false -> raise Malformed_code

let [<Literal>] private spanSize = 4

/// `init len f` returns a new string which contains `len` Unicode characters. The i-th Unicode character is initialized by `f i`
let init (len: int) (f: int -> BatUChar.t) : t =
    let builder = ImmutableArray.CreateBuilder<char>()

    for i in 0 .. len-1 do
        let newRune = f i
        let span = System.Span<byte>(NativePtr.stackalloc<byte> spanSize |> NativePtr.toVoidPtr, spanSize)
        let writtenLength = newRune.EncodeToUtf8 span
        builder.AddRange(span.Slice(0, writtenLength))           
    
    builder.DrainToImmutable()

/// Positions in the string represented by the number of bytes from the head. The location of the first character is 0
type index = int
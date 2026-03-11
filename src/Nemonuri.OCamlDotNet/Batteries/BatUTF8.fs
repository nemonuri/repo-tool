#nowarn "9" // Unverifiable .NET IL code

namespace Nemonuri.OCamlDotNet.Batteries

open Nemonuri.OCamlDotNet.Primitives

open Nemonuri.ByteChars
open System.Text.Unicode
open FSharp.NativeInterop
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

/// UTF-8 encoded Unicode strings. The type is normal string.
/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatUTF8.html
module BatUTF8 =

    /// UTF-8 encoded Unicode strings. The type is normal string.
    type t = OCamlString

    exception Malformed_code

    let inline private toSpan s = Obs.stringToReadOnlySpan s

    /// `length s` returns the number of Unicode characters contained in s
    let inline length (s: t) : int = Utf8SpanTheory.GetRuneCount(toSpan s)

    /// `validate s` successes if s is valid UTF-8, otherwise raises Malformed_code. 
    /// Other functions assume strings are valid UTF-8, so it is prudent to test their validity for strings from untrusted origins.
    let validate (s: t) : unit =
        match Utf8.IsValid(toSpan s) with
        | true -> ()
        | false -> raise Malformed_code

    /// get s n returns n-th Unicode character of s. The call requires O(n)-time.
    let get (s: t) (n: int) : BatUChar.t =
        try
            let success, rune = Utf8SpanTheory.TryGetRuneAt(toSpan s, n)
            if not success then Exceptions.invalid_arg (Obs.Unsafe.stringOfArray "Out of range"B) else
            rune
        with
            | :? System.ArgumentOutOfRangeException as e -> Exceptions.invalid_arg (Obs.stringOfDotNetString e.Message)

    let [<Literal>] private spanSize = 4

    /// `init len f` returns a new string which contains `len` Unicode characters. The i-th Unicode character is initialized by `f i`
    let init (len: int) (f: int -> BatUChar.t) : t =
        let builder = Nemonuri.Buffers.DrainableArrayBuilder<byte>(len)

        for i in 0 .. len-1 do
            let newRune = f i
            let span = DotNetSpans.NativePtrToSpan(NativePtr.stackalloc<byte> spanSize, spanSize)
            let writtenLength = newRune.EncodeToUtf8 span
            builder.AddRange(span.Slice(0, writtenLength))           
        
        builder.DrainToArraySemgent() |> Obs.Unsafe.stringOfArraySegment

    /// Positions in the string represented by the number of bytes from the head. The location of the first character is 0
    type index = int

    /// iter f s applies f to all Unicode characters in s. The order of application is same to the order of the Unicode characters in s.
    let iter (f: BatUChar.t -> unit) (s: t) : unit =
        let e = Utf8SpanTheory.EnumerateRunes(toSpan s)
        for runeStep in e do
            f runeStep

    let iteri (f: BatUChar.t -> int -> unit) (s: t) : unit =
        let e = Utf8SpanTheory.EnumerateRunes(toSpan s)
        let mutable stepIndex = 0
        for runeStep in e do
            f runeStep stepIndex;
            stepIndex <- stepIndex + 1

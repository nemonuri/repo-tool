namespace Nemonuri.OCamlDotNet.Primitives.Internals

open System
open System.Diagnostics
open Nemonuri.ByteChars
open type System.MemoryExtensions
open type Nemonuri.ByteChars.Extensions.UnsafePinnedSpanPointerExtensions

[<Struct>]
type internal UnsafeOCamlByteSpanSource =
| None
| Array of array: ArraySegment<byte>
| PinnedPointer of pointer: UnsafePinnedSpanPointer<byte>



module internal UnsafeOCamlByteSpanSources =

    type t = UnsafeOCamlByteSpanSource
    type bs = System.Span<byte>
    type rbs = System.ReadOnlySpan<byte>

    let toReadOnlySpan (s: t) : rbs =
        match s with
        | None -> rbs.Empty
        | Array v -> v.AsSpan()
        | PinnedPointer v -> v.LoadReadOnlySpan()
    
    let toSpan (s: t) : bs =
        match s with
        | None -> bs.Empty
        | Array v -> v.AsSpan()
        | PinnedPointer v -> v.LoadSpan()
    
    let empty = None

    let ofArraySegment (s: ArraySegment<byte>) = Array s

    let ofArray (s: byte[]) = ArraySegment<_>(s) |> ofArraySegment

    let ofPinnedSpan (s: rbs) = UnsafePinnedSpanPointerTheory.FromPinnedSpan(s) |> PinnedPointer

    let equal (s1: t) (s2: t) : bool = (toSpan s1).SequenceEqual(toSpan s2)

    let compare (s1: t) (s2: t) : int = (toSpan s1).SequenceCompareTo(toSpan s2)

    let hash (s: t) : int = CommunityToolkit.HighPerformance.ReadOnlySpanExtensions.GetDjb2HashCode(toReadOnlySpan s)

    let length (s: t) : int = (toSpan s).Length

    let slice (s: t) offset sliceLength  = 
        Nemonuri.ByteChars.Diagnostics.Guard.GuardSliceArgumentsAreInValidRange(length s, offset, sliceLength)
        match s with
        | None -> None
        | Array v -> ArraySegment<_>(v.Array, v.Offset + offset, sliceLength) |> ofArraySegment
        | PinnedPointer v -> v.Slice(offset, sliceLength) |> PinnedPointer
    
    let toDotNetString (s: t) : string = ByteCharSpanTheory.ToDotNetString(toSpan s)

    let ofDotNetString (s: string) : t = MutableByteStringTheory.FromDotNetStringWithUtf8Encoding(s) |> ofArraySegment
namespace Nemonuri.OCamlDotNet.Primitives

open System
open Nemonuri.ByteChars
open type System.MemoryExtensions
open type Nemonuri.ByteChars.Extensions.UnsafePinnedSpanPointerExtensions

    type OCamlChar = Microsoft.FSharp.Core.byte
    type OCamlInt = Microsoft.FSharp.Core.int

    [<RequireQualifiedAccess>]
    [<Struct>]
    [<CustomEquality; CustomComparison>]
    type OCamlByteSequenceSource =
    | None
    | Array of array: ArraySegment<byte>
    | ImmutableArray of immutableArray: ImmutableArraySegment<byte>
    | PinnedPointer of pointer: UnsafePinnedSpanPointer<byte>
        with
            member this.AsReadOnlySpan() : ReadOnlySpan<byte> =
                match this with
                | None -> ReadOnlySpan<byte>.Empty
                | Array v -> v.AsSpan()
                | ImmutableArray v -> v.AsSpan()
                | PinnedPointer v -> v.LoadReadOnlySpan()
            
            member this.UnsafeAsSpan() : Span<byte> =
                match this with
                | None -> Span<byte>.Empty
                | Array v -> v.AsSpan()
                | ImmutableArray v -> v.UnsafeAsSpan()
                | PinnedPointer v -> v.LoadSpan()

            interface IEquatable<OCamlByteSequenceSource> with
                member this.Equals (other: OCamlByteSequenceSource): bool = 
                    this.AsReadOnlySpan().SequenceEqual(other.AsReadOnlySpan())
            
            interface IComparable<OCamlByteSequenceSource> with
                member this.CompareTo (other: OCamlByteSequenceSource): int = 
                    this.AsReadOnlySpan().SequenceCompareTo(other.AsReadOnlySpan())

            override this.Equals (obj: obj): bool = 
                match obj with 
                | :? OCamlByteSequenceSource as v -> this.Equals(v)
                | _ -> false
            
            override this.GetHashCode (): int = 
                CommunityToolkit.HighPerformance.ReadOnlySpanExtensions.GetDjb2HashCode(this.AsReadOnlySpan())
        end

    [<RequireQualifiedAccess>]
    [<Struct>]
    type OCamlBytes = internal { Source: OCamlByteSequenceSource }

    [<RequireQualifiedAccess>]
    [<Struct>]
    type OCamlString = internal { Source: OCamlByteSequenceSource }

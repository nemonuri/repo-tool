namespace Nemonuri.OCamlDotNet.Primitives

open System

[<Interface>]
type ITemporaryReadOnlySpanSource<'T> =
    abstract member AsTemporarySpan: unit -> ReadOnlySpan<'T>

namespace Nemonuri.OCamlDotNet.Primitives.Operations

module TemporaryReadOnlySpanSources =

    type t<'a> = Nemonuri.OCamlDotNet.Primitives.ITemporaryReadOnlySpanSource<'a>

    let inline toReadOnlySpan (s: t<'a>) = s.AsTemporarySpan()

namespace Nemonuri.OCamlDotNet.Primitives

open System
open System.Diagnostics
open Nemonuri.OCamlDotNet.Primitives.Internals
module U = Nemonuri.OCamlDotNet.Primitives.Internals.Operations.UnsafeOCamlByteSpanSources

exception Not_found

type OCamlChar = Microsoft.FSharp.Core.byte
type OCamlInt = Microsoft.FSharp.Core.int

[<Struct>]
[<CustomEquality; CustomComparison>]
[<DebuggerDisplay("DebuggerDisplay,nq")>]
type OCamlByteSequenceSource = internal { UnsafeSource: UnsafeOCamlByteSpanSource }
    with

        interface IEquatable<OCamlByteSequenceSource> with
            member s.Equals (other: OCamlByteSequenceSource): bool = let s1 = s in Monad() { let! t1 = s1 in let! t2 = other in return! U.equal t1 t2 }
                
        
        interface IComparable<OCamlByteSequenceSource> with
            member s.CompareTo (other: OCamlByteSequenceSource): int = let s1 = s in Monad() { let! t1 = s1 in let! t2 = other in return! U.compare t1 t2 }
                
        
        interface IComparable with
            member s.CompareTo (obj: obj): int = 
                match obj with
                | null -> -1
                | :? OCamlByteSequenceSource as other -> (s :> IComparable<OCamlByteSequenceSource>).CompareTo(other)
                | _ -> invalidArg (nameof obj) "Cannot compare."

        override s.Equals (obj: obj): bool = 
            match obj with 
            | :? OCamlByteSequenceSource as v -> s.Equals(v)
            | _ -> false
        
        override s.GetHashCode (): int = let s1 = s in Monad() { let! t1 = s1 in return! U.hash t1 }

        member s.Slice (offset: int, length: int) : OCamlByteSequenceSource = let s1 = s in Monad() { let! t1 = s1 in return U.slice t1 offset length }

        interface ITemporaryReadOnlySpanSource<byte> with
            member s.AsTemporarySpan (): ReadOnlySpan<byte> = U.toReadOnlySpan s.UnsafeSource
            
        override s.ToString (): System.String = let s1 = s in Monad() { let! t1 = s1 in return! U.toString t1 }

    end

and private Monad =
        struct
            member inline this.ReturnFrom<'s>(s: 's) : 's = s
            member inline this.Return(s: UnsafeOCamlByteSpanSource) : OCamlByteSequenceSource = { UnsafeSource = s }
            member inline this.Bind<'s>(t: OCamlByteSequenceSource, sf: UnsafeOCamlByteSpanSource -> 's) : 's = t.UnsafeSource |> sf
        end

module Trs = Nemonuri.OCamlDotNet.Primitives.Operations.TemporaryReadOnlySpanSources

[<RequireQualifiedAccess>]
[<Struct>]
type OCamlBytes = internal { Source: OCamlByteSequenceSource } with
    interface ITemporaryReadOnlySpanSource<byte> with
        member this.AsTemporarySpan () = Trs.toReadOnlySpan this.Source

[<RequireQualifiedAccess>]
[<Struct>]
type OCamlString = internal { Source: OCamlByteSequenceSource } with
    interface ITemporaryReadOnlySpanSource<byte> with
        member this.AsTemporarySpan () = Trs.toReadOnlySpan this.Source

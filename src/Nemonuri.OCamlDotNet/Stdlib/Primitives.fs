namespace Nemonuri.OCamlDotNet.Primitives

open System
open Nemonuri.OCamlDotNet.Primitives.Internals
module U = Nemonuri.OCamlDotNet.Primitives.Internals.UnsafeOCamlByteSpanSources

[<Interface>]
type ITemporaryReadOnlySpanSource<'T> =
    abstract member AsTemporarySpan: unit -> ReadOnlySpan<'T>

module TemporaryReadOnlySpanSources =

    type t<'a> = ITemporaryReadOnlySpanSource<'a>

    let inline toReadOnlySpan (s: t<'a>) = s.AsTemporarySpan()


exception Not_found

type OCamlChar = Microsoft.FSharp.Core.byte
type OCamlInt = Microsoft.FSharp.Core.int

type TargetToSourceMonad<'TTarget, 'TSource> = 
    struct
        val MapTo : 'TTarget -> 'TSource
        val MapFrom : 'TSource -> 'TTarget
        new (mapTo: 'TTarget -> 'TSource, mapFrom: 'TSource -> 'TTarget) = { MapTo = mapTo; MapFrom = mapFrom }

        member inline this.ReturnFrom<'a>(s: 'a) : 'a = s
        member inline this.Return(t: 'TTarget) : 'TSource = this.MapTo t
        member inline this.Bind<'a>(s: 'TSource, [<InlineIfLambda>] tf: 'TTarget -> 'a) : 'a = s |> this.MapFrom |> tf
    end

[<Struct>]
[<CustomEquality; CustomComparison>]
type OCamlByteSpanSource = internal { UnsafeSource: UnsafeOCamlByteSpanSource }
    with
        static member internal Monad = OCamlByteSpanSourceMonad((fun t -> { UnsafeSource = t }),(fun s -> s.UnsafeSource))

        interface IEquatable<OCamlByteSpanSource> with
            member s.Equals (other: OCamlByteSpanSource): bool = let s1 = s in O.Monad { let! t1 = s1 in let! t2 = other in return! U.equal t1 t2 }
                
        
        interface IComparable<OCamlByteSpanSource> with
            member s.CompareTo (other: OCamlByteSpanSource): int = let s1 = s in O.Monad { let! t1 = s1 in let! t2 = other in return! U.compare t1 t2 }
                
        
        interface IComparable with
            member s.CompareTo (obj: obj): int = 
                match obj with
                | null -> 1
                | :? OCamlByteSpanSource as other -> (s :> IComparable<OCamlByteSpanSource>).CompareTo(other)
                | _ -> invalidArg (nameof obj) "Cannot compare."

        override s.Equals (obj: obj): bool = 
            match obj with 
            | :? OCamlByteSpanSource as v -> (s :> IEquatable<OCamlByteSpanSource>).Equals(v)
            | _ -> false
        
        override s.GetHashCode (): int = let s1 = s in O.Monad { let! t1 = s1 in return! U.hash t1 }

        interface ITemporaryReadOnlySpanSource<byte> with
            member s.AsTemporarySpan (): ReadOnlySpan<byte> = U.toReadOnlySpan s.UnsafeSource
            
        override s.ToString (): System.String = let s1 = s in O.Monad { let! t1 = s1 in return! U.toDotNetString t1 }

    end
and private OCamlByteSpanSourceMonad = TargetToSourceMonad<UnsafeOCamlByteSpanSource, OCamlByteSpanSource>
and private O = OCamlByteSpanSource


module Trs = TemporaryReadOnlySpanSources

[<Struct>]
type OCamlBytes = internal { Source: OCamlByteSpanSource } with
    interface ITemporaryReadOnlySpanSource<byte> with
        member this.AsTemporarySpan () = Trs.toReadOnlySpan this.Source

[<Struct>]
type OCamlString = internal { Source: OCamlByteSpanSource } with
    interface ITemporaryReadOnlySpanSource<byte> with
        member this.AsTemporarySpan () = Trs.toReadOnlySpan this.Source

[<Struct>]
[<NoEquality; NoComparison>]
type OCamlBuffer = internal { Value: Nemonuri.Collections.DrainableArrayBuilder<byte> }

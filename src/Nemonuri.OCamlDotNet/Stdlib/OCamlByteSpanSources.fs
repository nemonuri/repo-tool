namespace Nemonuri.OCamlDotNet.Primitives

open System
open Nemonuri.OCamlDotNet.Primitives



module OCamlByteSpanSources =

    type t = OCamlByteSpanSource
    type OCamlBytes = Nemonuri.OCamlDotNet.Primitives.OCamlBytes
    type OCamlString = Nemonuri.OCamlDotNet.Primitives.OCamlString
    module U = Nemonuri.OCamlDotNet.Primitives.Internals.UnsafeOCamlByteSpanSources

    let private mnd = OCamlByteSpanSource.Monad

    module Unsafe =

        let ofArraySegment s = mnd { return U.ofArraySegment s }

        let ofArray s = mnd { return U.ofArray s }

        let ofPinnedSpan (s: ReadOnlySpan<byte>) = { UnsafeSource = U.ofPinnedSpan s }

        let toSpan (s: t) = U.toSpan s.UnsafeSource

        //---- Bytes ---
        let toBytes (s: t) : OCamlBytes = { Source = s }

        let ofBytes (s: OCamlBytes) : t = s.Source

        let internal bmnd = TargetToSourceMonad<t, OCamlBytes>(toBytes, ofBytes)

        let bytesOfArraySegment s = bmnd { return ofArraySegment s }

        let bytesOfArray s = bmnd { return ofArray s }

        let bytesOfPinnedSpan s = ofPinnedSpan s |> toBytes

        let bytesToSpan (s: OCamlBytes) = toSpan (s |> ofBytes)        
        //---|

        //--- String ---
        let toString (s: t) : OCamlString = { Source = s }

        let ofString (s: OCamlString) : t = s.Source

        let internal smnd = TargetToSourceMonad<t, OCamlString>(toString, ofString)

        let stringOfArraySegment s = smnd { return ofArraySegment s }

        let stringOfArray s = smnd { return ofArray s }

        let stringOfPinnedSpan s = ofPinnedSpan s |> toString

        let stringToSpan (s: OCamlString) = toSpan (s |> ofString)        
        //---|

        let bytesToString (s: OCamlBytes) : OCamlString = s |> ofBytes |> toString

        let stringToBytes (s: OCamlString) : OCamlBytes = s |> ofString |> toBytes

    let toReadOnlySpan (s: t) = TemporaryReadOnlySpanSources.toReadOnlySpan s

    let referenceEqual (s1: t) (s2: t) = mnd { let! s1' = s1 in let! s2' = s2 in return! U.referenceEqual s1' s2' }

    let empty = mnd { return U.empty }

    let toArray (s: t) = (Unsafe.toSpan s).ToArray()

    let clone (s: t) = s |> toArray |> Unsafe.ofArray

    let equal (s1: t) (s2: t) = mnd { let! t1 = s1 in let! t2 = s2 in return! U.equal t1 t2 }

    let compare (s1: t) (s2: t) = mnd { let! t1 = s1 in let! t2 = s2 in return! U.compare t1 t2 }

    let hash (s: t) = mnd { let! t = s in return! U.hash t }

    let length (s: t) = mnd { let! t = s in return! U.length t }

    let slice (s: t) offset sliceLength = mnd { let! t = s in return U.slice t offset sliceLength }

    let toDotNetString (s: t) = mnd { let! t = s in return! U.toDotNetString t }

    let ofDotNetString (s: Core.string) = mnd { return U.ofDotNetString s }

    //---- Bytes ---
    let private bmnd = Unsafe.bmnd

    let bytesToReadOnlySpan (s: OCamlBytes) = toReadOnlySpan (s |> Unsafe.ofBytes)

    let bytesReferenceEqual s1 s2 = bmnd { let! s1' = s1 in let! s2' = s2 in return! referenceEqual s1' s2' }

    let emptyBytes = bmnd { return empty }

    let bytesToArray s = bmnd { let! t = s in return! toArray t }

    let cloneBytes s = bmnd { let! t = s in return clone t }

    let bytesEqual s1 s2 = bmnd { let! t1 = s1 in let! t2 = s2 in return! equal t1 t2 }

    let bytesCompare s1 s2 = bmnd { let! t1 = s1 in let! t2 = s2 in return! compare t1 t2 }

    let bytesHash s = bmnd { let! t = s in return! hash t }

    let bytesLength s = bmnd { let! t = s in return! length t }

    let bytesSlice s offset sliceLength = bmnd { let! t = s in return slice t offset sliceLength }

    let bytesToDotNetString s = bmnd { let! t = s in return! toDotNetString t }

    let bytesOfDotNetString s = bmnd { return ofDotNetString s }
    //---|

    //---- String ---
    let private smnd = Unsafe.smnd

    let stringToReadOnlySpan (s: OCamlString) = toReadOnlySpan (s |> Unsafe.ofString)

    let stringReferenceEqual s1 s2 = smnd { let! s1' = s1 in let! s2' = s2 in return! referenceEqual s1' s2' }

    let emptyString = smnd { return empty }

    let stringToArray s = smnd { let! t = s in return! toArray t }

    let cloneString s = smnd { let! t = s in return clone t }

    let stringEqual s1 s2 = smnd { let! t1 = s1 in let! t2 = s2 in return! equal t1 t2 }

    let stringCompare s1 s2 = smnd { let! t1 = s1 in let! t2 = s2 in return! compare t1 t2 }

    let stringHash s = smnd { let! t = s in return! hash t }

    let stringLength s = smnd { let! t = s in return! length t }

    let stringSlice s offset sliceLength = smnd { let! t = s in return slice t offset sliceLength }

    let stringToDotNetString s = smnd { let! t = s in return! toDotNetString t }

    let stringOfDotNetString s = smnd { return ofDotNetString s }
    //---|

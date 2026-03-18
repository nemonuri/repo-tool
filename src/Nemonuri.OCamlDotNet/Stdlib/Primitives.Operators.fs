#nowarn "86"

namespace Nemonuri.OCamlDotNet.Primitives

open System
open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Primitives.FormatBasics
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

module Operators =

    open Unchecked
    module S = Nemonuri.OCamlDotNet.Primitives.FormatBasics.Segments

    type Premise = struct

        static member ( == ) (x: OCamlString, y: OCamlString) = Obs.stringReferenceEqual x y

        static member ( == ) (x: OCamlBytes, y: OCamlBytes) = Obs.bytesReferenceEqual x y

        static member inline ( == ) (x: 'a, y: 'a) = LanguagePrimitives.PhysicalEquality x y


        static member ( ^ ) (x: OCamlString, y: OCamlString) : OCamlString = Forwarded.String.cat x y

        static member ( ^ ) (x: byte array, y: OCamlString) : OCamlString = Premise.(^)((Obs.Unsafe.stringOfArray x), y)

        static member ( ^ ) (x: byte array, y: byte array) : OCamlString = Premise.(^)(x, (Obs.Unsafe.stringOfArray y))

        static member ( ^ ) (x: OCamlFormat<'hd->'tl,'b,'c,'d>, y: OCamlString) : OCamlFormat<'hd->'tl,'b,'c,'d> = 
            let (S.FlattenSegment(le, fo, tr, tc)) = Formats.head x in
            let newHd = S.ofFlatten le fo (Forwarded.String.cat tr y) tc in
            Formats.replaceHead newHd x

        static member ( ^ ) (x: OCamlString, y: OCamlFormat<'hd->'tl,'b,'c,'d>) : OCamlFormat<'hd->'tl,'b,'c,'d> = 
            let (S.FlattenSegment(le, fo, tr, tc)) = Formats.head y in
            let newHd = S.ofFlatten (Forwarded.String.cat x le) fo tr tc in
            Formats.replaceHead newHd y

    end


    // Copy from: https://github.com/fsprojects/FSharp.Compatibility/blob/master/FSharp.Compatibility.OCaml/Pervasives.fs

    (*** Comparisons ***)

    /// e1 == e2 tests for physical equality of e1 and e2.
    let inline ( == ) x y =
        let inline call (p: ^p) (x': ^a) (y': ^a) = ((^p or ^a) : (static member (==): _*_ -> _) x', y')
        call defaultof<Premise> x y

    /// Negation of (==).
    let inline ( != ) x y =
        not <| (x == y)

    (*** Floating-point arithmetic ***)

    /// Unary negation.
    let inline ( ~-. ) (x : float) = -x

    /// Unary addition.
    let inline ( ~+. ) (x : float) = x

    /// Floating-point addition.
    let inline ( +. ) (x : float) (y : float) = x + y

    /// Floating-point subtraction.
    let inline ( -. ) (x : float) (y : float) = x - y

    /// Floating-point multiplication.
    let inline ( *. ) (x : float) (y : float) = x * y

    /// Floating-point division.
    let inline ( /. ) (x : float) (y : float) = x / y

    /// Exponentiation.
    let inline ( ** ) (x : float) (y : float) =
        Math.Pow (x, y)
    
    let inline ( ! ) (r: 'a ref) = r.Value

    let inline ( := ) (r: 'a ref) (a: 'a) = r.Value <- a

    let inline ( ^ ) x y =
        let inline call (p: ^p) (x': ^a) (y': ^b) = ((^p or ^a) : (static member ( ^ ): _*_ -> _) x', y')
        call defaultof<Premise> x y

    type OCamlString with
        member x.Item with get(i: OCamlInt) = Forwarded.String.get x i


    module Literals =

        type Premise =
            struct
                static member ( ~% ) (source: byte array) : OCamlString = Obs.Unsafe.stringOfArray source

                static member ( ~% ) (source: System.String) : OCamlString = Obs.stringOfDotNetString source

                static member ( ~% ) (source: unit -> OCamlFormat<_,_,_,_>) = source()
            end
        
        let inline ( ~% ) s =
            let inline call (p: ^p) (s': ^s) = ((^p or ^s) : (static member ( ~% ): ^s -> ^os) s') in
            call defaultof<Premise> s
        
        /// Constant pattern matching
        let inline (|C|_|) l r =
            let inline call (l': ^s) (r': ^os) = Core.Operators.(=) (%l') r' in
            match call l r with
            | true -> ValueSome r
            | false -> ValueNone
        
        /// Char range pattern matching
        let (|R|_|) (l: OCamlChar) (r: OCamlChar) (c: OCamlChar) = l <= c && c <= r
            

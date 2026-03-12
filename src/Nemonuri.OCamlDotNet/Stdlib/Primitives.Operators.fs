namespace Nemonuri.OCamlDotNet.Primitives

open System
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources

module Operators =

    // Copy from: https://github.com/fsprojects/FSharp.Compatibility/blob/master/FSharp.Compatibility.OCaml/Pervasives.fs

    (*** Comparisons ***)

    /// e1 == e2 tests for physical equality of e1 and e2.
    let inline ( == ) x y =
        LanguagePrimitives.PhysicalEquality x y

    /// Negation of (==).
    let inline ( != ) x y =
        not <| LanguagePrimitives.PhysicalEquality x y

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

    let inline ( ^ ) (x: OCamlString) (y: OCamlString) = Nemonuri.OCamlDotNet.Forwarded.String.cat x y

    module Literals =

        type Premise =
            struct
                static member ( ~% ) (source: byte array) : OCamlString = Obs.Unsafe.stringOfArray source
                static member ( ~% ) (source: System.String) : OCamlString = Obs.stringOfDotNetString source
            end
        
        let inline ( ~% ) s =
            let inline call (p: ^p) (s': ^s) = ((^p or ^s) : (static member ( ~% ): ^s -> ^os) s') in
            call Unchecked.defaultof<Premise> s
        
        /// Constant pattern matching
        let inline (|C|_|) l r =
            let inline call (l': ^s) (r': ^os) = Core.Operators.(=) (%l') r' in
            match call l r with
            | true -> ValueSome r
            | false -> ValueNone

module TypeShadowing =

    type char = OCamlChar

    type string = OCamlString
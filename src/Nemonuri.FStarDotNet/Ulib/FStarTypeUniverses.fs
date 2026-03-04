namespace Nemonuri.FStarDotNet.Primitives

open Nemonuri.FStarDotNet.Primitives.Operations
module A = Nemonuri.FStarDotNet.Primitives.Abbreviations


module FStarTypeUniverses =

    module Boxed =

        type Type = FStarOmega
        type typ = A.tc
        type type0 = A.tc<FStarOmega, FStarObject>
        type Type0 = A.Tc<FStarOmega, FStarObject>


    type Type<'a> = A.Tc<Boxed.Type, 'a>
    type typ<'a> = A.tc<Boxed.Type, 'a>
    type type0<'a> = A.tc<Boxed.Type0, 'a>
    type Type0<'a> = A.Tc<Boxed.Type0, 'a>

    type EqType<'a when 'a : equality> = Type0<'a>
    

    let pur (x: 'a) : Type0<'a> = 
        let pur0 (v: 'a) : Boxed.Type0 = { Witness = v |> FStarTypeContexts.toObj; Pure = FStarTypeContexts.toOmega }
        { Witness = x; Pure = pur0 }

    let inline extract (ty0: Type0<'a>) : 'a = ty0.Witness

    let inline toBoxed (x: Type0<'a>) : Boxed.Type0 = x |> FStarTypeContexts.tail

    let zero = () |> pur

    type Monad =
        struct
            member inline this.Bind(t1: Type0<'s1>, [<InlineIfLambda>] sf: 's1 -> Type0<'s2>) : Type0<'s2> = sf (extract t1)
            member inline this.Return(s: 's1) : Type0<'s1> = pur s
            member inline this.ReturnFrom(s: Type0<'s1>) : Type0<'s1> = s
        end
    
    let monad = Monad()

    type ExtractorMonad = 
        struct
            member inline this.Bind(t1: Type0<'s1>, [<InlineIfLambda>] sf: 's1 -> Type0<'s2>) : Type0<'s2> = monad.Bind(t1, sf)
            member inline this.Return(s: 's1) : Type0<'s1> = monad.Return(s)
            member inline this.Run(s: Type0<'s1>) : 's1 = s |> extract
        end

    let emonad = ExtractorMonad()

    type Comonad =
        struct
            member inline this.Bind(t1: 's1, [<InlineIfLambda>] sf: Type0<'s1> -> 's2) : 's2 = sf (pur t1)
            member inline this.Return(s: Type0<'s1>) : 's1 = extract s
        end

    let comonad = Comonad()

    let type0ToKindSource (x: Type0<'a>) : FStarKindSource<Boxed.Type0, 'a> = { Witness = x.Witness }
    let type0OfKindSource (x: FStarKindSource<Boxed.Type0, 'a>) : Type0<'a> = x.Witness |> pur

    type Type0OfKindSourceMonad = 
        struct
            member inline this.Return(s1: FStarKindSource<Boxed.Type0, 's1>) : Type0<'s1> = type0OfKindSource s1
            member inline this.Bind(t1: Type0<'s1>, sf: FStarKindSource<Boxed.Type0, 's1> -> Type0<'s2>) : Type0<'s2> = type0ToKindSource t1 |> sf
        end

    type Type0ToKindSourceMonad = 
        struct
            member inline this.Return(s1: Type0<'s1>) : FStarKindSource<Boxed.Type0, 's1> = type0ToKindSource s1
            member inline this.Bind(t1: FStarKindSource<Boxed.Type0, 's1>, sf: Type0<'s1> -> FStarKindSource<Boxed.Type0, 's2>) : FStarKindSource<Boxed.Type0, 's2> = type0OfKindSource t1 |> sf
        end


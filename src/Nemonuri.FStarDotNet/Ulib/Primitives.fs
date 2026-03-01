namespace Nemonuri.FStarDotNet.Primitives

open System
open Nemonuri.FStarDotNet.Primitives.Abstractions
module Ftc = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarTypeContexts
module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues
module A = Nemonuri.FStarDotNet.Primitives.Abstractions.Abbreviations

[<Struct>]
type FStarOmega =
    struct
        interface IFStarFixedTypeContext<FStarOmega> with
            member this.ToTailTypeContext () = this
        interface IFStarTypeContext with
            member this.BoxedWitness: objnull = this |> box
            member this.BoxToTailTypeContext (): IFStarTypeContext = Ftc.boxTail this
    end

[<Struct>]
type FStarThunk<'TTail when 'TTail :> IFStarTypeContext> = 
    { Tail: 'TTail; Witness: objnull } with

    interface IFStarThunk<'TTail> with
        member this.ToTailTypeContext (): 'TTail = this.Tail
    interface IFStarTypeContext with
        member this.BoxedWitness = this.Witness
        member this.BoxToTailTypeContext (): IFStarTypeContext = Ftc.boxTail this


[<Struct>]
type FStarObject = { Witness: objnull } with
    interface IFStarFixedTypeContext<FStarObject> with
        member this.ToTailTypeContext (): FStarObject = this
    interface IFStarTypeContext with
        member this.BoxedWitness: objnull = this.Witness
        member this.BoxToTailTypeContext (): IFStarTypeContext = Ftc.boxTail this


[<Struct>]
type FStarTypeContext<'TTail, 'THead when 'TTail :> IFStarTypeContext> = 
    { Witness: 'THead; Pure: 'THead -> 'TTail; } with
    interface IFStarTypeContext<'TTail, 'THead> with
        member this.Witness = this.Witness
        member this.ToTailTypeContext (): 'TTail = Ftc.witness this |> this.Pure
    interface IFStarTypeContext with
        member this.BoxedWitness = Ftc.boxWitness this
        member this.BoxToTailTypeContext (): IFStarTypeContext = Ftc.boxTail this

#if false
[<Struct>]
type FStarBoxedValue<'TTail, 'TTerm when 'TTail :> IFStarTypeContext and 'TTerm :> IFStarTypeContext<'TTail, 'TTerm>> =
    { Tail: 'TTail; Witness: 'TTerm; Embedder: 'TTerm -> objnull } 
    with
        member private this.Base: FStarTypeContext<'TTail, 'TTerm> = { Tail = this.Tail; Witness = this.Witness }

        interface IFStarBoxedValue<'TTail, 'TTerm>
        interface IFStarTypeContext<'TTail, 'TTerm> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext () = this.Base |> Ftc.tail
        interface IFStarTypeContext with
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext () = this.Base |> Ftc.Boxed.tail
        interface IExtractable with
            member this.ExtractToBox () = this.Embedder this.Witness
    end


[<Struct>]
type FStarValue<'TTail, 'TTerm, 'TValue when 'TTail :> IFStarTypeContext and 'TTerm :> IFStarTypeContext<'TTail, 'TTerm>> =
    { Tail: 'TTail; Witness: 'TTerm; Embedder: 'TTerm -> 'TValue } 
    with
        member private this.Base: FStarBoxedValue<'TTail, 'TTerm> = 
            { Tail = this.Tail; Witness = this.Witness; Embedder = this.Embedder >> box }
        
        interface IFStarValue<'TTail, 'TTerm, 'TValue> with
            member this.Extract () = this.Embedder this.Witness
        interface IFStarTypeContext<'TTail, 'TTerm> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext () = this.Base |> Ftc.tail
        interface IFStarTypeContext with
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext () = this.Base |> Ftc.Boxed.tail
        interface IExtractable with
            member this.ExtractToBox () = this.Base |> Fv.Boxed.extract
    end

[<Struct>]
type FStarLiftedValue<[<ComparisonConditionalOn; EqualityConditionalOn>] 'TValue> = { Value: 'TValue } 
    with
        member private this.Base: FStarValue<FStarObject,FStarLiftedValue<'TValue>,'TValue> =
            { Tail = { Witness = this.Value |> box }; Witness = this; Embedder = function | { Value = v } -> v }

        interface IFStarTypeContext<FStarObject,FStarLiftedValue<'TValue>> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext (): FStarObject = this.Base |> Ftc.tail
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext (): IFStarTypeContext = this.Base |> Ftc.Boxed.tail

        interface IFStarValue<FStarObject,FStarLiftedValue<'TValue>,'TValue> with
            member this.Extract (): 'TValue = this.Value
            member this.ExtractToBox (): objnull = this.Base |> Fv.Boxed.extract
    end

#endif

[<Struct>]
type FStarKindContext<'TKind when 'TKind :> A.tc and 'TKind : unmanaged> =
    struct
        member private this.Base: FStarTypeContext<FStarOmega, 'TKind> = { Witness = Unchecked.defaultof<'TKind>; Pure = fun _ -> FStarOmega() }

        interface IFStarTypeContext<FStarOmega, 'TKind> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext () = this.Base |> Ftc.tail
        interface IFStarTypeContext with
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext () = this.Base |> Ftc.Boxed.tail
    end

#if false

[<Struct>]
type FStarFunction<'TTail, 'TSource, 'TTarget when 'TTail :> A.tc> =
    { Tail: 'TTail; Witness: 'TSource -> 'TTarget } 
    with
        member private this.Base: FStarTypeContext<_,_> = { Tail = this.Tail; Witness = this.Witness }

        interface IFStarTypeContext<'TTail, 'TSource -> 'TTarget> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext () = this.Base |> Ftc.tail
        interface IFStarTypeContext with
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext () = this.Base |> Ftc.Boxed.tail
    end


[<Struct>]
type FStarTypeFunction<'TKind, 'TSource when 'TKind :> A.tc and 'TKind : unmanaged> =
    struct
        member private this.Base: FStarFunction<FStarOmega, 'TSource, FStarKinds.KindSource<'TKind, 'TSource>> =
            { Tail = FStarOmega(); Witness = fun v -> { Witness = v } }

        interface IFStarTypeContext<FStarOmega, 'TSource -> FStarKinds.KindSource<'TKind, 'TSource>> with
            member this.Witness = this.Base |> Ftc.witness
            member this.ToTailTypeContext () = this.Base |> Ftc.tail
        interface IFStarTypeContext with
            member this.BoxedWitness = this.Base |> Ftc.Boxed.witness
            member this.BoxToTailTypeContext () = this.Base |> Ftc.Boxed.tail
    end


[<Struct>]
type FStarObjectType =
    interface IFStarObjectType with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarObjectType() |> box
        member this.GetWitness (d: outref<objnull>) = d <- Unchecked.defaultof<objnull>
    
    static member Tail = Ftc.Boxed.tail (FStarObjectType())
    static member Witness = Ftc.Boxed.tail (FStarObjectType())


[<Struct>]
type FStarObject(value: objnull) =
    member private _.Base = FStarValue<_,_>(FStarObjectType(), value)

    interface IFStarEmbeddableTerm<FStarObjectType,FStarObject> with
        member this.GetTailTypeContext (d: outref<FStarObjectType>) = d <- Ftc.tail this.Base
        member this.GetValue (d: outref<FStarObject>) = d <- this
        member this.GetWitness (d: outref<FStarObject>) = d <- Fv.value this
    interface IEmbeddable with
        member this.Embed (d: outref<objnull>) = d <- Fv.value this.Base
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box

    member inline private this.Strict : IFStarEmbeddableTerm<FStarObjectType,FStarObject> = this

    interface IFStarEmbeddableTerm<IFStarObjectType,FStarObject> with
        member this.GetTailTypeContext (d: outref<IFStarObjectType>) = d <- Ftc.tail this.Strict
        member this.GetValue (d: outref<FStarObject>) = d <- Fv.value this.Strict
        member this.GetWitness (d: outref<FStarObject>) = d <- Ftc.witness this.Strict

[<Struct>]
type FStarLiftedType<'TEmbed> =
    member private _.Base = FStarObjectType()

    interface A.tc<FStarObjectType, 'TEmbed> with
        member this.GetTailTypeContext (d: outref<FStarObjectType>) = d <- this.Base
        member this.GetWitness (d: outref<'TEmbed>): unit = d <- Unchecked.defaultof<'TEmbed>
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
    interface IFStarObjectType


[<Struct>]
type FStarLiftedValue<[<ComparisonConditionalOn; EqualityConditionalOn>] 'TEmbed>(value: 'TEmbed) =
    static member create (value: 'TEmbed) = FStarLiftedValue<'TEmbed>(value)

    member private _.Base = FStarValue<_,_>(FStarLiftedType<'TEmbed>(), value)

    interface IFStarEmbeddableTerm<FStarLiftedType<'TEmbed>,FStarLiftedValue<'TEmbed>,'TEmbed> with
        member this.GetTailTypeContext (d: outref<FStarLiftedType<'TEmbed>>) = d <- Ftc.tail this.Base
        member this.GetValue (d: outref<FStarLiftedValue<'TEmbed>>): unit = d <- this
        member this.GetWitness (d: outref<FStarLiftedValue<'TEmbed>>) = d <- Fv.value this
        member this.Embed (d: outref<'TEmbed>) = d <- Fv.value this.Base
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box

    member private _.BoxedBase = FStarObject(value |> box)

    interface IFStarEmbeddableTerm<FStarObjectType,FStarObject> with
        member this.GetTailTypeContext (d: outref<FStarObjectType>) = d <- Ftc.tail this.BoxedBase
        member this.GetValue (d: outref<FStarObject>) = d <- Fv.value this.BoxedBase
        member this.GetWitness (d: outref<FStarObject>) = d <- Ftc.witness this.BoxedBase
    interface IEmbeddable with
        member this.Embed (d: outref<objnull>) = d <- Fv.Boxed.embed this.BoxedBase

    member inline private this.Strict : IFStarEmbeddableTerm<FStarLiftedType<'TEmbed>,FStarLiftedValue<'TEmbed>> = this

    interface IFStarEmbeddableTerm<IFStarObjectType,FStarLiftedValue<'TEmbed>> with
        member this.GetTailTypeContext (d: outref<IFStarObjectType>) = d <- Ftc.tail this.Strict
        member this.GetValue (d: outref<FStarLiftedValue<'TEmbed>>) = d <- this
        member this.GetWitness (d: outref<FStarLiftedValue<'TEmbed>>) = d <- this

    member inline private this.StrictBox : IFStarEmbeddableTerm<FStarObjectType,FStarObject> = this

    interface IFStarEmbeddableTerm<IFStarObjectType,FStarObject> with
        member this.GetTailTypeContext (d: outref<IFStarObjectType>) = d <- Ftc.tail this.StrictBox
        member this.GetValue (d: outref<FStarObject>) = d <- Fv.value this.StrictBox
        member this.GetWitness (d: outref<FStarObject>) = d <- Ftc.witness this.StrictBox
#endif

[<Struct>]
type FStarPair<'TP, 'TQ> = 
    | FStarPair of _1: 'TP * _2: 'TQ

[<Struct>]
type FStarSum<'TP, 'TQ> =
    | FStarSumLeft of left: 'TP
    | FStarSumRight of right: 'TQ

#if false
[<Struct>]
[<RequireQualifiedAccess>]
type DependentTypeProxy<'TSourceTypeContext, 'TSource, 'TTargetTypeContext, 'TImplication
                            when 'TSourceTypeContext :> A.tc
                            and 'TSource :> A.thunk<'TSourceTypeContext>
                            and 'TTargetTypeContext :> A.tc
                            and 'TImplication :> A.imp<'TSourceTypeContext, A.thunk<'TSourceTypeContext>, 'TTargetTypeContext>> = 
    {   Source: 'TSource;
        Implication: 'TImplication } with
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>): unit = d <- this.Source |> Ftc.Boxed.tail
        member this.GetWitness (d: outref<objnull>): unit = d <- null


[<Struct>]
type FStarDependentTuple<'TSourceTypeContext, 'TTypeImplication
                            when 'TTypeImplication :> A.imp<FStarOmega, 'TSourceTypeContext, FStarThunk<FStarOmega>>
                            and 'TTypeImplication : unmanaged> =
    {   BoxedSource: 'TSourceTypeContext;
        BoxedTarget: FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication> }

and [<Struct>]
    FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication
                                when 'TTypeImplication :> A.imp<FStarOmega, 'TSourceTypeContext, FStarThunk<FStarOmega>>
                                and 'TTypeImplication : unmanaged> =
    | Box of box: obj
    | Pointer of pointer: nativeint


[<Struct>]
type FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                        when 'TTypeImplication :> A.imp<FStarOmega, 'TSourceTypeContext, FStarThunk<FStarOmega>>
                                        and 'TTypeImplication : unmanaged> =
    {   Boxer: 'TTarget -> FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication>
        Unboxer: FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication> -> 'TTarget   }
#endif

type FStarTrivial = | FStarTrivial

type FStarEquals<'a, 'x, '_0> = | FStarRefl

[<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
type FStarTypeProxyAttribute(proxy: System.Type) = inherit Attribute()

[<AttributeUsage(AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
type FStarConstructorProxyAttribute(cons: Core.string) = inherit Attribute()

module Abbreviations =

    type tc = A.tc

    type thunk<'t when 't :> tc> = A.thunk<'t>

    type tc<'t, 'h when 't :> tc> = A.tc<'t, 'h>

    type Tc<'t, 'h when 't :> tc> = FStarTypeContext<'t, 'h>

    type term<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = A.term<'t, 'term>

    //type imp<'t, 'p, 'q when 't :> tc> = A.imp<'t, 'p, 'q>
    type kc<'k
                when 'k :> tc
                and 'k : unmanaged> = tc<FStarOmega,'k>

    type refine<'t> = A.refine<'t>

    type value<'t, 'term
                when 't :> tc
                and 'term :> term<'t, 'term>> = A.value<'t, 'term>

    type value<'t, 'term, 'v 
                when 't :> tc
                and 'term :> term<'t, 'term>> = A.value<'t, 'term, 'v>

    //type EqType<'a when 'a : equality> = FStarLiftedValue<'a>

    //type Fv<'a> = FStarLiftedValue<'a>


(*
module Prelude =
    
    open Abbreviations
    
    let inline impToArrow (imp: imp<'tc, 'p, 'q>) (p: 'p) : 'q = let q = imp.Invoke(p) in q

    let impToTerm<'tc, 'p, 'q when 'tc :> tc>(imp : imp<'tc, 'p, 'q>) : FStarValue<'tc, 'p -> 'q> =
        FStarValue<_,_>(imp.GetTailTypeContext(), impToArrow imp)




    let introAxiom<'tc, 'p when 'tc :> tc and 'p : unmanaged>(prev: 'tc) = FStarTypeContext<_,_>(Unchecked.defaultof<'p>, prev)

    let elemAxiom<'tc, 'p when 'tc :> tc and 'p : unmanaged>(tcp: tc<'tc, 'p>) = getTailTypeContext tcp

    let introWitness<'tc, 'w when 'tc :> tc> (witness: 'w) (prev: 'tc) =
        FStarTypeContext<_,_>(witness, prev)

    let elemWitness<'tc, 'w when 'tc :> tc>(tcw: tc<'tc, 'w>) = struct (getWitness tcw, getTailTypeContext tcw)
        

    let introForall<'tc, 'p, 'q, 'imp 
                        when 'tc :> tc 
                        and 'imp :> imp<'tc, 'p, 'q>
                        and 'imp : unmanaged>
        (prev: 'tc) = 
        introAxiom<_, 'imp> prev

    let elemForall<'tc, 'p, 'q, 'imp 
                        when 'tc :> tc 
                        and 'imp :> imp<'tc, 'p, 'q>
                        and 'imp : unmanaged>
        (tcimp: tc<'tc, 'imp>) = 
        elemAxiom<_, 'imp> tcimp

    let introExists<'tc, 'p, 'q, 'imp
                        when 'tc :> tc 
                        and 'imp :> imp<'tc, 'p, 'q voption>
                        and 'imp : unmanaged>
        (witness: 'p) (prev: 'tc) = 
        introAxiom<_, 'imp> prev
        |> introWitness witness 

    let elemExists<'tc, 'p, 'q, 'imp 
                        when 'tc :> tc 
                        and 'imp :> imp<'tc, 'p, 'q voption>
                        and 'imp : unmanaged>
        (tcimpP: tc<FStarTypeContext<'tc, 'imp>,'p>) = 
        let struct (witness, tcimp) = elemWitness<_, _> tcimpP
        struct (witness, elemAxiom<_,_> tcimp)
*)



(*
    [<Interface>]
    type IFStarTypeFunction<'tc, 'p when 'tc :> tc> =
        inherit tc<'tc, 'p -> DependentTypeProxy<'tc, 'p>>
        abstract member Invoke: 'p * outref<DependentTypeProxy<'tc, 'p>> -> unit
*)

(*
    /// Proxy type should implement target interface
    [<AttributeUsage(AttributeTargets.Interface)>]
    type FStarRefinementProxyAttribute(proxyType: System.Type) = inherit Attribute()
*)


[<AttributeUsage(AttributeTargets.All)>]
type unfold() = inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type opaque_to_smt() = inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type unopteq() = inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type inline_for_extraction() = inherit Attribute()
namespace Nemonuri.FStarDotNet.Primitives

open System
open Nemonuri.FStarDotNet.Primitives.Abstractions
module Ftc = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarTypeContexts
module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues
module A = Nemonuri.FStarDotNet.Primitives.Abstractions.Abbreviations


[<Struct>]
type FStarEmptyTypeContext = 
    interface IFStarEmptyTypeContext<FStarEmptyTypeContext> with
        member _.GetTailTypeContext (d: outref<objnull>): unit = d <- null
        member _.GetWitness (d: outref<objnull>): unit = d <- null

[<Struct>]
type FStarTypeContext<'TTail, 'THead when 'TTail :> IFStarTypeContext>(tail: 'TTail, witness: 'THead) = 
    interface IFStarTypeContext<'TTail, 'THead> with
        member _.GetWitness (d: outref<'THead>): unit = d <- witness
        member _.GetTailTypeContext (d: outref<'TTail>): unit = d <- tail
    interface IFStarTypeContext with
        member this.GetWitness (d: outref<objnull>): unit = d <- Ftc.witness this |> box
        member this.GetTailTypeContext (d: outref<objnull>): unit = d <- Ftc.tail this |> box

[<Struct>]
type FStarThunk<'TTail when 'TTail :> IFStarTypeContext>(tail: 'TTail, boxedWitness: objnull) =
    member private _.Base = FStarTypeContext<_,objnull>(tail, boxedWitness)

    interface IFStarTypeContextWithTail<'TTail> with
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.Boxed.witness this.Base
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.tail this.Base
        
[<Struct>]
type FStarTypeContext<'THead>(witness: 'THead) = 
    member private _.Base = FStarTypeContext<_,_>(FStarEmptyTypeContext(), witness)

    interface IFStarTypeContext<FStarEmptyTypeContext, 'THead> with
        member this.GetWitness (d: outref<'THead>) = d <- Ftc.witness this.Base
        member this.GetTailTypeContext (d: outref<FStarEmptyTypeContext>) = d <- Ftc.tail this.Base
    interface IFStarTypeContext with
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.Boxed.witness this.Base
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.tail this.Base


[<Struct>]
type FStarValue<'TTail, 'TValue when 'TTail :> A.tc>(tail: 'TTail, value: 'TValue) =
    member private _.Base = FStarTypeContext<_,_>(tail, value)

    interface A.value<'TTail, 'TValue> with
        member _.GetValue (d: outref<'TValue>) = d <- value
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- Ftc.tail this.Base
        member this.GetWitness(d: outref<'TValue>) = d <- Fv.value this
    interface A.tc with
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this.Base |> box
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.tail this.Base


[<Struct>]
type FStarDelayedValue<'TTail, 'TValue when 'TTail :> A.tc>(tail: 'TTail, delayed: 'TTail -> 'TValue) =
    member private _.Base = FStarTypeContext<_,'TValue>(tail, Unchecked.defaultof<_>)

    interface A.value<'TTail, 'TValue> with
        member _.GetValue (d: outref<'TValue>) = d <- delayed tail
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- Ftc.tail this.Base
        member this.GetWitness(d: outref<'TValue>) = d <- Fv.value this
    interface A.tc with
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this.Base |> box
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.witness this.Base


[<Struct>]
type FStarFunction<'TTail, 'TSource, 'TTarget when 'TTail :> A.tc>(tail: 'TTail, impl: 'TSource -> 'TTarget) =
    member private _.Base = FStarTypeContext<_,_>(tail, impl)

    interface A.imp<'TTail, 'TSource, 'TTarget> with
        member _.Invoke (p: 'TSource, d: outref<'TTarget>) = d <- impl p
    interface A.tc<'TTail, 'TSource -> 'TTarget> with
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- Ftc.tail this.Base
        member this.GetWitness (d: outref<('TSource -> 'TTarget)>) = d <- Ftc.witness this.Base
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.tail this.Base
        member this.GetWitness (d: outref<objnull>) = d <- Ftc.Boxed.witness this.Base


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

[<Struct>]
type FStarPair<'TP, 'TQ> = 
    | FStarPair of _1: 'TP * _2: 'TQ

[<Struct>]
type FStarSum<'TP, 'TQ> =
    | FStarSumLeft of left: 'TP
    | FStarSumRight of right: 'TQ

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
[<RequireQualifiedAccess>]
type FStarDependentTuple<'TSourceTypeContext, 'TTypeImplication
                            when 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                            and 'TTypeImplication : unmanaged> =
    {   BoxedSource: 'TSourceTypeContext;
        BoxedTarget: FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication> }

and [<Struct>]
    [<RequireQualifiedAccess>]
    FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication
                                when 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                and 'TTypeImplication : unmanaged> =
    | Box of box: obj
    | Pointer of pointer: nativeint


[<Struct>]
[<RequireQualifiedAccess>]
type FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                        when 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                        and 'TTypeImplication : unmanaged> =
    {   Boxer: 'TTarget -> FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication>
        Unboxer: FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication> -> 'TTarget   }


(*
type FStarDependentTypedValueSolver<'TSourceTypeContext, 'TTypeImplication, 'TTarget
                                        when 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                        and 'TTypeImplication : unmanaged> =
    FStarDependentTypedValue<'TSourceTypeContext, 'TTypeImplication> -> 'TTarget
*)

(*
[<Struct>]
[<RequireQualifiedAccess>]
type FStarDependentTupleSolution<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication, 'TSource, 'TTarget
                                    when 'TSourceTypeContext :> A.tc
                                    and 'TTargetTypeContext :> A.tc
                                    and 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, 'TTargetTypeContext>
                                    and 'TSource :> A.thunk<'TSourceTypeContext>
                                    and 'TTarget :> A.thunk<'TTargetTypeContext>> =
    {   Source: 'TSource;
        Implication: FStarFunction<'TTypeImplication, 'TSource, 'TTarget> }
*)

(*
[<Interface>]
type IFStarDependentTupleSolver<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication
                                    when 'TSourceTypeContext :> A.tc
                                    and 'TTargetTypeContext :> A.tc
                                    and 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, 'TTargetTypeContext>> =
    abstract member Solve: FStarDependentTuple<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication> -> 
                            FStarDependentTupleSolution<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication, A.thunk<'TSourceTypeContext>, A.thunk<'TTargetTypeContext>> ref -> unit
*)

[<Interface>]
type IFStarDependentTupleSolver<'TSourceTypeContext, 'TTypeImplication, 'TSource, 'TTarget
                                    when 'TTypeImplication :> A.imp<IFStarObjectType, 'TSourceTypeContext, IFStarTypeContext>
                                    and 'TTypeImplication : unmanaged> =
    inherit A.imp<IFStarObjectType, 
                    FStarDependentTuple<'TSourceTypeContext, 'TTypeImplication>, 
                    FStarPair<'TSource, 'TTarget>>
    // inherit IFStarDependentTupleSolver<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication>
    // abstract member Solve: FStarDependentTuple<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication> -> 
    //                        FStarDependentTupleSolution<'TSourceTypeContext, 'TTargetTypeContext, 'TTypeImplication, 'TSource, 'TTarget> ref -> unit




[<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
type FStarTypeProxyAttribute(proxy: System.Type) = inherit Attribute()

[<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
type FStarTypeInstanceConstructorAttribute(cons: Core.string) = inherit Attribute()

module Abbreviations =

    type tc = A.tc

    type tc<'t, 'h when 't :> tc> = A.tc<'t, 'h>

    type value<'t, 'v when 't :> tc> = A.value<'t, 'v>

    type thunk<'t when 't :> tc> = A.thunk<'t>

    type etc<'fix when 'fix :> etc<'fix>> = IFStarEmptyTypeContext<'fix>

    type Etc = FStarEmptyTypeContext

    type pureTc<'h> = tc<Etc, 'h>

    type Tc<'t, 'h when 't :> tc> = FStarTypeContext<'t, 'h>

    type Thunk<'t when 't :> tc> = FStarThunk<'t>

    type Thunk = Thunk<Etc>

    type term<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = A.term<'t, 'term>

    type imp<'t, 'p, 'q when 't :> tc> = A.imp<'t, 'p, 'q>

    type refine<'t when 't :> tc> = A.refine<'t>

    //type pureType<'a when 'a :> tc<'a>> = tc<'a>

    //type pureTerm<'a> = term<FStarEmptyTypeContext, 'a>

    //type pureType<'a when 'a :> pureTerm<'a>> = pureTerm<'a>

    //type pureValue<'a, 'c when 'a :> pureType<'a> and 'c :> pureTerm<'a> and 'c : unmanaged> = 'c

    type pureFuncType<'p, 'q> = imp<FStarEmptyTypeContext, 'p, 'q>

    type DType<'st, 's, 'tt, 'imp
                when 'st :> tc
                and 's :> thunk<'st>
                and 'tt :> tc
                and 'imp :> imp<'st, thunk<'st>, 'tt>> = DependentTypeProxy<'st, 's, 'tt, 'imp>

    type eterm<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = A.eterm<'t, 'term>

    type eterm<'t, 'term, 'v 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = A.eterm<'t, 'term, 'v>

    type dvalue<'st, 's, 'tt, 'imp, 't
                    when 'st :> tc
                    and 's :> thunk<'st>
                    and 'tt :> tc
                    and 'imp :> imp<'st, thunk<'st>, 'tt>
                    and 't :> thunk<'tt>> = A.dvalue<'st, 's, 'tt, 'imp, 't>

//    type eqtype<'a when 'a : equality> =
//        IFStarEmbeddableTerm<FStarLiftedType<'a>,FStarLiftedValue<'a>,'a>

    type EqType<'a when 'a : equality> = FStarLiftedValue<'a>

    type Fv<'a> = FStarLiftedValue<'a>


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
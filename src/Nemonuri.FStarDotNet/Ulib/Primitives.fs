namespace Nemonuri.FStarDotNet.Primitives

open System
open Nemonuri.FStarDotNet.Primitives.Abstractions
module FStarTypeContexts = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarTypeContexts
module FStarValues = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues
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
        member this.GetWitness (d: outref<objnull>): unit = d <- FStarTypeContexts.witness this |> box
        member this.GetTailTypeContext (d: outref<objnull>): unit = d <- FStarTypeContexts.tail this |> box

[<Struct>]
type FStarThunk<'TTail when 'TTail :> IFStarTypeContext>(tail: 'TTail, boxedWitness: objnull) =
    member private _.Base = FStarTypeContext<_,objnull>(tail, boxedWitness)

    interface IFStarTypeContextWithTail<'TTail> with
        member this.GetWitness (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.witness this.Base
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.tail this.Base
        
[<Struct>]
type FStarTypeContext<'THead>(witness: 'THead) = 
    member private _.Base = FStarTypeContext<_,_>(FStarEmptyTypeContext(), witness)

    interface IFStarTypeContext<FStarEmptyTypeContext, 'THead> with
        member this.GetWitness (d: outref<'THead>) = d <- FStarTypeContexts.witness this.Base
        member this.GetTailTypeContext (d: outref<FStarEmptyTypeContext>) = d <- FStarTypeContexts.tail this.Base
    interface IFStarTypeContext with
        member this.GetWitness (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.witness this.Base
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.tail this.Base


[<Struct>]
type FStarValue<'TTail, 'TValue when 'TTail :> A.tc>(tail: 'TTail, value: 'TValue) =
    member private _.Base = FStarTypeContext<_,_>(tail, value)

    interface A.value<'TTail, 'TValue> with
        member _.GetValue (d: outref<'TValue>) = d <- value
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- FStarTypeContexts.tail this.Base
        member this.GetWitness(d: outref<'TValue>) = d <- FStarValues.value this
    interface A.tc with
        member this.GetWitness (d: outref<objnull>) = d <- FStarTypeContexts.witness this.Base |> box
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.tail this.Base


[<Struct>]
type FStarDelayedValue<'TTail, 'TValue when 'TTail :> A.tc>(tail: 'TTail, delayed: 'TTail -> 'TValue) =
    member private _.Base = FStarTypeContext<_,'TValue>(tail, Unchecked.defaultof<_>)

    interface A.value<'TTail, 'TValue> with
        member _.GetValue (d: outref<'TValue>) = d <- delayed tail
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- FStarTypeContexts.tail this.Base
        member this.GetWitness(d: outref<'TValue>) = d <- FStarValues.value this
    interface A.tc with
        member this.GetWitness (d: outref<objnull>) = d <- FStarTypeContexts.witness this.Base |> box
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.witness this.Base


[<Struct>]
type FStarFunction<'TTail, 'TSource, 'TTarget when 'TTail :> A.tc>(tail: 'TTail, impl: 'TSource -> 'TTarget) =
    member private _.Base = FStarTypeContext<_,_>(tail, impl)

    interface A.imp<'TTail, 'TSource, 'TTarget> with
        member _.Invoke (p: 'TSource, d: outref<'TTarget>) = d <- impl p
    interface A.tc<'TTail, 'TSource -> 'TTarget> with
        member this.GetTailTypeContext (d: outref<'TTail>) = d <- FStarTypeContexts.tail this.Base
        member this.GetWitness (d: outref<('TSource -> 'TTarget)>) = d <- FStarTypeContexts.witness this.Base
    interface A.tc with
        member this.GetTailTypeContext (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.tail this.Base
        member this.GetWitness (d: outref<objnull>) = d <- FStarTypeContexts.Boxed.witness this.Base


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
        member this.GetTailTypeContext (d: outref<objnull>): unit = d <- this.Source |> FStarTypeContexts.Boxed.tail
        member this.GetWitness (d: outref<objnull>): unit = d <- null



[<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
type FStarTypeProxyAttribute(proxy: System.Type) = inherit Attribute()

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

    type eterm<'t, 'term, 'v 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = A.eterm<'t, 'term, 'v>

    type dvalue<'st, 's, 'tt, 'imp, 't
                    when 'st :> tc
                    and 's :> thunk<'st>
                    and 'tt :> tc
                    and 'imp :> imp<'st, thunk<'st>, 'tt>
                    and 't :> thunk<'tt>> = A.dvalue<'st, 's, 'tt, 'imp, 't>


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
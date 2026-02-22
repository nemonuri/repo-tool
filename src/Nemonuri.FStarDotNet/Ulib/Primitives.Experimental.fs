namespace Nemonuri.FStarDotNet.Primitives

open System

module Experimental =


    [<Interface>]
    type ITypeHead<'THead> =
        interface end

    [<Interface>]
    type ITypeTail<'TTail> =
        interface end
(*
    [<Interface>]
    type ITypeTailAndHead<'TTail, 'THead> =
        inherit ITypeHead<'THead>

    let tyHead (_: ITypeHead<'THead>) = typeof<'THead>

    let tyTail (_: ITypeTailAndHead<'TTail, 'THead>) = typeof<'TTail>
*)

    [<Interface>]
    [<CompiledName("IFStarTypeContext")>]
    type tc =
        abstract member GetWitness: outref<objnull> -> unit
        abstract member GetTailTypeContext: outref<objnull> -> unit
    
    [<Interface>]
    [<CompiledName("IFStarEmptyTypeContext")>]
    type etc =
        inherit tc

    [<Struct>]
    type FStarEmptyTypeContext = 
        interface etc with
            member _.GetTailTypeContext (d: outref<objnull>): unit = d <- null
            member _.GetWitness (d: outref<objnull>): unit = d <- null

    let emptyTc = FStarEmptyTypeContext()

    [<Interface>]
    type IFStarTypeContextWithTail<'t> =
        inherit tc
        inherit ITypeTail<'t>

    [<Interface>]
    [<CompiledName("IFStarTypeContext`2")>]
    type tc<'t, 'h when 't :> tc> =
        inherit ITypeHead<'h>
        inherit IFStarTypeContextWithTail<'t>
        abstract member GetWitness: outref<'h> -> unit
        abstract member GetTailTypeContext: outref<'t> -> unit
    
    let inline getWitness<'t, 'h when 't :> tc> (tc: tc<'t, 'h>) : 'h = tc.GetWitness()
    let inline getTailTypeContext<'t, 'h when 't :> tc> (tc: tc<'t, 'h>) : 't = tc.GetTailTypeContext()
    
    type tc<'h> = tc<FStarEmptyTypeContext, 'h>

(*
    [<Interface>]
    [<CompiledName("IFStarType`1")>]
    type ty<'tc when 'tc :> tc> =
        abstract member GetTypeContext: outref<'tc> -> unit
*)

    [<Struct>]
    type FStarTypeContext<'h>(witness: 'h) = 
        interface tc<'h> with
            member _.GetWitness (dest: outref<'h>): unit = dest <- witness
            member _.GetTailTypeContext (dest: outref<FStarEmptyTypeContext>): unit = dest <- emptyTc
        interface tc with
            member this.GetWitness (d: outref<objnull>): unit = d <- getWitness this
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- getTailTypeContext this

    [<Struct>]
    type FStarTypeContext<'t, 'h when 't :> tc>(witness: 'h, tailTc: 't) = 
        interface tc<'t, 'h> with
            member _.GetWitness (dest: outref<'h>): unit = dest <- witness
            member _.GetTailTypeContext (dest: outref<'t>): unit = dest <- tailTc
        interface tc with
            member this.GetWitness (d: outref<objnull>): unit = d <- getWitness this
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- getTailTypeContext this

    [<Interface>]
    [<CompiledName("IFStarValue`2")>]
    type term<'tc, 'v when 'tc :> tc> =
        inherit tc<'tc, 'v>
        abstract member GetValue: outref<'v> -> unit

    [<Struct>]
    type FStarValue<'tc, 'v when 'tc :> tc>(tc: 'tc, value: 'v) =
        interface term<'tc, 'v> with
            member _.GetValue (dest: outref<'v>): unit = dest <- value
            member _.GetTailTypeContext (d: outref<'tc>): unit = d <- tc
            member this.GetWitness(d: outref<'v>) = d <- (this :> term<'tc, 'v>).GetValue()
        interface tc with
            member this.GetWitness (d: outref<objnull>): unit = d <- getWitness this
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- getTailTypeContext this

    [<Struct>]
    type FStarDelayedValue<'tc, 'v when 'tc :> tc>(tc: 'tc, delayed: 'tc -> 'v) =
        interface term<'tc, 'v> with
            member _.GetValue (dest: outref<'v>): unit = dest <- delayed tc
            member _.GetTailTypeContext (d: outref<'tc>): unit = d <- tc
            member this.GetWitness(d: outref<'v>) = d <- (this :> term<'tc, 'v>).GetValue()
        interface tc with
            member this.GetWitness (d: outref<objnull>): unit = d <- getWitness this
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- getTailTypeContext this

    [<Interface>]
    [<CompiledName("IFStarFunction`3")>]
    type imp<'tc, 'p, 'q when 'tc :> tc> =
        inherit tc<'tc, 'p -> 'q>
        abstract member Invoke: 'p * outref<'q> -> unit


    [<Interface>]
    type IFStarFunctionGroup<'tc, 'p when 'tc :> tc> =
        abstract member TrySpecialize<'q>: 'p -> imp<'tc, 'p, 'q> option
    
    let inline impToArrow (imp: imp<'tc, 'p, 'q>) (p: 'p) : 'q = let q = imp.Invoke(p) in q

    let impToTerm<'tc, 'p, 'q when 'tc :> tc>(imp : imp<'tc, 'p, 'q>) : FStarValue<'tc, 'p -> 'q> =
        FStarValue<_,_>(imp.GetTailTypeContext(), impToArrow imp)

    [<Struct>]
    type FStarFunction<'tc, 'p, 'q when 'tc :> tc>(tc: 'tc, impl: 'p -> 'q) =
        interface imp<'tc, 'p, 'q> with
            member _.Invoke (p: 'p, qDest: outref<'q>): unit = qDest <- impl p
        interface tc<'tc, 'p -> 'q> with
            member this.GetTailTypeContext (d: outref<'tc>): unit = d <- impToTerm this |> getTailTypeContext
            member this.GetWitness (d: outref<('p -> 'q)>): unit = d <- impToTerm this |> getWitness
        interface tc with
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- getTailTypeContext this
            member this.GetWitness (d: outref<objnull>): unit = d <- getWitness this


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


    [<Struct>]
    [<RequireQualifiedAccess>]
    type DependentTypeProxy<'sourceTc, 'source, 'targetTc, 'imp
                                when 'sourceTc :> tc
                                and 'source :> tc<'sourceTc>
                                and 'targetTc :> tc
                                and 'imp :> imp<FStarEmptyTypeContext, 'sourceTc, 'targetTc>> = 
        { SourceTypeContext: 'sourceTc; 
          Source: 'source;
          TargetTypeContext: 'targetTc;
          Implication: 'imp } with
        interface tc with
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- this.SourceTypeContext.GetTailTypeContext()
            member this.GetWitness (d: outref<objnull>): unit = d <- null

(*
    [<Interface>]
    type IFStarTypeFunction<'tc, 'p when 'tc :> tc> =
        inherit tc<'tc, 'p -> DependentTypeProxy<'tc, 'p>>
        abstract member Invoke: 'p * outref<DependentTypeProxy<'tc, 'p>> -> unit
*)


    [<Interface>]
    [<CompiledName("IFStarRefinement`1")>]
    type refine<'T when 'T :> tc> = interface end

(*
    /// Proxy type should implement target interface
    [<AttributeUsage(AttributeTargets.Interface)>]
    type FStarRefinementProxyAttribute(proxyType: System.Type) = inherit Attribute()
*)

    /// position 0 means 'self'
    [<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = true)>]
    type TypeParameterExtensionAttribute(proxy: System.Type, position: int) = inherit Attribute()

    [<AttributeUsage(AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Class, AllowMultiple = false)>]
    type TypeConstraintExtensionAttribute(checker: System.Type) = inherit Attribute()

    module Aliases =

        type EmptyTc = FStarEmptyTypeContext

        type pureType<'a when 'a :> tc<'a>> = tc<'a>

        type pureTerm<'a> = term<FStarEmptyTypeContext, 'a>

        type pureValue<'a, 'c when 'a :> pureType<'a> and 'c :> pureTerm<'a> and 'c : unmanaged> = 'c

        type pureFuncType<'p, 'q> = imp<FStarEmptyTypeContext, 'p, 'q>

        type Ftc<'t, 'h when 't :> tc> = FStarTypeContext<'t, 'h>

        type tcWithTail<'t> = IFStarTypeContextWithTail<'t>

        type DType<'st, 's, 'tt, 'imp
                    when 'st :> tc
                    and 's :> tc<'st>
                    and 'tt :> tc
                    and 'imp :> imp<EmptyTc, 'st, 'tt>> = DependentTypeProxy<'st, 's, 'tt, 'imp>
namespace Nemonuri.FStarDotNet.Primitives

module Experimental =

(*
    [<Interface>]
    type ITypeHead<'THead> =
        interface end

    [<Interface>]
    type ITypeTailAndHead<'TTail, 'THead> =
        inherit ITypeHead<'THead>

    let tyHead (_: ITypeHead<'THead>) = typeof<'THead>

    let tyTail (_: ITypeTailAndHead<'TTail, 'THead>) = typeof<'TTail>
*)

    [<Interface>]
    [<CompiledName("IFStarTypeContext")>]
    type tc =
        interface end
    
    [<Interface>]
    [<CompiledName("IFStarEmptyTypeContext")>]
    type etc =
        inherit tc

    [<Struct>]
    type FStarEmptyTypeContext = interface etc

    let emptyTc = FStarEmptyTypeContext()

    [<Interface>]
    [<CompiledName("IFStarTypeContext`2")>]
    type tc<'t, 'h when 't :> tc> =
        inherit tc
        abstract member GetWitness: outref<'h> -> unit
        abstract member GetTailTypeContext: outref<'t> -> unit
    
    type tc<'h> = tc<FStarEmptyTypeContext, 'h>

    [<Interface>]
    [<CompiledName("IFStarType`1")>]
    type ty<'tc when 'tc :> tc> =
        abstract member GetTypeContext: outref<'tc> -> unit

    [<Interface>]
    [<CompiledName("IFStarValue`2")>]
    type term<'tc, 'v when 'tc :> tc> =
        inherit ty<'tc>
        abstract member GetValue: outref<'v> -> unit

    [<Struct>]
    type FStarTypeContext<'h>(witness: 'h) = 
        interface tc<'h> with
            member _.GetWitness (dest: outref<'h>): unit = dest <- witness
            member _.GetTailTypeContext (dest: outref<FStarEmptyTypeContext>): unit = dest <- emptyTc

    [<Struct>]
    type FStarTypeContext<'t, 'h when 't :> tc>(witness: 'h, tailTc: 't) = 
        interface tc<'t, 'h> with
            member _.GetWitness (dest: outref<'h>): unit = dest <- witness
            member _.GetTailTypeContext (dest: outref<'t>): unit = dest <- tailTc

    [<Struct>]
    type FStarValue<'tc, 'v when 'tc :> tc>(tc: 'tc, value: 'v) =
        interface term<'tc, 'v> with
            member _.GetTypeContext (dest: outref<'tc>): unit = dest <- tc
            member _.GetValue (dest: outref<'v>): unit = dest <- value

    [<Struct>]
    type FStarDelayedValue<'tc, 'v when 'tc :> tc>(tc: 'tc, delayed: 'tc -> 'v) =
        interface term<'tc, 'v> with
            member _.GetTypeContext (dest: outref<'tc>): unit = dest <- tc
            member _.GetValue (dest: outref<'v>): unit = dest <- delayed tc

    [<Interface>]
    [<CompiledName("IFStarFunction`3")>]
    type imp<'tc, 'p, 'q when 'tc :> tc> =
        inherit ty<'tc>
        abstract member Invoke: 'p * outref<'q> -> unit

(*
    [<Struct>]
    type FStarEnsuredResult<'tc, 'p, 'q, 'pt, 'imp 
                        when 'tc :> tc 
                        and 'pt :> term<'tc, 'p>
                        and 'pt : struct
                        and 'imp :> imp<'tc, 'pt, 'q voption>
                        and 'imp : unmanaged>
        (source: 'pt) =
        interface term<'tc, 'q> with
            member _.GetTypeContext (d: outref<'tc>): unit = d <- source.GetTypeContext()
            member _.GetValue (d: outref<'q>): unit = 
                let result = Unchecked.defaultof<'imp>.Invoke(source)
                d <- ValueOption.get result
*)

    [<Struct>]
    type FStarFunction<'tc, 'p, 'q when 'tc :> tc>(tc: 'tc, impl: 'p -> 'q) =
        interface imp<'tc, 'p, 'q> with
            member _.Invoke (p: 'p, qDest: outref<'q>): unit = 
                qDest <- impl p
            member _.GetTypeContext (dest: outref<'tc>): unit = dest <- tc

    /// modus ponens
    let apply<'tc, 'p, 'q when 'tc :> tc>
        (imp: imp<'tc, 'p, 'q>) (p: 'p) : term<'tc, 'q>
        =
        let qValue = imp.Invoke(p)
        FStarValue<'tc,_>(imp.GetTypeContext(), qValue)

    let introAxiom<'tc, 'p when 'tc :> tc and 'p : unmanaged>(prev: 'tc) = FStarTypeContext<_,_>(Unchecked.defaultof<'p>, prev)

    let elemAxiom<'tc, 'p when 'tc :> tc and 'p : unmanaged>(tcp: tc<'tc, 'p>) = 
        tcp.GetTailTypeContext()

    let introWitness<'tc, 'w when 'tc :> tc> (witness: 'w) (prev: 'tc) =
        FStarTypeContext<_,_>(witness, prev)

    let elemWitness<'tc, 'w when 'tc :> tc>(tcw: tc<'tc, 'w>) = 
        struct (tcw.GetWitness(), tcw.GetTailTypeContext())

    let introForall<'tc, 'p, 'q, 'pt, 'imp 
                        when 'tc :> tc 
                        and 'pt :> term<'tc, 'p>
                        and 'pt : struct
                        and 'imp :> imp<'tc, 'pt, 'q>
                        and 'imp : unmanaged>
        (prev: 'tc) = 
        introAxiom<_, 'imp> prev

    let elemForall<'tc, 'p, 'q, 'pt, 'imp 
                        when 'tc :> tc 
                        and 'pt :> term<'tc, 'p>
                        and 'pt : struct
                        and 'imp :> imp<'tc, 'pt, 'q>
                        and 'imp : unmanaged>
        (tcimp: tc<'tc, 'imp>) = 
        elemAxiom<_, 'imp> tcimp

    let introExists<'tc, 'p, 'q, 'pt, 'imp 
                        when 'tc :> tc 
                        and 'pt :> term<'tc, 'p>
                        and 'pt : struct
                        and 'imp :> imp<'tc, 'pt, 'q voption>
                        and 'imp : unmanaged>
        (witness: 'pt) (prev: 'tc) = 
        introAxiom<_, 'imp> prev
        |> introWitness witness 

    let elemExists<'tc, 'p, 'q, 'pt, 'imp 
                        when 'tc :> tc 
                        and 'pt :> term<'tc, 'p>
                        and 'pt : struct
                        and 'imp :> imp<'tc, 'pt, 'q voption>
                        and 'imp : unmanaged>
        (tcimpPt: tc<FStarTypeContext<'tc, 'imp>,'pt>) = 
        let struct (witness, tcimp) = elemWitness<_, _> tcimpPt
        struct (witness, elemAxiom<_,_> tcimp)

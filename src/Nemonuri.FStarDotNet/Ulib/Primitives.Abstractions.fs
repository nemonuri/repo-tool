namespace Nemonuri.FStarDotNet.Primitives.Abstractions


[<Interface>]
type ITypeHead<'THead> =
    interface end

[<Interface>]
type ITypeTail<'TTail> =
    interface end

[<Interface>]
type IEmbeddable =
    interface 
        abstract member Embed: d:outref<objnull> -> unit
    end

[<Interface>]
type IEmbeddable<'TTarget> =
    interface 
        abstract member Embed: d:outref<'TTarget> -> unit
    end

[<Interface>]
type IFStarTypeContext =
    abstract member GetWitness: d:outref<objnull> -> unit
    abstract member GetTailTypeContext: d:outref<objnull> -> unit

[<Interface>]
type IFStarTypeContextWithTail<'TTail> =
    inherit IFStarTypeContext
    inherit ITypeTail<'TTail>

[<Interface>]
type IFStarEmptyTypeContext<'TFix when 'TFix :> IFStarEmptyTypeContext<'TFix>> =
    inherit IFStarTypeContextWithTail<'TFix>

[<Interface>]
type IFStarObjectType = inherit IFStarEmptyTypeContext<IFStarObjectType>

[<Interface>]
type IFStarTypeContext<'TTail, 'THead when 'TTail :> IFStarTypeContext> =
    inherit ITypeHead<'THead>
    inherit IFStarTypeContextWithTail<'TTail>
    abstract member GetWitness: d:outref<'THead> -> unit
    abstract member GetTailTypeContext: d:outref<'TTail> -> unit

[<Interface>]
type IFStarValue<'TTail, 'TValue when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext<'TTail, 'TValue>
    abstract member GetValue: d:outref<'TValue> -> unit

[<Interface>]
type IFStarTerm<'TTail, 'TTerm when 'TTail :> IFStarTypeContext and 'TTerm :> IFStarTerm<'TTail, 'TTerm>> =
    inherit IFStarValue<'TTail, 'TTerm>



[<Interface>]
type IFStarEmbeddableTerm<'TTail, 'TTerm
                            when 'TTail :> IFStarTypeContext 
                            and 'TTerm :> IFStarTerm<'TTail, 'TTerm>> =
    inherit IFStarTerm<'TTail, 'TTerm>
    inherit IEmbeddable

[<Interface>]
type IFStarEmbeddableTerm<'TTail, 'TTerm, 'TValue 
                            when 'TTail :> IFStarTypeContext 
                            and 'TTerm :> IFStarTerm<'TTail, 'TTerm>> =
    inherit IFStarEmbeddableTerm<'TTail, 'TTerm>
    inherit IEmbeddable<'TValue>

[<Interface>]
type IFStarFunction<'TTail, 'TSource, 'TTarget when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext<'TTail, 'TSource -> 'TTarget>
    abstract member Invoke: s:'TSource * d:outref<'TTarget> -> unit

[<Interface>]
type IFStarRefinement<'T when 'T :> IFStarTypeContext> = interface end

[<Interface>]
type IFStarDependentValue<'TSourceTypeContext, 'TSource, 'TTargetTypeContext, 'TImplication, 'TTarget
                            when 'TSourceTypeContext :> IFStarTypeContext
                            and 'TSource :> IFStarTypeContextWithTail<'TSourceTypeContext>
                            and 'TTargetTypeContext :> IFStarTypeContext
                            and 'TImplication :> IFStarFunction<'TSourceTypeContext, IFStarTypeContextWithTail<'TSourceTypeContext>, 'TTargetTypeContext>
                            and 'TTarget :> IFStarTypeContextWithTail<'TTargetTypeContext>> = 
    inherit IFStarValue<'TTargetTypeContext, 'TTarget>


module Abbreviations =

    type tc = IFStarTypeContext

    type thunk<'t when 't :> tc> = IFStarTypeContextWithTail<'t>

    type tc<'t, 'h when 't :> tc> = IFStarTypeContext<'t, 'h>

    type value<'t, 'v when 't :> tc> = IFStarValue<'t, 'v>

    type term<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = IFStarTerm<'t, 'term>

    type imp<'t, 'p, 'q when 't :> tc> = IFStarFunction<'t, 'p, 'q>

    type refine<'t when 't :> tc> = IFStarRefinement<'t>

    type eterm<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = IFStarEmbeddableTerm<'t, 'term>

    type eterm<'t, 'term, 'v 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = IFStarEmbeddableTerm<'t, 'term, 'v>

    type dvalue<'st, 's, 'tt, 'imp, 't
                    when 'st :> tc
                    and 's :> thunk<'st>
                    and 'tt :> tc
                    and 'imp :> imp<'st, thunk<'st>, 'tt>
                    and 't :> thunk<'tt>> = IFStarDependentValue<'st, 's, 'tt, 'imp, 't>
    

module A = Abbreviations

module FStarTypeContexts =

    module Boxed =

        let witness (tc: A.tc) = let r: objnull = tc.GetWitness() in r

        let tail (tc: A.tc) = let r: objnull = tc.GetTailTypeContext() in r
    
    let witness (tc: A.tc<_,'h>) = let r: 'h = tc.GetWitness() in r

    let tail (tc: A.tc<'t,_>) = let r: 't = tc.GetTailTypeContext() in r
    
module FStarValues =

    module Boxed =

        let embed (term: A.eterm<_,_>) = let r: objnull = term.Embed() in r    

    let value (term: A.value<_,'v>) = let r: 'v = term.GetValue() in r

    let embed (term: A.eterm<_,_,'v>) = let r: 'v = term.Embed() in r

module FStarFunctions =

    let toArrow (imp: A.imp<'t, 'p, 'q>) (p: 'p) = let r: 'q = imp.Invoke(p) in r

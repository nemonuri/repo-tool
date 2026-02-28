namespace Nemonuri.FStarDotNet.Primitives.Abstractions


[<Interface>]
type ITypeHead<'THead> =
    interface end

[<Interface>]
type ITypeTail<'TTail> =
    interface end

[<Interface>]
type IExtractable =
    interface 
        abstract member EmbedToBox: unit -> objnull
    end

[<Interface>]
type IExtractable<'TTarget> =
    interface 
        inherit IExtractable
        abstract member Embed: unit -> 'TTarget
    end

[<Interface>]
type IFStarTypeContext =
    abstract member BoxedWitness: objnull
    abstract member BoxToTailTypeContext: unit -> IFStarTypeContext

[<Interface>]
type IFStarThunk<'TTail when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext
    inherit ITypeTail<'TTail>
    abstract member ToTailTypeContext: unit -> 'TTail

[<Interface>]
type IFStarFixedTypeContext<'TFix when 'TFix :> IFStarFixedTypeContext<'TFix>> =
    inherit IFStarThunk<'TFix>


[<Interface>]
type IFStarWitnessed<'THead> =
    inherit ITypeHead<'THead>
    abstract member Witness: 'THead

[<Interface>]
type IFStarTypeContext<'TTail, 'THead when 'TTail :> IFStarTypeContext> =
    inherit IFStarThunk<'TTail>
    inherit IFStarWitnessed<'THead>
    
(*
[<Interface>]
type IFStarValue<'TTail, 'TValue when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext<'TTail, 'TValue>
    abstract member GetValue: d:outref<'TValue> -> unit
*)


[<Interface>]
type IFStarBoxedValue<'TTail, 'TTerm 
                    when 'TTail :> IFStarTypeContext
                    and 'TTerm :> IFStarTypeContext<'TTail, 'TTerm>> =
    inherit IExtractable

[<Interface>]
type IFStarValue<'TTail, 'TTerm, 'TValue 
                            when 'TTail :> IFStarTypeContext
                            and 'TTerm :> IFStarTypeContext<'TTail, 'TTerm>> =
    inherit IFStarBoxedValue<'TTail, 'TTerm>
    inherit IExtractable<'TValue>

(*
[<Interface>]
type IFStarTerm<'TTail, 'TTerm when 'TTail :> IFStarTypeContext and 'TTerm :> IFStarTerm<'TTail, 'TTerm>> =
    inherit IFStarTypeContext<'TTail, 'TTerm>
*)

(*
[<Interface>]
type IFStarEmbeddableTerm<'TTail, 'TTerm
                            when 'TTail :> IFStarTypeContext 
                            and 'TTerm :> IFStarTerm<'TTail, 'TTerm>> =
    inherit IFStarTerm<'TTail, 'TTerm>
    inherit IEmbeddable
*)

(*
[<Interface>]    
type IFStarFunction<'TWeakSource, 'TSource, 'TTarget, 'TStrongTarget
                        when 'TWeakSource :> IFStarTypeContext
                        and 'TSource :> IFStarThunk<'TWeakSource>
                        and 'TTarget :> IFStarTypeContext
                        and 'TStrongTarget :> IFStarThunk<'TTarget>> =
    inherit IFStarTypeContext<'TWeakSource -> 'TStrongTarget, 'TSource -> 'TTarget>
*)

(*
[<Interface>]
type IFStarRefinement<'T when 'T :> IFStarTypeContext> = interface end
*)

[<Interface>]
type IFStarRefinement<'T> = interface end

(*
[<Interface>]
type IFStarDependentValue<'TSourceTypeContext, 'TSource, 'TTargetTypeContext, 'TImplication, 'TTarget
                            when 'TSourceTypeContext :> IFStarTypeContext
                            and 'TSource :> IFStarTypeContextWithTail<'TSourceTypeContext>
                            and 'TTargetTypeContext :> IFStarTypeContext
                            and 'TImplication :> IFStarFunction<'TSourceTypeContext, IFStarTypeContextWithTail<'TSourceTypeContext>, 'TTargetTypeContext>
                            and 'TTarget :> IFStarTypeContextWithTail<'TTargetTypeContext>> = 
    inherit IFStarValue<'TTargetTypeContext, 'TTarget>
*)

module Abbreviations =

    type tc = IFStarTypeContext

    type thunk<'t when 't :> tc> = IFStarThunk<'t>

    type tc<'t, 'h when 't :> tc> = IFStarTypeContext<'t, 'h>

    type term<'t, 'term 
                when 't :> tc 
                and 'term :> term<'t, 'term>> = tc<'t, 'term>

    //type imp<'t, 'p, 'q when 't :> tc> = IFStarFunction<'t, 'p, 'q>

    type refine<'t> = IFStarRefinement<'t>

    type value<'t, 'term
                when 't :> tc
                and 'term :> term<'t, 'term>> = IFStarBoxedValue<'t, 'term>

    type value<'t, 'term, 'v 
                when 't :> tc
                and 'term :> term<'t, 'term>> = IFStarValue<'t, 'term, 'v>

    /// Target context changer
    type tcc<'t when 't :> tc> = 't -> objnull -> 't


module A = Abbreviations

module FStarTypeContexts =

    module Boxed =

        let witness (tc: A.tc) = tc.BoxedWitness

        let tail (tc: A.tc) = tc.BoxToTailTypeContext()
    
    let witness (tc: A.tc<_,'h>) = tc.Witness

    let tail (tc: A.thunk<'t>) = tc.ToTailTypeContext()

    let inline boxWitness (tc: A.tc<_,'h>) = witness tc |> box

    let inline boxTail (tc: #A.tc) : A.tc = tc |> unbox
    
module FStarValues =

    module Boxed =

        let embed (term: A.value<_,_>) = term.EmbedToBox()

    let embed (term: A.value<_,_,'v>) = term.Embed()

    let boxEmbed (term: A.value<_,_,'v>): objnull = embed term |> box

(*
module FStarFunctions =

    let toArrow (imp: A.imp<'t, 'p, 'q>) (p: 'p) = imp.Witness p
*)

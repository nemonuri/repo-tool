namespace Nemonuri.FStarDotNet.Primitives.Abstractions


[<Interface>]
type ITypeHead<'THead> =
    interface end

[<Interface>]
type ITypeTail<'TTail> =
    interface end

[<Interface>]
type IFStarTypeContext =
    abstract member GetWitness: outref<objnull> -> unit
    abstract member GetTailTypeContext: outref<objnull> -> unit

[<Interface>]
type IFStarEmptyTypeContext =
    inherit IFStarTypeContext

[<Interface>]
type IFStarTypeContextWithTail<'TTail> =
    inherit IFStarTypeContext
    inherit ITypeTail<'TTail>

[<Interface>]
type IFStarTypeContext<'TTail, 'THead when 'TTail :> IFStarTypeContext> =
    inherit ITypeHead<'THead>
    inherit IFStarTypeContextWithTail<'TTail>
    abstract member GetWitness: outref<'THead> -> unit
    abstract member GetTailTypeContext: outref<'TTail> -> unit

[<Interface>]
type IFStarValue<'TTail, 'TValue when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext<'TTail, 'TValue>
    abstract member GetValue: outref<'TValue> -> unit

(*
[<Interface>]
[<CompiledName("IFStarFunction`3")>]
type imp<'tc, 'p, 'q when 'tc :> tc> =
    inherit tc<'tc, 'p -> 'q>
    abstract member Invoke: 'p * outref<'q> -> unit
*)

[<Interface>]
type IFStarFunction<'TTail, 'TSource, 'TTarget when 'TTail :> IFStarTypeContext> =
    inherit IFStarTypeContext<'TTail, 'TSource -> 'TTarget>
    abstract member Invoke: 'TSource * outref<'TTarget> -> unit

(*
[<Interface>]
type IFStarFunctionGroup<'tc, 'p when 'tc :> tc> =
    abstract member TrySpecialize<'q>: 'p -> imp<'tc, 'p, 'q> option
*)

[<Interface>]
type IFStarRefinement<'T when 'T :> IFStarTypeContext> = interface end

module Abbreviations =

    type tc = IFStarTypeContext

    type tc<'t, 'h when 't :> tc> = IFStarTypeContext<'t, 'h>

    type term<'t, 'v when 't :> tc> = IFStarValue<'t, 'v>

    type imp<'t, 'p, 'q when 't :> tc> = IFStarFunction<'t, 'p, 'q>

module A = Abbreviations

module TypeContexts =

    module Boxed =

        let witness (tc: A.tc) = let r: objnull = tc.GetWitness() in r

        let tail (tc: A.tc) = let r: objnull = tc.GetTailTypeContext() in r
    
    let witness (tc: A.tc<_,'h>) = let r: 'h = tc.GetWitness() in r

    let tail (tc: A.tc<'t,_>) = let r: 't = tc.GetTailTypeContext() in r
    
module Values =

    let value (term: A.term<_,'v>) = let r: 'v = term.GetValue() in r

module Functions =

    let toArrow (imp: A.imp<'t, 'p, 'q>) (p: 'p) = let r: 'q = imp.Invoke(p) in r

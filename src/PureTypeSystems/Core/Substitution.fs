namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Primitives.Expressions
open Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel
open type Nemonuri.PureTypeSystems.Primitives.Expressions.SubstitutionTheory

module Substitution =

    open Unchecked

    let inline callWith (x: 'a) (opt: voption<'a -> 'b>) ([<InlineIfLambda>] fallback: 'a -> 'b) =
        match opt with
        | ValueSome f -> f x
        | ValueNone -> fallback x
    
    [<NoEquality; NoComparison>]
    type BasicPremise = struct

        static member Substitute (tmp: Var, x: 'a) = TypeLevelTheory.ToData(x)

        static member Substitute (tmp: Data<Var>, x: 'a) = SubstituteInDataLevel(tmp, x)

    end

    let inline basicSubstitute tmp x =
        let inline call (p: ^p) (tmp': ^tmp) (x': ^x) = ((^p or ^tmp) : (static member Substitute : _*_ -> _) tmp', x')
        call defaultof<BasicPremise> tmp x

    let inline private basicSubst x tmp = basicSubstitute tmp x

    let inline private ( ! ) (tmp: App<_, _>) = tmp.Expression

    [<NoEquality; NoComparison>]
    type DataLevelPremise = struct

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

        static member inline Substitute (tmp: App<_, _>, x: _, [<Struct>] ?opt: _ -> _) = 
            (fun x -> ! ! ! ! ! ! ! ! ! ! ! !tmp |> basicSubst x) |> callWith x opt

    end

    let inline dataSubstitute tmp x =
        let inline call (p: ^p) (tmp': ^tmp) (x': ^x) (o: ^o) = ((^p or ^tmp) : (static member Substitute : _*_*_ -> _) tmp', x', o)
        call defaultof<DataLevelPremise> tmp x ValueNone

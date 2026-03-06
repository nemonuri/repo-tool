namespace Nemonuri.FStarDotNet.Primitives.FStarTypeSystems

open Nemonuri.PureTypeSystems
module Pts = Nemonuri.PureTypeSystems.Operations

[<NoEquality; NoComparison>]
[<Struct>]
type FStarGhostType<'TKinds, 'TRefiner when 'TRefiner :> ITypeRefiner> = { Kinds: Kind<'TKinds>; Refiner: 'TRefiner; Witness: objnull }
    with
        member this.Check(x: 'T) = this.Refiner.GetCondition<'T>() x

        interface ITypeRefiner with
            member this.GetCondition (): Condition<'T> = this.Check<'T>
    end

[<NoEquality; NoComparison>]
[<Struct>]
type FStarType<'TKinds, 'TRefiner, 'TWitness when 'TRefiner :> ITypeRefiner> = { Kinds: Kind<'TKinds>; Refiner: 'TRefiner; Witness: 'TWitness }
    with
        member this.Check(_: 'T) = 
            let selfCond = Pts.toSelfCondition this.Witness in
            Pts.conditionAnd selfCond (this.Refiner.GetCondition<'TWitness>()) this.Witness

        interface ITypeRefiner with
            member this.GetCondition (): Condition<'T> = this.Check<'T>
    end


type Type0 = FStarGhostType<unit, Tautology>

type EqType<'a when 'a : equality> = FStarType<unit, Tautology, 'a>


module Operations =

    open TypeEquality

    let unitKind = Pts.unitKind

    let tautology = Pts.tautology

    let toFStarType kinds refiner witness = { Kinds = kinds; Refiner = refiner; Witness = witness }

    let toFStarEqType (s: 'a) : EqType<'a> = toFStarType unitKind tautology s

    let tryToDomain<'dom, 'x> (x: 'x) =
        match Teq.tryRefl<'x, 'dom> with
        | None -> None
        | Some v -> Teq.cast v x |> Some
    
    let toType0 (s: objnull) : Type0 = { Kinds = unitKind; Refiner = tautology; Witness = s }
    
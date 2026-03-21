namespace Nemonuri.FStarDotNet.Primitives.FStarTypeSystems

open Nemonuri.PureTypeSystems
open Nemonuri.PureTypeSystems.Primitives

#if false
type ISupportWitness<'T> =
    interface
        abstract member Witness: 'T
    end

[<NoEquality; NoComparison>]
[<Struct>]
type FStarGhostType<'TExpr, 'TRefiner when 'TRefiner :> IRefinerPremise<'TExpr> and 'TRefiner : unmanaged> = { Witness: TypeExpr<'TExpr> }
    with
        interface IRefinerPremise<'TExpr> with
            member _.Judge(pre, post) = Unchecked.defaultof<'TRefiner>.Judge(&pre, &post)

        interface ISupportWitness<TypeExpr<'TExpr>> with
            member this.Witness = this.Witness
    end

[<NoEquality; NoComparison>]
[<Struct>]
type FStarType<'TExpr, 'TRefiner when 'TRefiner :> IRefinerPremise<'TExpr> and 'TRefiner : unmanaged> = { Witness: 'TExpr }
    with
        interface IRefinerPremise<'TExpr> with
            member _.Judge(pre, post) = Unchecked.defaultof<'TRefiner>.Judge(&pre, &post)

        interface ISupportWitness<'TExpr> with
            member this.Witness = this.Witness
    end
#endif

//type FStarKind<'TExpr, 'TRefiner when 'TRefiner :> IRefinerPremise<TypeExpr<'TExpr>> and 'TRefiner : unmanaged> = Refined<TypeExpr<'TExpr>, 'TRefiner>

type Type0 = Kinds.Data

type EqType<'a when 'a : equality> = 'a


module Operations =

    open TypeEquality

    //let tautology = Pts.tautology

#if false
    let toFStarType (witness: 'e) (refiner: 'r) : FStarType<'e,'r> = { Witness = witness }

    let toFStarEqType (s: 'a) : EqType<'a> = toFStarType unitKind tautology s

    let tryToDomain<'dom, 'x> (x: 'x) =
        match Teq.tryRefl<'x, 'dom> with
        | None -> None
        | Some v -> Teq.cast v x |> Some
    
    let toType0 (s: objnull) : Type0 = { TypeExpr = unitKind; Refiner = tautology; Witness = s }
#endif

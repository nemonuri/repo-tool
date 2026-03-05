namespace Nemonuri.FStarDotNet.Primitives.FStarTypeSystems

open HCollections
open Nemonuri.PureTypeSystems


[<NoEquality; NoComparison>]
type FStarType<'TKinds, 'TDotNet, 'TRefiner when 'TRefiner :> ITypeRefiner<'TDotNet>> =
    struct
        val Kinds: HList<'TKinds>
        val Witness: 'TDotNet
        val Refiner: 'TRefiner
        new(kinds: _, witness: _, refiner: _) = { Kinds = kinds; Witness = witness; Refiner = refiner }
    end

[<NoEquality; NoComparison>]
type Tautology<'T> =
    struct
        member _.Check(_: 'T) : bool voption = ValueSome true

        interface ITypeRefiner<'T> with
            member this.Condition = this.Check
    end

type Type0<'a> = FStarType<unit, 'a, Tautology<'a>>

type EqType<'a when 'a : equality> = Type0<'a>


module Operations =

    let toFStar kinds witness refiner = FStarType<_,_,_>(kinds, witness, refiner)

    let tautology<'a> = Tautology<'a>()

    let toType0 s = toFStar HList.empty s tautology

    
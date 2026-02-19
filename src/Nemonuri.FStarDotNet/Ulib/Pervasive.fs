namespace Nemonuri.FStarDotNet

    open System;
    open Nemonuri.FStarDotNet.Primitives

    type FStarBool = IFStarInstance<bool>

    type FStarUnit = IFStarInstance<unit>

    type FStarInt = IFStarInstance<bigint>

    type logical = Nemonuri.FStarDotNet.Primitives.ILogical

    type typeFunc<'a> = Nemonuri.FStarDotNet.Primitives.IDependentTypePremise<'a>

    type elem<'a> = IEquatable<'a>

    [<MeasureAnnotatedAbbreviation>]
    type App<'f, 'a, 'b when 'f : (member Invoke : 'a -> 'b)> = 'b

    [<AttributeUsage(AttributeTargets.All)>]
    type unfold() = inherit Attribute()

    [<AttributeUsage(AttributeTargets.All)>]
    type opaque_to_smt() = inherit Attribute()

    [<AttributeUsage(AttributeTargets.All)>]
    type unopteq() = inherit Attribute()
        
    [<Struct>]
    type DotNetToFStarApplicativePremise =
        member inline _.Return (x: 'TDotNet) = FStarInstance<_>(x)
        member inline this.Apply (f: IFStarInstance<'TDotNet1 -> 'TDotNet2>) (x: IFStarInstance<'TDotNet1>) = x.Value |> f.Value |> this.Return

    [<Struct>]
    type FStarToDotNetMonadPremise =
        member inline _.Return (x: IFStarInstance<'TDotNet>) = x.Value
        member inline _.Bind (x: 'TDotNet1) (f: IFStarInstance<'TDotNet1> -> 'TDotNet2) = DotNetToFStarApplicativePremise().Return x |> f


    [<AutoOpen>]
    module Pervasives =

        module internal InternalTheory =

            let inline introD2F() = DotNetToFStarApplicativePremise()

            let inline introF2D() = FStarToDotNetMonadPremise()

            let inline fret (x: 'd) = introD2F().Return x

            let inline dret (x: IFStarInstance<'d>) = introF2D().Return x

            let inline fapp<'d1, 'd2> (f: IFStarInstance<'d1 -> 'd2>) (x: IFStarInstance<'d1>) = introD2F().Apply f x

            let inline fmap (f: 'd1 -> 'd2) f1 = (fret f, f1) ||> fapp

            let inline fmap2 (f: 'd1 -> 'd2 -> 'd3) f1 f2 = (fmap f f1, f2) ||> fapp

            let inline fmap3 (f: 'd1 -> 'd2 -> 'd3 -> 'd4) f1 f2 f3 = (fmap2 f f1 f2, f3) ||> fapp


            let inline dbind<'d1, 'd2> (x: 'd1) (f: IFStarInstance<'d1> -> 'd2) = introF2D().Bind x f

            let inline dmap (f: IFStarInstance<'d1> -> IFStarInstance<'d2>) (d1: 'd1) = dbind d1 f |> dret

            let inline dmap2 (f: IFStarInstance<'d1> -> IFStarInstance<'d2> -> IFStarInstance<'d3>) d1 d2 = dbind d1 f |> dbind d2 |> dret

            let inline dmap3 (f: IFStarInstance<'d1> -> IFStarInstance<'d2> -> IFStarInstance<'d3> -> IFStarInstance<'d4>) d1 d2 d3 = dbind d1 f |> dbind d2 |> dbind d3 |> dret

            let inline curry2 (f: ('a * 'b) -> 'c) (a: 'a) (b: 'b) = f (a, b)

        module T = InternalTheory

        let inline fstarValueOfDotNetValue (v: 'DotNet) = T.fret v

        let inline fstarValueToDotNetValue (v: IFStarInstance<'DotNet>) = T.dret v
        

        let inline fstarBoolOfDotNetBool (v: bool) : FStarBool = fstarValueOfDotNetValue v

        let inline fstarBoolToDotNetBool (v: FStarBool) = fstarValueToDotNetValue v

        let inline fstarIntOfDotNetInt (v: bigint) : FStarInt = fstarValueOfDotNetValue v

        let inline fstarIntToDotNetInt (v: FStarInt) = fstarValueToDotNetValue v


        let inline fstarBoolLogicalAnd (b1: FStarBool) (b2: FStarBool) : FStarBool = ((&&) |> T.fmap2) b1 b2

        let inline fstarBoolLogicalOr (b1: FStarBool) (b2: FStarBool) : FStarBool = ((||) |> T.fmap2) b1 b2

        let inline fstarBoolLogicalNot (b1: FStarBool) : FStarBool = (not |> T.fmap) b1


        let inline fstarIntMultiply (i1: FStarInt) (i2: FStarInt) : FStarInt = (T.curry2 bigint.Multiply |> T.fmap2) i1 i2

        let inline fstarIntSubtraction (i1: FStarInt) (i2: FStarInt) : FStarInt = (T.curry2 bigint.Subtract |> T.fmap2) i1 i2





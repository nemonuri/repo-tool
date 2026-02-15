namespace Nemonuri.FStarDotNet

    open System

    type FStarBool = Nemonuri.FStarDotNet.Primitives.Bool

    type FStarUnit = Nemonuri.FStarDotNet.Primitives.Unit

    type FStarInt = bigint

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

    [<AutoOpen>]
    module Pervasives =

        let fstarBoolToDotNetBool (b: FStarBool) : bool = b.Equals(FStarBool.True.Singleton)

        let fstarBoolOfDotNetBool (b: bool) : FStarBool = 
            match b with
            | true -> FStarBool.True.Singleton
            | false -> FStarBool.False.Singleton

        let inline private bto o = fstarBoolToDotNetBool o

        let inline private bof o = fstarBoolOfDotNetBool o

        let fstarBoolLogicalAnd (b1: FStarBool) (b2: FStarBool) = (&&) (bto b1) (bto b2)

        let fstarBoolLogicalOr (b1: FStarBool) (b2: FStarBool) = (||) (bto b1) (bto b2)

        let fstarBoolLogicalNot b = bto b |> not |> bof

        let fstarIntMultiply (i1: FStarInt) (i2: FStarInt) : FStarInt = bigint.Multiply(i1, i2)

        let fstarIntSubtraction (i1: FStarInt) (i2: FStarInt) : FStarInt = bigint.Subtract(i1, i2)




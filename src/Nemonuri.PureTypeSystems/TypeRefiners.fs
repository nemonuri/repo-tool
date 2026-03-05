namespace Nemonuri.PureTypeSystems

module TypeRefiners =

    module P = Nemonuri.PureTypeSystems.Primitives

    let check (refiner: ITypeRefiner<'a>) x = P.check refiner.Condition x

    let tryExtract (refiner: ITypeRefiner<'a>) x = P.tryExtract refiner.Condition x

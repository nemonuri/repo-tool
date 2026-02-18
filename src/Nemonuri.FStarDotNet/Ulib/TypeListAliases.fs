namespace Nemonuri.FStarDotNet

    open Nemonuri.FStarDotNet.Primitives

    module TypeListAliases =

        type Empty = EmptyTypeList

        type Types<'t0> = TypeList<'t0, Empty>

        type Types<'t0, 't1> = TypeList<'t0, Types<'t1>>

        type Types<'t0, 't1, 't2> = TypeList<'t0, Types<'t1, 't2>>

        type Types<'t0, 't1, 't2, 't3> = TypeList<'t0, Types<'t1, 't2, 't3>>
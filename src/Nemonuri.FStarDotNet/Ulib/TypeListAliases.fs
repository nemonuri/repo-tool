namespace Nemonuri.FStarDotNet

    open Nemonuri.FStarDotNet.Primitives

    module TypeListAliases =

        type Empty = EmptyTypeList

(** ...대체 왜 이 조건들을 다 명시해야 하는데?? *)
        type tl<'h, 't when 't : unmanaged and 't :> ITypeList and 't : (new: unit -> 't) and 't : struct and 't :> System.ValueType> = 
            TypeList<'h, 't>
        
        type tl<'h> = tl<'h, Empty>

        type Types<'t0> = tl<'t0>

        type Types<'t0, 't1> = tl<'t0, Types<'t1>>

        type Types<'t0, 't1, 't2> = tl<'t0, Types<'t1, 't2>>

        type Types<'t0, 't1, 't2, 't3> = tl<'t0, Types<'t1, 't2, 't3>>
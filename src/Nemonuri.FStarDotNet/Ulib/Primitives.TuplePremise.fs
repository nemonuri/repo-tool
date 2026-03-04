namespace Nemonuri.FStarDotNet.Primitives


type TuplePremise = 
    struct
        static member inline AddItem(_: unit, r: 's2) = System.ValueTuple.Create(r)

        static member inline AddItem(l: System.ValueTuple<'s1>, r: 's2) = l.Item1, r

        static member inline AddItem(l: (_*_), r: _) = 
            match l with | _1,_2 -> _1,_2, r

        static member inline AddItem(l: (_*_*_), r: _) = 
            match l with | _1,_2,_3 -> _1,_2,_3, r

        static member inline AddItem(l: (_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4 -> _1,_2,_3,_4, r

        static member inline AddItem(l: (_*_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4,_5 -> _1,_2,_3,_4,_5, r

        static member inline AddItem(l: (_*_*_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4,_5,_6 -> _1,_2,_3,_4,_5,_6, r

        static member inline AddItem(l: (_*_*_*_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4,_5,_6,_7 -> _1,_2,_3,_4,_5,_6,_7, r

        static member inline AddItem(l: (_*_*_*_*_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8 -> _1,_2,_3,_4,_5,_6,_7,_8,r

        static member inline AddItem(l: (_*_*_*_*_*_*_*_*_), r: _) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,r

    end

namespace Nemonuri.FStarDotNet.Primitives.Operations

open Nemonuri.FStarDotNet.Primitives

module Tuples =

    let inline addItem l r =
        let inline call (p': ^p) (l': ^l) (r': ^r) = ((^p or ^l) : (static member AddItem: _*_ -> _) l', r') in
        call Unchecked.defaultof<TuplePremise> l r

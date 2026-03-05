namespace Nemonuri.PureTypeSystems.Primitives.Operations

module SingletonValueTuples =

    let toValueTuple x = System.ValueTuple.Create(x)

    let ofValueTuple (x: System.ValueTuple<'a>) = x.Item1

    let (|ValueTuple|) x = ofValueTuple x

namespace Nemonuri.PureTypeSystems.Primitives

open Operations.SingletonValueTuples

type TuplePremise = 
    struct
//--- Push ---

        static member inline Push(_: unit, r: 's2) = System.Tuple.Create(r)

        static member inline Push(l: System.Tuple<'s1>, r: 's2) = l.Item1, r

        static member inline Push((l: (_*_), r: _)) = 
            match l with | _1,_2 -> _1,_2, r

        static member inline Push((l: (_*_*_), r: _)) = 
            match l with | _1,_2,_3 -> _1,_2,_3, r

        static member inline Push((l: (_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4 -> _1,_2,_3,_4, r

        static member inline Push((l: (_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5 -> _1,_2,_3,_4,_5, r

        static member inline Push((l: (_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6 -> _1,_2,_3,_4,_5,_6, r

        static member inline Push((l: (_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7 -> _1,_2,_3,_4,_5,_6,_7, r

        static member inline Push((l: (_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8 -> _1,_2,_3,_4,_5,_6,_7,_8,r

        static member inline Push((l: (_*_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,r

        static member inline Push((l: (_*_*_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9,_10 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,r

        static member inline Push((l: (_*_*_*_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,r

        static member inline Push((l: (_*_*_*_*_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,r

        static member inline Push((l: (_*_*_*_*_*_*_*_*_*_*_*_*_), r: _)) = 
            match l with | _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13 -> _1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13,r

//---|

//--- Pop ---

        
        static member inline Pop(((), (l: _,r: _))) = l,r

        static member inline Pop(((), (l: (_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_*_*_*_*_), r: _))) = l,r

        static member inline Pop(((), (l: (_*_*_*_*_*_*_*_*_*_*_*_*_), r: _))) = l,r

//---|

//--- Dequeue ---

        ///static member inline Dequeue(l: System.Tuple<'a>) = l.Item1
        static member inline Dequeue(l,r) = l,r

        static member inline Dequeue(l: _, r: (_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_*_*_*_*_)) = l,r

        static member inline Dequeue(l: _, r: (_*_*_*_*_*_*_*_*_*_*_*_*_)) = l,r

//---|
    end


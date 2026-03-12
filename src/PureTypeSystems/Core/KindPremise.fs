namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Operations

type KindPremise =
    struct
//--- ToDotNet ---

        static member inline ToDotNet(_1: System.Tuple<_>) = _1.Item1
        static member inline ToDotNet(_1,_2) = toDotNet _1 _2
        static member inline ToDotNet(_1,_2,_3) = K.ToDotNet(_1, K.ToDotNet(_2,_3))
        static member inline ToDotNet(_1,_2,_3,_4) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4))
        static member inline ToDotNet(_1,_2,_3,_4,_5) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8,_9))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10) = K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11) = 
            K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12) = 
            K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13) = 
            K.ToDotNet(_1, K.ToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13))

//---|

//--- KindToDotNet ---

        static member inline KindToDotNet(Kind(_1: System.Tuple<_>)) = K.ToDotNet(_1)
        static member inline KindToDotNet(Kind(_1,_2)) = K.ToDotNet(_1,_2)
        static member inline KindToDotNet(Kind(_1,_2,_3)) = K.ToDotNet(_1,_2,_3)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4)) = K.ToDotNet(_1,_2,_3,_4)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5)) = K.ToDotNet(_1,_2,_3,_4,_5)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6)) = K.ToDotNet(_1,_2,_3,_4,_5,_6)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7)) = K.ToDotNet(_1,_2,_3,_4,_5,_6,_7)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8)) = K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8,_9)) = K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10)) = K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11)) = 
            K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12)) = 
            K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12)
        static member inline KindToDotNet(Kind(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13)) = 
            K.ToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13)

//---|
    end
and private K = KindPremise
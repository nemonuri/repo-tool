namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Kinds

type TypeExprPremise =
    struct
//--- TypeExprToDotNet ---

        static member inline TypeExprToDotNet(_1: Bracket<_>) = _1.Value
        static member inline TypeExprToDotNet(_1,_2) = cons _1 _2
        static member inline TypeExprToDotNet(_1,_2,_3) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3))
        static member inline TypeExprToDotNet(_1,_2,_3,_4) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8,_9))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10) = K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11) = 
            K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12) = 
            K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12))
        static member inline TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13) = 
            K.TypeExprToDotNet(_1, K.TypeExprToDotNet(_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13))

//---|

//--- ToDotNet ---

        static member inline ToDotNet(TypeExpr(_1: Bracket<_>)) = K.TypeExprToDotNet(_1)
        static member inline ToDotNet(TypeExpr(_1,_2)) = K.TypeExprToDotNet(_1,_2)
        static member inline ToDotNet(TypeExpr(_1,_2,_3)) = K.TypeExprToDotNet(_1,_2,_3)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4)) = K.TypeExprToDotNet(_1,_2,_3,_4)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8,_9)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10)) = K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11)) = 
            K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12)) = 
            K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12)
        static member inline ToDotNet(TypeExpr(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13)) = 
            K.TypeExprToDotNet(_1,_2,_3,_4,_5,_6,_7,_8,_9,_10,_11,_12,_13)

//---|
    end
and private K = TypeExprPremise
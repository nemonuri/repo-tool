namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Operations.Forwarded.Apps
open Nemonuri.PureTypeSystems.Operations.Kinds

type AppPremise =
    struct
//--- ToDotNet ---

        static member inline ToDotNet(x: System.Tuple<_>) = x.Item1
        static member inline ToDotNet(_1,_2) = toDotNet _1 _1 _2
        static member inline ToDotNet(_1,_2,_3) = A.ToDotNet(_1, A.ToDotNet(_2,_3))
        static member inline ToDotNet(_1,_2,_3,_4) = A.ToDotNet(_1, A.ToDotNet(_2,_3,_4))
        static member inline ToDotNet(_1,_2,_3,_4,_5) = A.ToDotNet(_1, A.ToDotNet(_2,_3,_4,_5))
        static member inline ToDotNet(_1,_2,_3,_4,_5,_6) = A.ToDotNet(_1, A.ToDotNet(_2,_3,_4,_5,_6))


//---|

//--- AppToDotNet ---

        static member inline AppToDotNet(App(_1: System.Tuple<'x>)) = A.ToDotNet(_1)
        static member inline AppToDotNet(App(_1,_2)) = A.ToDotNet(_1,_2)
        static member inline AppToDotNet(App(_1,_2,_3)) = A.ToDotNet(_1,_2,_3)
        static member inline AppToDotNet(App(_1,_2,_3,_4)) = A.ToDotNet(_1,_2,_3,_4)
        static member inline AppToDotNet(App(_1,_2,_3,_4,_5)) = A.ToDotNet(_1,_2,_3,_4,_5)
        static member inline AppToDotNet(App(_1,_2,_3,_4,_5,_6)) = A.ToDotNet(_1,_2,_3,_4,_5,_6)

//---|
    end
and private A = AppPremise
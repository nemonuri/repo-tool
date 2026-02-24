namespace Nemonuri.FStarDotNet.Primitives


module FStarTypes =

    exception InvalidInhabitant of System.Type

    let raiseInvalid (ty: System.Type) = raise (InvalidInhabitant ty)

module Functions =

    let inline curry f s1 s2 = f (s1, s2)

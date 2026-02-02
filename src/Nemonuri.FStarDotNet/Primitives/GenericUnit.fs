module Nemonuri.FStarDotNet.GenericUnit

type IUnitLike = interface end

[<RequireQualifiedAccess>]
type 't1 unit = 
    | Unit of unit
    interface IUnitLike

[<RequireQualifiedAccess>]
type ('t1, 't2) unit = 
    | Unit of unit
    interface IUnitLike

[<RequireQualifiedAccess>]
type ('t1, 't2, 't3) unit = 
    | Unit of unit
    interface IUnitLike

[<RequireQualifiedAccess>]
type ('t1, 't2, 't3, 't4) unit = 
    | Unit of unit
    interface IUnitLike
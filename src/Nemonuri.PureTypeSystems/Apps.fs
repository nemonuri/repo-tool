namespace Nemonuri.PureTypeSystems.Operations

open Nemonuri.PureTypeSystems

module Apps =

    let (|App|) x = Nemonuri.PureTypeSystems.Operations.Forwarded.Apps.(|App|) x

    let pur x = App<_>(x)

    let extract (x: App<_>) = x.Witness

    let inline toDotNet x = 
        let inline call (p': ^p) (x': ^x) = ((^p or ^x) : (static member AppToDotNet: _ -> _) x') in
        call Unchecked.defaultof<AppPremise> x

    type Builder =
        struct
            member inline _.Return(t: ^t) = toDotNet t
            member inline _.Bind(s: 's1, tf: App<'s1> -> 's2) = pur s |> tf
        end
    
    let builder = Builder()

module Tests =

    type Vop =
        struct
            static member ToDotNet<'a>(_: Vop, a: 'a) = ValueSome a
        end
    
    let test1() = (Vop(), Vop(), 123) |> (Apps.pur >> Apps.toDotNet)

    let test2() = Apps.builder { let! x' = (Vop(), Vop(), 123) in return x' }
    
        
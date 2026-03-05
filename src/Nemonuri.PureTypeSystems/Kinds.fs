namespace Nemonuri.PureTypeSystems

module Kinds =

    let pur x = Kind<_>(x)

    let extract (x: Kind<_>) = x.Witness

    let inline toDotNet x = 
        let inline call (p': ^p) (x': ^x) = ((^p or ^x) : (static member KindToDotNet: _ -> _) x') in
        call Unchecked.defaultof<KindPremise> x

    type Builder =
        struct
            member inline _.Return(t: ^t) = toDotNet t
            member inline _.Bind(s: 's1, tf: Kind<'s1> -> 's2) = pur s |> tf
        end
    
    let builder = Builder()

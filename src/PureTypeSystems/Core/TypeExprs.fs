namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives

module TypeExprs =

    let pur x = TypeExpr<_>(x)

    let extract (x: TypeExpr<_>) = x.Witness

    let inline toDotNet x = 
        let inline call (p': ^p) (x': ^x) = ((^p or ^x) : (static member ToDotNet: _ -> _) x') in
        call Unchecked.defaultof<TypeExprPremise> x

    type Monad =
        struct
            member inline _.Return(t: ^t) = toDotNet t
            member inline _.Bind(s: 's1, tf: TypeExpr<'s1> -> 's2) = pur s |> tf
        end
    
    let monad = Monad()

namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Primitives.TypeExpressions
open Nemonuri.PureTypeSystems.Primitives.TypeConstructors

module Kinds =

    open Unchecked
    type private Eth = Nemonuri.PureTypeSystems.Primitives.TypeExpressions.ExpressionTheory
    type private Ath = Nemonuri.PureTypeSystems.Primitives.ArrowTheory
    type B<'a> = Bracket<'a>

    let inline cons kind p =
        let inline call (k': ^k) (p': ^p) = ((^k or ^p) : (static member Cons : _ -> _) p') in
        call kind p
    
    let inline decons kind q =
        let inline call (k': ^k) (q': ^q) = ((^k or ^q) : (static member Decons : _ -> _) q') in
        call kind q

    let inline deconsToApp (kind: ^k) q = decons kind q |> Eth.UnsafeToApp<^k,_> 
        

    type Data<'a> = TypeExpressions.Data<'a>

    let dotNetToData (dn: 'dn) : Data<'dn> = Eth.ToData dn

    let dotNetOfData (data: Data<'dn>) = data.Value

    type App<'k, 'e> = TypeExpressions.App<'k, 'e>

    type ToAppPremise = struct

        static member ToApp(_: 'TKind, data: Data<'TData>) : App<'TKind, Data<'TData>> = Eth.UnsafeToApp data

        static member ToApp(_: 'TKind, app: App<'THead, 'TTail>) : App<'TKind, App<'THead, 'TTail>> = Eth.UnsafeToApp app

    end

    let inline toApp kind dataOrApp =
        let inline call (p: ^p) (k: ^k) (x: ^x) = ((^p or ^k) : (static member ToApp : _*_ -> _) k,x)
        call defaultof<ToAppPremise> kind dataOrApp


    type Premise = struct

        static member ToDotNet(data: Data<'TData>) = dotNetOfData data

        static member inline ToDotNet(app: App<'THead, Data<_>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, Data<_>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, Data<_>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, Data<_>>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, Data<_>>>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>>) = Premise.ToDotNet(app.Expression) |> cons defaultof<'THead>


        static member OfDotNet(k: Var, d: 'TData) = dotNetToData d

        static member inline OfDotNet(k: App<'THead, Var>, d) = Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, Var>>, d) = Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, Var>>>, d) = Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, App<_, Var>>>>, d) = Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, App<_, App<_, Var>>>>>, d) = Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, App<_, App<_, App<_, Var>>>>>>, d) = 
            Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, Var>>>>>>>, d) = 
            Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

        static member inline OfDotNet(k: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, App<_, Var>>>>>>>>, d) = 
            Premise.OfDotNet(k.Expression, decons defaultof<'THead> d) |> toApp defaultof<'THead>

    end


    let inline toDotNet appOrData =
        let inline call (p: ^p) (x: ^x) = ((^p or ^x) : (static member ToDotNet : _ -> _) x)
        call defaultof<Premise> appOrData

    let inline ofDotNet kinds appOrData =
        let inline call (p: ^p) (k: ^k) (x: ^x) = ((^p or ^x) : (static member OfDotNet : _*_ -> _) k, x)
        call defaultof<Premise> kinds appOrData

namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives

module Kinds =

    open Unchecked

    let inline cons kind p =
        let inline call (k': ^k) (p': ^p) = ((^k or ^p) : (static member Cons : _ -> _) p') in
        call kind p
    
    let inline decons kind q =
        let inline call (k': ^k) (q': ^q) = ((^k or ^q) : (static member Decons : _ -> _) q') in
        call kind q

    //type Data = IdentityKind
    type Data<'a> = TypeExpressions.Data<'a>

    let dotNetToData (dn: 'dn) = Data<'dn>(dn)

    let dotNetOfData (d: Data<'dn>) = d.Value

    type Data = struct

        static member Cons (x: 'p) = KindTheory.Cons<Data,'p,Data<'p>>(&x)
        static member Decons x = KindTheory.Decons<Data,'p,Data<'p>>(&x)

        interface IKindPremise<Data> with

            member _.TryToPair (handlePair: byref<ArrowHandlePair<'p,'q>>): bool = 
                ArrowPairTheory.TryToTypeEqualHandlePair<'p,Data<'p>,DataImpl<'p>,_,_>(&handlePair);
    end
    and private DataImpl<'p> = struct

        interface IArrowPairPremise<'p,Data<'p>> with
            member _.Apply (pre: inref<'p>) = dotNetToData pre
            member _.ContraApply (post: inref<Data<'p>>) = dotNetOfData post
    end


    type App<'k, 'kd> = TypeExpressions.App<'k, 'kd>

    type Guard<'t, 'j when 'j : unmanaged and 'j :> IJudgePremise and 'j : struct and 'j : (new: unit -> 'j) and 'j :> System.ValueType> = 
        TypeExpressions.App<StrictGuardKind<'j>, 't>

    type LooseGuard<'t, 'j when 'j : unmanaged and 'j :> IJudgePremise and 'j : struct and 'j : (new: unit -> 'j) and 'j :> System.ValueType> = 
        TypeExpressions.App<LooseGuardKind<'j>, 't>

    type Premise = struct

        static member ToApp(_: 'TKind, data: Data<'TData>) = App<'TKind, Data<'TData>>(data)

        static member ToApp(_: 'TKind, app: App<'THead, 'TTail>) = App<'TKind, App<'THead, 'TTail>>(app)

        static member ToDotNet(data: Data<'TData>) = dotNetOfData data

        static member inline ToDotNet(app: App<'THead, Data<_>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, Data<_>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, Data<_>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, Data<_>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, Data<_>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

    end

    let inline toApp kind dataOrApp =
        let inline call (p: ^p) (k: ^k) (x: ^x) = ((^p or ^k) : (static member ToApp : _*_ -> _) k,x)
        call defaultof<Premise> kind dataOrApp
    
    let inline toDotNet appOrData =
        let inline call (p: ^p) (x: ^x) = ((^p or ^x) : (static member ToDotNet : _ -> _) x)
        call defaultof<Premise> appOrData

#if false
    let inline guardToDotNet (guarded: Guard<^t,^j>) =
        guarded.Value |> toDotNet |> Refiners.refine<^dn,^j>
#endif


module TypeShadowing =

    type invalid = Invalid

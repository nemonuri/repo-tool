namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Primitives.TypeExpressions
open Nemonuri.PureTypeSystems.Primitives.TypeConstructors

module Kinds =

    open Unchecked
    type private Eth = Nemonuri.PureTypeSystems.Primitives.TypeExpressions.ExpressionTheory
    type private Ath = Nemonuri.PureTypeSystems.Primitives.ArrowTheory

    let inline cons kind p =
        let inline call (k': ^k) (p': ^p) = ((^k or ^p) : (static member Cons : _ -> _) p') in
        call kind p
    
    let inline decons kind q =
        let inline call (k': ^k) (q': ^q) = ((^k or ^q) : (static member Decons : _ -> _) q') in
        call kind q


    type Data<'a> = Refined<TypeExpressions.Data<'a>>

    let dotNetToData (dn: 'dn) : Data<'dn> = Eth.ToData dn |> Eth.ToRefinedData

    type DataJudge<'dn>(data: Data<'dn>) = class

        interface IIntroducer<Judgement> with
            member _.Introduce<'p> (_: inref<Judgement>): ArrowHandle<'p,Judgement> = 
                    let ok, result = Ath.TryToTypeEqualHandle<Data<'dn>, Judgement, DataJudgeImpl<'dn>, 'p, Judgement>() in
                    if ok then result else Ath.GetFailureHandle<_,_>()
    end
    and private DataJudgeImpl<'dn> = struct

        interface IArrowPremise<Data<'dn>, Judgement> with
            member _.Apply (pre: inref<Data<'dn>>): Judgement = 
                let v = pre.Value in
                pre.JudgeHandle.Judge(&v)
    end

    let judgeableOfData (d: Data<'dn>) : Judgeable<'dn> = Judgeable<_>(d.Value.Value, DataJudge<'dn>.Instance)

    type App<'k, 'expr> = Refined<TypeExpressions.App<'k, 'expr>>

    type AppJudge<'k, 'e, 'c when 'k :> IKindPremise<'k>>() = class

        static member Instance = AppJudge<'k, 'e, 'c>()

        interface IIntroducer<Judgement> with
            member _.Introduce<'p> (_: inref<Judgement>): ArrowHandle<'p,Judgement> = 
                    let ok, result = Ath.TryToTypeEqualHandle<App<'k,'e>, Judgement, AppJudgeImpl<'k, 'e, 'c>, 'p, Judgement>() in
                    if ok then result else Ath.GetFailureHandle<_,_>()

    end
    and private AppJudgeImpl<'k, 'e, 'c when 'k :> IKindPremise<'k>> = struct

        interface IArrowPremise<App<'k,'e>, Judgement> with
            member _.Apply (pre: inref<_>): Judgement = 
                let v = pre.Value in
                pre.JudgeHandle.Judge(&v)
    end

    let judgeableOfApp (a: App)


    type Premise = struct

        static member ToApp(_: 'TKind, data: Data<'TData>) : App<'TKind, Data<'TData>> = Eth.ToApp data |> Eth.ToRefinedApp

        static member ToApp(_: 'TKind, app: App<'THead, 'TTail>) : App<'TKind, App<'THead, 'TTail>> = Eth.ToApp app |> Eth.ToRefinedApp

        static member ToDotNet(data: Data<'TData>) = judgeableOfData data

        static member inline ToDotNet(app: App<'THead, Data<_>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

#if false
        static member inline ToDotNet(app: App<'THead, App<_, Data<_>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, Data<_>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, Data<_>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, Data<_>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>

        static member inline ToDotNet(app: App<'THead, App<_, App<_, App<_, App<_, App<_, App<_, App<_, Data<_>>>>>>>>>) = Premise.ToDotNet(app.Value) |> cons defaultof<'THead>
#endif

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

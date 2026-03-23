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

    type DataJudge<'dn>(judgeHandle: JudgeHandle<TypeExpressions.Data<'dn>>) = class

        member internal _.JudgeHandle = judgeHandle;

        interface IIntroducer<Judgement> with
            member _.Introduce<'p> (_: inref<Judgement>): ArrowHandle<'p,Judgement> = 
                    let ok, (result: ArrowHandle<'p,Judgement>) = Ath.TryToTypeEqualHandle<_,_, DataJudgeImpl<'dn>, _,_>() in
                    if ok then result else Ath.GetFailureHandle<_,_>()
    end
    and private DataJudgeImpl<'dn> = struct

        interface IMethodPremise<DataJudge<'dn>, 'dn, Judgement> with
            member _.Apply (pre: inref<struct (DataJudge<'dn> * 'dn)>): Judgement = 
                    let struct (ctx, v) = pre in
                    let data0 = Eth.ToData v in
                    ctx.JudgeHandle.Judge(&data0)
    end

    let judgeableOfData (d: Data<'dn>) : Judgeable<'dn, DataJudge<'dn>> = Judgeable<_,_>(d.Value.Value, DataJudge<_>(d.JudgeHandle))

    type App<'k, 'expr> = Refined<TypeExpressions.App<'k, 'expr>>


    type AppJudge<'k, 'e, 'c, 'j 
                    when 'k :> IKindPremise<'k>
                    and 'j :> IIntroducer<Judgement>>
                    (judgeHandle: JudgeHandle<TypeExpressions.App<'k,'e>>, innerIntroducer: 'j) = class

        member internal _.JudgeHandle = judgeHandle

        interface IIntroducer<Judgement> with
            member _.Introduce<'p> (hint: inref<Judgement>): ArrowHandle<'p,Judgement> = 
                    let ok, (result: ArrowHandle<'p,Judgement>) = Ath.TryToTypeEqualHandle<_,_, AppJudgeImpl<'k, 'e, 'c, 'j>, _,_>() in
                    if ok then result else 
                    innerIntroducer.Introduce<'p>(&hint)

    end
    and private AppJudgeImpl<'k, 'e, 'c, 'j 
                                when 'k :> IKindPremise<'k>
                                and 'j :> IIntroducer<Judgement>> = struct

        interface IMethodPremise<AppJudge<'k, 'e, 'c, 'j>, 'c, Judgement> with
            member _.Apply (pre: inref<_>): Judgement = 
                let struct (ctx, v) = pre in
                let app0 = KindTheory.DeconsToApp<'k,_,'e>(&v) in
                ctx.JudgeHandle.Judge(&app0)
    end

#if false
    let unsafeJudgeableOfApp (app: App<'k, 'e>) (constructed: 'c) : Judgeable<'c, AppJudge<'k, 'e, 'c>> = 
        Judgeable<_,_>(constructed,  AppJudge<_,_,'c>(app.JudgeHandle))
    
    let inline judgeableOfApp (app: App<^k, ^e>) =
        let app0 = app.Value in
        let coned = cons defaultof<^k> app0.Expression in
        unsafeJudgeableOfApp app coned
#endif

    type Premise = struct

        static member ToApp(_: 'TKind, data: Data<'TData>) : App<'TKind, Data<'TData>> = Eth.ToApp data |> Eth.ToRefinedApp

        static member ToApp(_: 'TKind, app: App<'THead, 'TTail>) : App<'TKind, App<'THead, 'TTail>> = Eth.ToApp app |> Eth.ToRefinedApp

        static member ToDotNet(data: Data<'TData>) = judgeableOfData data

        static member inline ToDotNet(app: App<'THead, Data<_>>) = //Premise.ToDotNet(app.Value.Expression)
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal = cons defaultof<'THead> tail.Value in
            let headJd = AppJudge<'THead, _,_,_>(app.JudgeHandle, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)


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

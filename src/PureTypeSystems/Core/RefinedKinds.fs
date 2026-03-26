namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Primitives.Expressions
open Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel
open Nemonuri.PureTypeSystems.Primitives.TypeConstructors

module RefinedKinds =

    open Unchecked
    type private Eth = Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel.TypeLevelTheory
    type private Ath = Nemonuri.PureTypeSystems.Primitives.ArrowTheory

#if false
    let inline cons kind p =
        let inline call (k': ^k) (p': ^p) = ((^k or ^p) : (static member Cons : _ -> _) p') in
        call kind p
    
    let inline decons kind q =
        let inline call (k': ^k) (q': ^q) = ((^k or ^q) : (static member Decons : _ -> _) q') in
        call kind q
#endif


    type RefinedData<'a> = Refined<Data<'a>>

    let dotNetToData (dn: 'dn) : RefinedData<'dn> = Eth.ToData dn |> Eth.ToRefinedData

    type DataJudge<'dn>(judgeHandle: JudgeHandle<Data<'dn>>) = class

        member internal _.JudgeHandle = judgeHandle;

        interface IIntroducer<JudgeResult> with
            member _.Introduce<'p> (_: inref<JudgeResult>): ArrowHandle<'p,JudgeResult> = 
                    let ok, (result: ArrowHandle<'p,JudgeResult>) = Ath.TryToTypeEqualHandle<_,_, DataJudgeImpl<'dn>, _,_>() in
                    if ok then result else Ath.GetFailureHandle<_,_>()
    end
    and private DataJudgeImpl<'dn> = struct

        interface IMethodPremise<DataJudge<'dn>, 'dn, JudgeResult> with
            member _.Apply (pre: inref<struct (DataJudge<'dn> * 'dn)>): JudgeResult = 
                    let struct (ctx, v) = pre in
                    let data0 = Eth.ToData v in
                    ctx.JudgeHandle.Judge(&data0)
    end

    let judgeableOfData (d: RefinedData<'dn>) : Judgeable<'dn, DataJudge<'dn>> = Judgeable<_,_>(d.Value.Value, DataJudge<_>(d.JudgeHandle))

    type RefinedApp<'k, 'expr> = Refined<App<'k, 'expr>>


    type AppJudge<'k, 'e, 'c, 'j 
                    when 'k :> IKindPremise<'k>
                    and 'j :> IIntroducer<JudgeResult>>
        (
            judgeHandle: JudgeHandle<App<'k,'e>>, 
            deconstructor: 'c -> App<'k,'e>,
            innerIntroducer: 'j
        ) = class

        member internal _.JudgeHandle = judgeHandle

        member internal _.Deconstructor = deconstructor

        interface IIntroducer<JudgeResult> with
            member _.Introduce<'p> (hint: inref<JudgeResult>): ArrowHandle<'p,JudgeResult> = 
                    let ok, (result: ArrowHandle<'p,JudgeResult>) = Ath.TryToTypeEqualHandle<_,_, AppJudgeImpl<'k, 'e, 'c, 'j>, _,_>() in
                    if ok then result else 
                    innerIntroducer.Introduce<'p>(&hint)

    end
    and private AppJudgeImpl<'k, 'e, 'c, 'j 
                                when 'k :> IKindPremise<'k>
                                and 'j :> IIntroducer<JudgeResult>> = struct

        interface IMethodPremise<AppJudge<'k, 'e, 'c, 'j>, 'c, JudgeResult> with
            member _.Apply (pre: inref<_>): JudgeResult = 
                let struct (ctx, v) = pre in
                let app0 = ctx.Deconstructor v in
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


        static member ToApp(_: 'TKind, data: RefinedData<'TData>) : RefinedApp<'TKind, RefinedData<'TData>> = Eth.ToApp data |> Eth.ToRefinedApp

        static member ToApp(_: 'TKind, app: RefinedApp<'THead, 'TTail>) : RefinedApp<'TKind, RefinedApp<'THead, 'TTail>> = Eth.ToApp app |> Eth.ToRefinedApp

#if false
        static member ToDotNet(data: RefinedData<'TData>) = judgeableOfData data

        static member inline ToDotNet(app: RefinedApp<'THead, RefinedData<'TData>>) = 
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal: 'hd = Kinds.cons defaultof<'THead> tail.Value in
            let decons (hd: 'hd) = 
                let d = Kinds.decons defaultof<'THead> hd in
                let v = Eth.ToRefinedData(Eth.ToData(d), tail.Introducer.JudgeHandle) in
                Premise.ToApp(defaultof<'THead>, v).Value
            in
            let headJd = AppJudge<'THead,_,_,_>(app.JudgeHandle, decons, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)

        static member inline ToDotNet(app: RefinedApp<'kh, RefinedApp<'kt, RefinedData<_>>>) = 
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal: 'hd = Kinds.cons defaultof<'kh> tail.Value in
            let decons (hd: 'hd) = 
                let d = Kinds.decons defaultof<'kh> hd in
                let v = Eth.ToRefinedApp<'kt,_>(tail.Introducer.Deconstructor d, tail.Introducer.JudgeHandle) in
                Premise.ToApp(defaultof<'kh>, v).Value
            in
            let headJd = AppJudge<'kh,_,_,_>(app.JudgeHandle, decons, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)

        static member inline ToDotNet(app: RefinedApp<'kh, RefinedApp<'kt, RefinedApp<_, RefinedData<_>>>>) =
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal: 'hd = Kinds.cons defaultof<'kh> tail.Value in
            let decons (hd: 'hd) = 
                let d = Kinds.decons defaultof<'kh> hd in
                let v = Eth.ToRefinedApp<'kt,_>(tail.Introducer.Deconstructor d, tail.Introducer.JudgeHandle) in
                Premise.ToApp(defaultof<'kh>, v).Value
            in
            let headJd = AppJudge<'kh,_,_,_>(app.JudgeHandle, decons, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)

        static member inline ToDotNet(app: RefinedApp<'kh, RefinedApp<'kt, RefinedApp<_, RefinedApp<_, RefinedData<_>>>>>) = 
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal: 'hd = Kinds.cons defaultof<'kh> tail.Value in
            let decons (hd: 'hd) = 
                let d = Kinds.decons defaultof<'kh> hd in
                let v = Eth.ToRefinedApp<'kt,_>(tail.Introducer.Deconstructor d, tail.Introducer.JudgeHandle) in
                Premise.ToApp(defaultof<'kh>, v).Value
            in
            let headJd = AppJudge<'kh,_,_,_>(app.JudgeHandle, decons, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)

        static member inline ToDotNet(app: RefinedApp<'kh, RefinedApp<'kt, RefinedApp<_, RefinedApp<_, RefinedApp<_, RefinedData<_>>>>>>) = 
            let tail = Premise.ToDotNet(app.Value.Expression) in
            let headVal: 'hd = Kinds.cons defaultof<'kh> tail.Value in
            let decons (hd: 'hd) = 
                let d = Kinds.decons defaultof<'kh> hd in
                let v = Eth.ToRefinedApp<'kt,_>(tail.Introducer.Deconstructor d, tail.Introducer.JudgeHandle) in
                Premise.ToApp(defaultof<'kh>, v).Value
            in
            let headJd = AppJudge<'kh,_,_,_>(app.JudgeHandle, decons, tail.Introducer) in
            Judgeable<_,_>(headVal, headJd)


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

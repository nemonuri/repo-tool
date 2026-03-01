namespace Nemonuri.FStarDotNet.Primitives

open Nemonuri.FStarDotNet.Primitives.Abstractions

module FStarTypes =

    exception InvalidInhabitant of System.Type

    let raiseInvalid (ty: System.Type) = raise (InvalidInhabitant ty)

    type TypeParameterProxy<'T> = struct end


module FStarKinds =

    [<Struct>]
    type KindSource<'TTail, 'THead> = { Witness: 'THead }
        with
            interface ITypeTail<'TTail>
            interface IFStarWitnessed<'THead> with
                member this.Witness = this.Witness
        end

    [<Struct>]
    type EnsuredBijection<'TTail, 'THead, 'TTarget> = private { Bijection: Bijection<'TTarget, KindSource<'TTail, 'THead>> }
        with
            static member Create(): EnsuredBijection<'TTail, 'THead, 'TTarget> =
                let inline raise'() = FStarTypes.raiseInvalid typeof<EnsuredBijection<'TTail, 'THead, 'TTarget>>
                match Bijection<'TTarget, KindSource<'TTail, 'THead>>.Default with
                | Null -> raise'()
                | NonNull bij -> 
                match bij.Pure, bij.Extract with
                | NonNull _, NonNull _ -> { Bijection = bij }
                | _ -> raise'()
        end

    let isBijectionInitialized<'tail, 'head, 'target> = Bijection<'target, KindSource<'tail, 'head>>.Default <> null

    let initBijection<'tail, 'head, 'target> 
        (pureImpl: 'target -> KindSource<'tail, 'head>) 
        (extractImpl: KindSource<'tail, 'head> -> 'target) =
        if isBijectionInitialized<'tail, 'head, 'target> then
            invalidOp "Already initialized."
        else
            Bijection<'target, KindSource<'tail, 'head>>.Default <- Bijection<_,_>(pureImpl, extractImpl)

    let private initIdIfUninitialized<'tail, 'head> =
        if isBijectionInitialized<'tail, 'head, KindSource<'tail, 'head>> then () else
            initBijection<'tail, 'head, KindSource<'tail, 'head>> id id

    let getBijection<'tail, 'head, 'target> = 
        match TypeEquality.Teq.tryRefl<'target, KindSource<'tail, 'head>> with
        | Some _ -> initIdIfUninitialized<'tail, 'head>
        | None -> ()
        EnsuredBijection<'tail, 'head, 'target>.Create().Bijection

    let getIdBijection<'tail, 'head> = getBijection<'tail, 'head, KindSource<'tail, 'head>>

    [<Struct>]
    type KindSourceMonad<'TTail> =
        struct
            member inline this.Return(wit: 'THead) : KindSource<'TTail, 'THead> = { Witness = wit }

            member inline this.Bind(x: KindSource<'TTail, 'THead>, f: 'THead -> KindSource<'TTail, 'THead2>) = x.Witness |> f
        end

    [<Struct>]
    type TargetMonad<'TTail> =
        struct
            member inline this.Return(s: KindSource<'TTail, 'THead>) : 'TTarget = getBijection<'TTail, 'THead, 'TTarget>.Extract.Invoke(s)
            
            member inline this.Bind(x: 'TTarget, f: KindSource<'TTail, 'THead> -> 'TTarget2) = getBijection<'TTail, 'THead, 'TTarget>.Pure.Invoke(x) |> f
        end

    [<Struct>]
    type KindMonad<'TTail> =
        struct
            member inline this.Return(s: 's) : 't = TargetMonad<'TTail>() { return KindSourceMonad<'TTail>() { return s } }

            member inline this.Bind(t: 't, f: 's -> 't2) : 't2 = 
                TargetMonad<'TTail>() { let! ks = t in return KindSourceMonad<'TTail>() { let! s = ks in return f s } }
        end

    let inline kmonad<'t>() = KindMonad<'t>()


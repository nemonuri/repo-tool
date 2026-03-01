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
    type EnsuredBijection<'TTail, 'THead, 'TTarget> = private { Bijection: Bijection<KindSource<'TTail, 'THead>, 'TTarget> }
        with
            static member Create(): EnsuredBijection<'TTail, 'THead, 'TTarget> =
                let inline raise'() = FStarTypes.raiseInvalid typeof<EnsuredBijection<'TTail, 'THead, 'TTarget>>
                match Bijection<KindSource<'TTail, 'THead>, 'TTarget>.Default with
                | Null -> raise'()
                | NonNull bij -> 
                match bij.Pure, bij.Extract with
                | NonNull _, NonNull _ -> { Bijection = bij }
                | _ -> raise'()
        end

    let inline getBijection<'tail, 'head, 'target> = EnsuredBijection<'tail, 'head, 'target>.Create().Bijection

    let inline isBijectionInitialized<'tail, 'head, 'target> = Bijection<KindSource<'tail, 'head>, 'target>.Default <> null

    let inline initBijection<'tail, 'head, 'target> 
        (pureImpl: KindSource<'tail, 'head> -> 'target) 
        (extractImpl: 'target -> KindSource<'tail, 'head>) =
        if isBijectionInitialized<'tail, 'head, 'target> then
            invalidOp "Already initialized."
        else
            Bijection<KindSource<'tail, 'head>, 'target>.Default <- Bijection<_,_>(pureImpl, extractImpl)

    [<Struct>]
    type KindSourceMonad<'TTail> =
        struct
            member inline this.Return(wit: 'THead) : KindSource<'TTail, 'THead> = { Witness = wit }

            member inline this.Bind(x: KindSource<'TTail, 'THead>, f: 'THead -> KindSource<'TTail, 'THead2>) = x.Witness |> f
        end

    [<Struct>]
    type TargetMonad<'TTail> =
        struct
            member inline this.Return(s: KindSource<'TTail, 'THead>) : 'TTarget = getBijection<'TTail, 'THead, 'TTarget>.Pure.Invoke(s)
            
            member inline this.Bind(x: 'TTarget, f: KindSource<'TTail, 'THead> -> 'TTarget2) = getBijection<'TTail, 'THead, 'TTarget>.Extract.Invoke(x) |> f
        end

    [<Struct>]
    type KindMonad<'TTail> =
        struct
            member inline this.Return(s: 's) : 't = TargetMonad<'TTail>() { return KindSourceMonad<'TTail>() { return s } }

            member inline this.Bind(t: 't, f: 's -> 't2) : 't2 = 
                TargetMonad<'TTail>() { let! ks = t in return KindSourceMonad<'TTail>() { let! s = ks in return f s } }
        end

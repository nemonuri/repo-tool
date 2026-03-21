namespace Nemonuri.PureTypeSystems

open Nemonuri.PureTypeSystems.Primitives

#if false
type IKind =
    interface
        abstract member TryToDotNet<'T>: 'T -> obj option

        abstract member TryFromDotNet<'T>: obj -> 'T option
    end


type Kind<'T> =
    struct
        val Witness: 'T
        new(witness: 'T) = { Witness = witness }
    end    

/// https://en.wikipedia.org/wiki/Three-valued_logic
type Trilean = bool voption

type Condition<'a> = 'a -> Trilean

type ITypeRefiner =
    interface
        abstract member GetCondition<'T>: unit -> Condition<'T>
    end

type IConstant<'T> =
    interface
        abstract member Value: 'T
    end

type Tautology<'T> =
    struct
        static member Judge (pre: inref<'T>, post: byref<'T>): Judgement = post <- pre; Judgement.True
            
        interface IRefinerPremise<'T> with
            member x.Judge (pre: inref<'T>, post: byref<'T>): Judgement = Tautology<'T>.Judge(&pre, &post)
    end


[<NoEquality; NoComparison>]
type Tautology =
    struct
        member _.Check(_: 'T) : bool voption = ValueSome true

        interface ITypeRefiner with
            member this.GetCondition (): Condition<'T> = this.Check<'T>
    end

[<NoEquality; NoComparison>]
type AlwaysUnknown =
    struct
        member _.Check(_: 'T) : bool voption = ValueNone

        interface ITypeRefiner with
            member this.GetCondition (): Condition<'T> = this.Check<'T>
    end
#endif

module Kinds =

    open Unchecked

    let inline cons kind p =
        let inline call (k': ^k) (p': ^p) = ((^k or ^p) : (static member Cons : _ -> _) p') in
        call kind p
    
    let inline decons kind q =
        let inline call (k': ^k) (q': ^q) = ((^k or ^q) : (static member Decons : _ -> _) q') in
        call kind q

    type Data = IdentityKind
    type Data<'a> = TypeExpressions.Data<'a>

    let dotNetToData (dn: 'dn) = Data<'dn>(dn)

    let dotNetOfData (d: Data<'dn>) = d.Value

    type App<'k, 'kd> = TypeExpressions.App<'k, 'kd>

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
    // let inline ofDotNet (kind: ^k) (dn: ^dn) = ((^k or ^dn) : (static member FromDotNet: ^dn -> ^k * ^t) dn)

    let (|TypeExpr|) (x: TypeExpr<'t>) = x.Witness

    let tautology = Tautology()

    // let unitKind = Kind<unit>( () )

    let triUnknown = ValueNone
    let triTrue = ValueSome true
    let triFalse = ValueSome false

    let inline (|TriUnknown|TriTrue|TriFalse|) (trilean: Trilean) =
        match trilean with
        | ValueNone -> TriUnknown
        | ValueSome v -> if v then TriTrue else TriFalse
    
    let tryExtract (cond: Condition<'a>) (x: 'a) =
        match cond x with
        | TriTrue -> Some x
        | _ -> None

    
    /// https://en.wikipedia.org/wiki/Three-valued_logic#Kleene_and_Priest_logics
    let triAnd (l: Trilean) (r: Trilean) : Trilean = 
        match l, r with
        | TriTrue, TriTrue -> triTrue
        | TriFalse, _ | _, TriFalse -> triFalse
        | _, _ -> triUnknown

    let conditionAnd (cond1: Condition<'a>) (cond2: Condition<'a>) (x: 'a) =
        match cond1 x with 
        | TriFalse -> triFalse
        | TriTrue -> cond2 x
        | TriUnknown -> triAnd triUnknown (cond2 x)
    
    let inline tryToRefiner (x: 'a) =
        match box x with
        | :? ITypeRefiner as v -> Some v
        | _ -> None


    let toSelfCondition (x: 'a) =
        match tryToRefiner x with
        | Some v -> v.GetCondition<'a>()
        | _ -> 
            if (typedefof<FSharpFunc<_,_>>.IsAssignableFrom(typedefof<'a>)) then AlwaysUnknown().Check<'a> else tautology.Check<'a>

    let checkSelf (x: 'a) = toSelfCondition x x
#endif




module TypeShadowing =

    type invalid = Invalid

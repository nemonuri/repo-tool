namespace Nemonuri.PureTypeSystems


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


module Operations =

    let inline toDotNet (head: ^h) (tail: ^t) = ((^h or ^t) : (static member ToDotNet: _*_ -> _) head, tail)

    let inline ofDotNet (kind: ^k) (dn: ^dn) = ((^k or ^dn) : (static member FromDotNet: ^dn -> ^k * ^t) dn)

    let (|Kind|) (x: Kind<'t>) = x.Witness

    let tautology = Tautology()

    let unitKind = Kind<unit>( () )

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

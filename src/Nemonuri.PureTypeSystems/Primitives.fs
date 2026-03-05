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

type Condition<'a> = 'a -> bool voption

type ITypeRefiner<'a> =
    interface
        abstract member Condition: Condition<'a>
    end

type TypeRefiner<'a> = { Condition: Condition<'a> }
    with
        interface ITypeRefiner<'a> with
            member this.Condition = this.Condition
    end

module Primitives =

    let inline toDotNet (head: ^h) (tail: ^t) = ((^h or ^t) : (static member ToDotNet: _*_ -> _) head, tail)

    let inline ofDotNet (kind: ^k) (dn: ^dn) = ((^k or ^dn) : (static member FromDotNet: ^dn -> ^k * ^t) dn)

    let (|Kind|) (x: Kind<'t>) = x.Witness

    let inline (|Unknown|Valid|Invalid|) (checkResult: bool voption) =
        match checkResult with
        | ValueNone -> Unknown
        | ValueSome v -> if v then Valid else Invalid
    
    let check (cond: Condition<'a>) (x: 'a) = cond x

    let tryExtract (cond: Condition<'a>) (x: 'a) =
        match check cond x with
        | Valid -> Some x
        | _ -> None



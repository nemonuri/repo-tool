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


module Primitives =

    let inline toDotNet (head: ^h) (tail: ^t) = ((^h or ^t) : (static member ToDotNet: _*_ -> _) head, tail)

    let inline ofDotNet (kind: ^k) (dn: ^dn) = ((^k or ^dn) : (static member FromDotNet: ^dn -> ^k * ^t) dn)

    let (|Kind|) (x: Kind<'t>) = x.Witness


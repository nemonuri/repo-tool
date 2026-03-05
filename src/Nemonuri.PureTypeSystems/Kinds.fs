namespace Nemonuri.PureTypeSystems


type IKind<'THead> =
    interface
        abstract member TryToDotNet<'TTail>: 'THead * 'TTail -> obj option

        abstract member TryFromDotNet<'TTail>: obj -> ('THead * 'TTail) option
    end


type App<'T> =
    struct
        val Witness: 'T
        new(witness: 'T) = { Witness = witness }
    end    


namespace Nemonuri.PureTypeSystems.Operations

module Kinds =

    let inline toDotNet (kind: ^k) (head: ^h) (tail: ^t) = ((^k or ^h) : (static member ToDotNet: _*_ -> _) head, tail)

    let inline ofDotNet (kind: ^k) (dn: ^dn) = ((^k or ^h) : (static member FromDotNet: ^dn -> ^h * ^t) dn)


namespace Nemonuri.PureTypeSystems.Operations.Forwarded

open Nemonuri.PureTypeSystems

module Apps =

    let (|App|) (x: App<_>) = x.Witness


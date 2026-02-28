namespace Nemonuri.FStarDotNet.Primitives

#if false
[<Struct>]
type FStarBox = internal { Object: objnull; Pointer: nativeint voption }

module FStarBoxes =

    let none = { Object = null; Pointer = ValueNone }

    let ofObj (o: obj) = { Object = o; Pointer = ValueNone }

    let ofPtr (p: nativeint) = { Object = null; Pointer = ValueSome p }

    let ofBytes (bytes: byte[]) (index: unativeint) = { Object = bytes; Pointer = nativeint index |> ValueSome }

    let (|None|Object|Pointer|Bytes|Unknown|) (box: FStarBox) =
        match box.Object, box.Pointer with
        | Null, ValueNone -> None
        | NonNull o, ValueNone -> Object o
        | Null, ValueSome p -> Pointer p
        | (:? array<byte> as bytes, ValueSome p) -> Bytes (bytes, p)
        | unk -> Unknown unk
#endif
module Nemonuri.PureTypeSystems.UnitTests.KindsTests

open Xunit
open Nemonuri.PureTypeSystems
open Nemonuri.PureTypeSystems.Primitives
open type Nemonuri.PureTypeSystems.Primitives.ArrowTheory
module K = Kinds

[<RequireQualifiedAccess>]
module private OptionKinds = begin

    [<NoEquality; NoComparison>]
    type Cons<'T> = struct
        interface IArrowPremise<'T, 'T option> with member _.Apply (pre: inref<'T>): 'T option = Some pre
    end

    [<NoEquality; NoComparison>]
    type Decons<'T> = struct
        interface IArrowPremise<'T option, 'T> with member _.Apply (pre: inref<'T option>): 'T = defaultArg pre Unchecked.defaultof<'T>
    end

end

[<NoEquality; NoComparison>]
type OptionKind = struct

    static member Cons<'T>(p: 'T) : 'T option = KindTheory.Cons<OptionKind,_,_>(&p)

    static member Decons<'T>(p: 'T option) : 'T = KindTheory.Decons<OptionKind,_,_>(&p)


    interface IKindPremise<OptionKind> with

        member _.TryToCons (handle: byref<ArrowHandle<'TP,'TQ>>): bool = TryToTypeEqualHandle<'TP, 'TP option, OptionKinds.Cons<'TP>,_,_>(&handle)
                
        member _.TryToDecons (handle: byref<ArrowHandle<'TQ,'TP>>): bool = TryToTypeEqualHandle<'TP option, 'TP, OptionKinds.Decons<'TP>,_,_>(&handle)

end

[<Fact>]
let ``Kinds.cons OptionKind int``() =
    let actual = K.cons (OptionKind()) 123
    Assert.IsType<int option>(actual)

[<Fact>]
let ``Kinds.cons OptionKind string``() =
    let actual = K.cons (OptionKind()) "123"
    Assert.IsType<string option>(actual)

[<Fact>]
let ``Kinds.cons OptionKind (option int)``() =
    let actual = K.cons (OptionKind()) (Some 123)
    Assert.IsType<int option option>(actual)

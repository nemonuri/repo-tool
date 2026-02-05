#nowarn 64

namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit


type TheoryDataBuilder<'s1,'t1>(f1: 's1 -> 't1) =
    member x.Zero() = TheoryData<'t1>()
    member x.Combine(acc: TheoryData<'t1>, e: 's1) = acc.Add(f1 e)

type TheoryDataBuilder<'s1,'t1,'s2,'t2>(f1: 's1->'t1, f2: 's2->'t2) =
    member x.Zero() = TheoryData<'t1,'t2>()
    member x.Combine(acc: TheoryData<'t1,'t2>, (e1: 's1, e2: 's2)) = acc.Add(f1 e1, f2 e2)


module TheoryDataBuilder =

    let width1 f1 = TheoryDataBuilder<_,_>(f1)

    let width2 f1 f2 = TheoryDataBuilder<_,_,_,_>(f1, f2)

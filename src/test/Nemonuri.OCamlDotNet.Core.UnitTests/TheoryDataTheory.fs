#nowarn 64

namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit

module TheoryDataTheory =

    type private Te = System.TupleExtensions

    let create1 (f1: 'a1->'b1) (l: list<_>) = TheoryData<_>(List.map f1 l)

    let create2 (f1: 'a1->'b1) (f2: 'a2->'b2) (l: list<(_*_)>) = 
        TheoryData<_,_>(List.map ((fun (i1, i2) -> f1 i1, f2 i2) >> Te.ToValueTuple) l)

    let create3 (f1: 'a1->'b1) (f2: 'a2->'b2) (f3: 'a3->'b3) (l: list<(_*_*_)>) = 
        TheoryData<_,_,_>(List.map ((fun (i1, i2, i3) -> f1 i1, f2 i2, f3 i3) >> Te.ToValueTuple) l)

    let create4 (f1: 'a1->'b1) (f2: 'a2->'b2) (f3: 'a3->'b3) (f4: 'a4->'b4) (l: list<(_*_*_*_)>) = 
        TheoryData<_,_,_,_>(List.map ((fun (i1, i2, i3, i4) -> f1 i1, f2 i2, f3 i3, f4 i4) >> Te.ToValueTuple) l)

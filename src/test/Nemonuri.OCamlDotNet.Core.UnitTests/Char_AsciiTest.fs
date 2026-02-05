namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit
open Nemonuri.OCamlDotNet.Core
type Te = System.TupleExtensions

module Char_AsciiTestTheory =

    let mapTuple2 f1 f2 (i1, i2) = f1 i1, f2 i2

open Char_AsciiTestTheory

type Char_AsciiTest(out: ITestOutputHelper) =

    static member Members1 : TheoryData<char, char> = 
        let f = char.FromDotNetChar
        TheoryData<_,_>([
            ('a', 'A'); (' ', ' '); ('ÿ', 'ÿ'); ('1', '1'); ('g', 'G'); ('K', 'K')
            ] |> List.map (mapTuple2 f f >> Te.ToValueTuple)
        )

    [<Theory>]
    [<MemberData(nameof Char_AsciiTest.Members1)>]
    member _.uppercase (c: char) (expected: char) =
        let actual = Char.Ascii.uppercase c
        Assert.Equal(expected, actual)

    static member Members2 : TheoryData<char, char> = 
        let f = char.FromDotNetChar
        TheoryData<_,_>([
            ('a', 'A'); (' ', ' '); ('ÿ', 'ÿ'); ('1', '1'); ('g', 'G'); ('K', 'K')
            ] |> List.map (mapTuple2 f f >> Te.ToValueTuple)
        )


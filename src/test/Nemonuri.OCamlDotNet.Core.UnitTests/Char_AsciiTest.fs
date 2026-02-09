namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit
open Nemonuri.OCamlDotNet
module Tt = TheoryDataTheory

module private Char_AsciiTestTheory =

    let d2o (c: Core.char) = Core.Operators.byte c

open Char_AsciiTestTheory

type Char_AsciiTest(out: ITestOutputHelper) =

    static member Members1 = Tt.create2 d2o d2o [
        ('a', 'A'); (' ', ' '); ('ÿ', 'ÿ'); ('1', '1'); ('g', 'G'); ('K', 'K')
    ]
    [<Theory>]
    [<MemberData(nameof Char_AsciiTest.Members1)>]
    member _.uppercase (c: char) (expected: char) =
        let actual = Char.Ascii.uppercase c
        Assert.Equal(expected, actual)

    static member Members2 = Tt.create2 d2o d2o [
       ('A', 'a'); (' ', ' '); ('ÿ', 'ÿ'); ('1', '1'); ('G', 'g'); ('k', 'k')
    ]
    [<Theory>]
    [<MemberData(nameof Char_AsciiTest.Members2)>]
    member _.lowercase (c: char) (expected: char) =
        let actual = Char.Ascii.lowercase c
        Assert.Equal(expected, actual)


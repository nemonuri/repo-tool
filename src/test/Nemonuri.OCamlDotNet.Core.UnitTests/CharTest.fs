namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit
open Nemonuri.OCamlDotNet
open Nemonuri.ByteChars
module Tt = TheoryDataTheory

module private CharTestTheory =

    let toByteString (bytes: byte[]) : string = ByteStringTheory.FromByteSpan bytes

open CharTestTheory

type CharTest(out: ITestOutputHelper) =

    static member Members1 = Tt.create2 id toByteString [
        ('a'B, [|'a'B|]); ('/'B, [|'/'B|]); ('\\'B, @"\\"B); ('\n'B, @"\n"B);
        (' 'B, @"\ "B); ('\u007f'B, @"\127"B);
        ('\u001f'B, @"\031"B); (byte 'ÿ', @"\255"B)
    ]
    [<Theory>]
    [<MemberData(nameof CharTest.Members1)>]
    member _.escaped(charValue: char, expectedString: string) =
        let actualString = Char.escaped charValue
        Assert.Equal<char>(expectedString, actualString)

    static member Members2 = Tt.create3 id id id [
        ('a', false, 'a'B);
        ('\t', false, '\t'B);
        ('©', false, byte '©');
        ('☆', true, Unchecked.defaultof<_>);
        ('→', true, Unchecked.defaultof<_>);
    ]
    [<Theory>]
    [<MemberData(nameof CharTest.Members2)>]
    member _.CheckedDotNetCharToByteChar
        (dotNetChar: Microsoft.FSharp.Core.char)
        (expectingException: bool)
        (expectedChar: char) =
        let mutable actualChar = 0uy
        let testCode() = actualChar <- ByteCharTheory.CheckedDotNetCharToByteChar dotNetChar

        if expectingException then
            Assert.Throws<System.OverflowException> testCode |> ignore
        else
            testCode();
            Assert.Equal(expectedChar, actualChar)

    static member Members3 = Tt.create3 id id id [
        ('a'B, 'a', true);
        ('\t'B, '\t', true);
        (byte 'ÿ', 'ÿ', true);
        (0uy, '\u0000', true);
        ('b'B, 'c', false);
        ('\n'B, '\r', false);
        ('c'B, Operators.char ((Operators.int 'c') + 0x100), false)
    ]
    member _.ByteCharToDotNetChar
        (byteChar: char)
        (toCompareDotNetChar: Microsoft.FSharp.Core.char)
        (expectingEqual: bool) =
        let actualChar = ByteCharTheory.ByteCharToDotNetChar byteChar
        if expectingEqual then
            Assert.Equal(toCompareDotNetChar, actualChar)
        else
            Assert.Equal(toCompareDotNetChar, actualChar)
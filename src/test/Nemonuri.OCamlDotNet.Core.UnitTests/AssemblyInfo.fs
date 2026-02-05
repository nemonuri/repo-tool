namespace Nemonuri.OCamlDotNet.Core.UnitTests

open Xunit.Sdk
open Nemonuri.OCamlDotNet.Xunit

[<assembly: RegisterXunitSerializer(typeof<CharSerializer>, [|typeof<Nemonuri.OCamlDotNet.Char>|])>]
[<assembly: RegisterXunitSerializer(typeof<StringSerializer>, [|typeof<Nemonuri.OCamlDotNet.String>|])>]
do()

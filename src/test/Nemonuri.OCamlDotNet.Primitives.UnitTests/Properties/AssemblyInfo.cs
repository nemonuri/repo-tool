using Nemonuri.OCamlDotNet.Primitives.UnitTests;

[assembly: RegisterXunitSerializer(typeof(CharSerializer), [typeof(Nemonuri.OCamlDotNet.Char)])]
[assembly: RegisterXunitSerializer(typeof(StringSerializer), [typeof(Nemonuri.OCamlDotNet.String)])]
using Nemonuri.OCamlDotNet.Primitives.UnitTests;
using Nemonuri.OCamlDotNet.Xunit;

[assembly: RegisterXunitSerializer(typeof(CharSerializer), [typeof(Nemonuri.OCamlDotNet.Char)])]
[assembly: RegisterXunitSerializer(typeof(StringSerializer), [typeof(Nemonuri.OCamlDotNet.String)])]
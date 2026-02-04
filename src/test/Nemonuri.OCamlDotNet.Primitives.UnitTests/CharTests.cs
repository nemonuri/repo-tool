using Nemonuri.OCamlDotNet.Extensions;

namespace Nemonuri.OCamlDotNet.Primitives.UnitTests;

public class CharTests
{
    private readonly ITestOutputHelper _out;

    public CharTests(ITestOutputHelper @out)
    {
        _out = @out;
    }

    [Theory]
    [MemberData(nameof(Members1))]
    public void ToEscaped(byte charValue, byte[] expectedBytes)
    {
        // Arrange
        Char ocamlChar = new (charValue);

        // Act
        String actualString = ocamlChar.ToEscaped();
        ReadOnlySpan<byte> actualBytes = actualString.AsSpan().ToBytes();

        // Assert
        Assert.Equal(expectedBytes, actualBytes);
    }

    public static TheoryData<byte, byte[]> Members1 =>
    [
        ((byte)'a', [(byte)'a']), 
        ((byte)'/', [(byte)'/']),
        ((byte)'\\', [..@"\\"u8]),
        ((byte)'\n', [..@"\n"u8]),
        ((byte)' ', [..@"\ "u8]),
        ((byte)'\u007f', [..@"\127"u8]),
        ((byte)'\u001f', [..@"\031"u8]),
        ((byte)'ÿ', [..@"\255"u8])
    ];
}

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

    [Theory]
    [MemberData(nameof(Members2))]
    public void FromDotNetChar(char dotnetChar, bool expectingSuccess, Char expectedChar)
    {
        // Arrange
        Char actualChar = default;
        void TestCode()
        {
            actualChar = Char.FromDotNetChar(dotnetChar);
        }

        // Act & Assert
        if (expectingSuccess)
        {
            TestCode();
            Assert.Equal(expectedChar, actualChar);
        }
        else
        {
            Assert.Throws<OverflowException>(TestCode);
        }
    }
    public static TheoryData<char, bool, Char> Members2 =>
    [
        ('a', true, new((byte)'a')),
        ('\t', true, new((byte)'\t')),
        ('©', true, new((byte)'©')),
        ('☆', false, default),
        ('→', false, default)
    ];
}

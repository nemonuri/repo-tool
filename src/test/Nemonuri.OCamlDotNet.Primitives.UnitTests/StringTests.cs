namespace Nemonuri.OCamlDotNet.Primitives.UnitTests;

public class StringTests
{
    private readonly ITestOutputHelper _out;

    public StringTests(ITestOutputHelper @out)
    {
        _out = @out;
    }

    [Theory]
    [MemberData(nameof(Members1))]
    public void FromDotNetString(string dotnetString, bool expectingException, Char[] expectedChars)
    {
        // Arrange
        Char[] actualChars = [];
        void TestCode()
        {
            actualChars = String.FromDotNetString(dotnetString).AsSpan().ToArray();
        }

        // Act & Assert
        if (expectingException)
        {
            Assert.Throws<OverflowException>(TestCode);
        }
        else
        {
            TestCode();
            Assert.Equal(expectedChars, actualChars);
        }
    }
    public static TheoryData<string, bool, Char[]> Members1 =>
    [
        ("abcde", false, [.."abcde"u8]),
        ("Hello, World!\n:-)", false, [.."Hello, World!\n:-)"u8]),
        ("Latin-1 ©ÿ¼", false, [.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼']),
        ("☆ 안녕! ★", true, []),
        ("©️" /* emoji */, true, [])
    ];

    [Theory]
    [MemberData(nameof(Members2))]
    public void ToDotNetString(Char[] ocamlStringAsChars, string expectedDotNetString)
    {
        // Arrange
        String ocamlString = new(ocamlStringAsChars);

        // Act
        string actualDotNetString = ocamlString.ToDotNetString();

        // Assert
        Assert.Equal(expectedDotNetString, actualDotNetString);
    }
    public static TheoryData<Char[], string> Members2 =>
    [
        ([.."abcde"u8], "abcde"),
        ([.."Hello, World!\n:-)"u8], "Hello, World!\n:-)"),
        ([.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼'], "Latin-1 ©ÿ¼")
    ];
}
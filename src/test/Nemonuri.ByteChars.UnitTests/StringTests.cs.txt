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

    [Theory]
    [MemberData(nameof(Members3))]
    public void Test_Equals(String left, String right, bool expectedResult)
    {
        // Arrange
        _out.WriteLine($"left = {left.ToDotNetString()}");
        _out.WriteLine($"right = {right.ToDotNetString()}");

        // Act
        bool actualResult = left.Equals(right);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }
    public static TheoryData<String, String, bool> Members3 =>
    [
        (new([.."Hello, World!\n:-)"u8]), new([.."Hello, World!\n:-)"u8]), true),
        (new([.."Hello, World!\n:-)"u8]), new([.."Hello, World!\n:-("u8]), false),
        (new([.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼']), new([.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼']), true),
        (new([.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼']), new([.."Latin-1 ©ÿ¼"u8]), false),
        (default, [], true)
    ];

    [Theory]
    [MemberData(nameof(Members4))]
    public void GetHashCode_CompareToClone_ShouldEqual(String original)
    {
        // Arrange
        String clone = new(original.AsEnumerable());
        _out.WriteLine($"original = {original.ToDotNetString()}");
        _out.WriteLine($"clone = {clone.ToDotNetString()}");

        // Act
        int originalHashCode = original.GetHashCode();
        int cloneHashCode = clone.GetHashCode();
        _out.WriteLine($"originalHashCode = {originalHashCode}");
        _out.WriteLine($"cloneHashCode = {cloneHashCode}");

        // Assert
        Assert.Equal(originalHashCode, cloneHashCode);
    }
    public static TheoryData<String> Members4 =>
    [
        new([.."Hello, World!\n:-)"u8]),
        new([.."Latin-1 "u8, (Char)'©', (Char)'ÿ', (Char)'¼']),
        new([]),
        new([.."absadfefph2134nfvfd8"u8])
    ];
}
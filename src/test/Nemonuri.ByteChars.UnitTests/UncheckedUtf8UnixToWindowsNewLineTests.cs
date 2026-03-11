using Nemonuri.Transcodings;

namespace Nemonuri.ByteChars.UnitTests;

public class UncheckedUtf8UnixToWindowsNewLineTests
{
    private readonly ITestOutputHelper _out;

    public UncheckedUtf8UnixToWindowsNewLineTests(ITestOutputHelper @out)
    {
        _out = @out;
    }

    [Theory]
    [MemberData(nameof(Members1))]
    public void Test1(byte[] source, byte[] expected)
    {
        // Arrange

        // Act
        var actual = TranscodingTheory.TranscodeToArraySegmentWhileDestinationTooSmall<byte,byte,UncheckedUtf8UnixToWindowsNewLine>(source.AsSpan(), out _);

        // Assert
        Assert.Equal(expected.AsSpan(), actual.AsSpan());
    }

    public static TheoryData<byte[],byte[]> Members1 => new()
    {
        { [.."\n"u8], [.."\r\n"u8] },
        { [.."Hello, world!"u8], [.."Hello, world!"u8] },
        { [.."Hello, \nworld!"u8], [.."Hello, \r\nworld!"u8] }
    };

}
using Nemonuri.Transcodings;
using System.Buffers;

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
    public void TransCode_OperationStatusShouldBeDone(byte[] source, int destSize, byte[] expectedBytes)
    {
        // Arrange
        UncheckedUtf8UnixToWindowsNewLine th = new();
        byte[] dest = new byte[destSize];

        // Act
        var actualStatus = th.Transcode(source, dest, out _, out int actualWritten);

        // Assert
        Assert.Equal(expectedBytes.AsSpan(), dest.AsSpan()[..actualWritten]);
        Assert.Equal(OperationStatus.Done, actualStatus);
    }

    public static TheoryData<byte[],int,byte[]> Members1 => new()
    {
        { [.."\n"u8], 256, [.."\r\n"u8] },
        { [.."Hello, world!"u8], 256, [.."Hello, world!"u8] },
        { [.."Hello, \nworld!"u8], 256, [.."Hello, \r\nworld!"u8] },
        { [.."Hello, \r\nworld!"u8], 256, [.."Hello, \r\nworld!"u8] },
        { [.."이것은, \n\n 시험용 문자열입니다★"u8], 1024, [.."이것은, \r\n\r\n 시험용 문자열입니다★"u8]  }
    };


    [Theory]
    [MemberData(nameof(Members2))]
    public void Test1(byte[] source, byte[] expected)
    {
        // Arrange

        // Act
        var actual = TranscodingTheory.TranscodeToArraySegmentWhileDestinationTooSmall<byte,byte,UncheckedUtf8UnixToWindowsNewLine>(source.AsSpan(), out _);

        // Assert
        Assert.Equal(expected.AsSpan(), actual.AsSpan());
    }

    public static TheoryData<byte[],byte[]> Members2 => new( Members1.Select(static a => (a.Data.Item1, a.Data.Item3)) );


}
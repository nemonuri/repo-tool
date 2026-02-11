using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Unicode;
using static Nemonuri.ByteChars.ByteCharTheory;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars;

static unsafe partial class ByteStringTheory
{
    public static bool TryAsciiByteSpanToUtf16DotNetString
    (
        ReadOnlySpan<byte> byteSpan,
        [NotNullWhen(true)] out string? dotnetString
    )
    {
        if (byteSpan.Length == 0)
        {
            dotnetString = string.Empty;
            return true;
        }

        fixed(byte *ptr = byteSpan)
        {
            UnsafePinnedSpanPointer<byte> sp = new(ptr, byteSpan.Length);
            if (!PinnedByteSpanPointerPremise.IsValidAll(sp))
            {
                dotnetString = null;
                return false;
            }
        }

        Span<char> dest = new char[Sls.GetFixedSize()];
        var rs = Sls.SplitSpan(byteSpan);

        var sb = Internal.StringBuilderPoolTheroy.Shared.Get();

        foreach (var chunk in rs)
        {
            OperationStatus status = Utf8.ToUtf16(chunk, dest, out int bytesRead, out int charsWritten);
            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(bytesRead == charsWritten);
            sb.Append(dest[..charsWritten]);
        }
        
        string result = sb.ToString();
        Debug.Assert( byteSpan.Length == result.Length );

        Internal.StringBuilderPoolTheroy.Shared.Return(sb);
        
        dotnetString = result;
        return true;
    }
}
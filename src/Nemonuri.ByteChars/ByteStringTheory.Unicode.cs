using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Unicode;
using static Nemonuri.ByteChars.ByteCharTheory;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars;

static unsafe partial class ByteStringTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendCharSpan(StringBuilder sb, ReadOnlySpan<char> chars)
    {
#if NETSTANDARD2_1_OR_GREATER
        return sb.Append(chars);
#else
        return Nemonuri.NetStandards.Text.StringBuilderTheory.AppendSpan(sb, chars);
#endif
    }

    public static bool TryAsciiByteSpanToUtf16DotNetString
    (
        ReadOnlySpan<byte> byteSpan,
        [NotNullWhen(true)] out string? dotNetString
    )
    {
        if (byteSpan.Length == 0)
        {
            dotNetString = string.Empty;
            return true;
        }

        fixed(byte *ptr = byteSpan)
        {
            UnsafePinnedSpanPointer<byte> sp = new(ptr, byteSpan.Length);
            if (!PinnedByteSpanPointerPremise.IsValidAll(sp))
            {
                dotNetString = null;
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
            AppendCharSpan(sb, dest[..charsWritten]);
        }
        
        string result = sb.ToString();
        Debug.Assert( byteSpan.Length == result.Length );

        Internal.StringBuilderPoolTheroy.Shared.Return(sb);
        
        dotNetString = result;
        return true;
    }

    public static bool TryAsciiByteStringToUtf16DotNetString
    (
        ImmutableArray<byte> byteString,
        [NotNullWhen(true)] out string? dotNetString
    )
    {
        return TryAsciiByteSpanToUtf16DotNetString(byteString.AsSpan(), out dotNetString);
    }

    public static ImmutableArray<byte> DotNetStringToUtf8ByteString
    (
        string dotNetString
    )
    {
        return UnicodeEncoding.UTF8.GetBytes(dotNetString).ToImmutableArray();
    }
}
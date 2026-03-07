using Nemonuri.FixedSizes;
using Nemonuri.ByteChars.Internal;
using Nemonuri.ByteChars.ByteSpans;
using System.Diagnostics;
using System.Text.Unicode;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars;

public static class ByteCharSpanTheory
{
    public static ByteCharSpanSplitEnumerator SplitByteCharSpan(ReadOnlySpan<byte> source, byte seperator)
    {
        return new 
        (
#if NET9_0_OR_GREATER
            source.Split(seperator)
#else
            Nemonuri.NetStandards.MemorySplitTheory.Split(source, seperator)
#endif
        );
    }

    public static ByteCharSpanSplitEnumerator SplitByteCharSpan(ReadOnlySpan<byte> source, ReadOnlySpan<byte> seperator)
    {
        return new 
        (
#if NET9_0_OR_GREATER
            source.Split(seperator)
#else
            Nemonuri.NetStandards.MemorySplitTheory.Split(source, seperator)
#endif
        );
    }

    public static string ToDotNetString(ReadOnlySpan<byte> source)
    {
        var sb = StringBuilderPoolTheory.Shared.Get();
        foreach (byte byteChar in source)
        {
            sb.Append(ByteCharTheory.ByteCharToDotNetChar(byteChar));
        }
        string result = sb.ToString();
        StringBuilderPoolTheory.Shared.Return(sb);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendCharSpan(StringBuilder sb, ReadOnlySpan<char> chars)
    {
#if NETSTANDARD2_1_OR_GREATER
        return sb.Append(chars);
#else
        return Nemonuri.NetStandards.Text.StringBuilderTheory.AppendSpan(sb, chars);
#endif
    }

    public unsafe static bool TryAsciiByteSpanToUtf16DotNetString
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

        var sb = Internal.StringBuilderPoolTheory.Shared.Get();

        foreach (var chunk in rs)
        {
            OperationStatus status = Utf8.ToUtf16(chunk, dest, out int bytesRead, out int charsWritten);
            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(bytesRead == charsWritten);
            AppendCharSpan(sb, dest[..charsWritten]);
        }
        
        string result = sb.ToString();
        Debug.Assert( byteSpan.Length == result.Length );

        Internal.StringBuilderPoolTheory.Shared.Return(sb);
        
        dotNetString = result;
        return true;
    }


    public static ByteCharSpanRuneEnumerator EnumerateRunes(ReadOnlySpan<byte> byteCharSpan) => new(byteCharSpan);

    public static int GetRuneCount(ReadOnlySpan<byte> byteCharSpan)
    {
        int count = 0;
        var e = EnumerateRunes(byteCharSpan);
        while (e.MoveNext())
        {
            count++;
        }
        return count;
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static bool TryGetRuneAt(ReadOnlySpan<byte> byteCharSpan, int index, out Rune rune)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0);
        int stepIndex = 0;
        foreach (Rune stepRune in EnumerateRunes(byteCharSpan))
        {
            if (stepIndex == index)
            {
                rune = stepRune;
                return true;
            }
            stepIndex++;
        }
        rune = default;
        return false;
    }
}
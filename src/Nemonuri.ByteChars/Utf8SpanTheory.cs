using Nemonuri.FixedSizes;
using Nemonuri.ByteChars.Internal;
using System.Diagnostics;
using System.Text.Unicode;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars;

public static class Utf8SpanTheory
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

    public static bool IsValid(ReadOnlySpan<byte> source) => Utf8.IsValid(source);

    public static bool TryToDotNetString
    (
        ReadOnlySpan<byte> source,
        [NotNullWhen(true)] out string? dotNetString
    )
    {
        if (source.Length == 0)
        {
            dotNetString = string.Empty;
            return true;
        }

        if (!IsValid(source))
        {
            dotNetString = null;
            return false;
        }

        Span<char> dest = new char[Sls.GetFixedSize()];
        var rs = Sls.SplitSpan(source);

        var sb = StringBuilderPoolTheory.Shared.Get();

        ReadOnlySpan<byte> remainedSource = source;
        while (true)
        {
            OperationStatus status = Utf8.ToUtf16(remainedSource, dest, out int bytesRead, out int charsWritten);  
            if (!(status is OperationStatus.Done or OperationStatus.DestinationTooSmall))
            {
                // Assert: source data is invlid (how?)
                StringBuilderPoolTheory.Shared.Return(sb);
                dotNetString = null;
                return false;
            }

            AppendCharSpan(sb, dest[..charsWritten]);
            if (status is OperationStatus.Done) { break; }

            Debug.Assert( status is OperationStatus.DestinationTooSmall );
            remainedSource = remainedSource[bytesRead..];
        }

        // Done.
        dotNetString = sb.ToString();
        StringBuilderPoolTheory.Shared.Return(sb);
        return true;
    }

    public static ByteCharSpanRuneEnumerator EnumerateRunes(ReadOnlySpan<byte> source) => new(source);

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
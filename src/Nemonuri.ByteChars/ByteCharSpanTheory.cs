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
}
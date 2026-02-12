using System.Collections;
#if NET9_0_OR_GREATER
using SpanSplitEnumerator = System.MemoryExtensions.SpanSplitEnumerator<byte>;
#else
using SpanSplitEnumerator = Nemonuri.NetStandards.MemorySplitTheory.SpanSplitEnumerator<byte>;
#endif

namespace Nemonuri.ByteChars;

public readonly ref struct ByteStringSplitEnumerator
#if NET9_0_OR_GREATER
 : IEnumerator<RangedReadOnlySpan<byte>>
#endif
{
    private readonly SpanSplitEnumerator _spanSplitEnumerator;

    internal ByteStringSplitEnumerator(SpanSplitEnumerator spanSplitEnumerator)
    {
        _spanSplitEnumerator = spanSplitEnumerator;
    }

    public readonly RangedReadOnlySpan<byte> Current => new (_spanSplitEnumerator.Source, _spanSplitEnumerator.Current);

    public readonly bool MoveNext()
    {
        return _spanSplitEnumerator.MoveNext();
    }

#if NET9_0_OR_GREATER
    void IEnumerator.Reset() => throw new NotSupportedException();

    object IEnumerator.Current => Current.GetSliced().ToArray();

    readonly void IDisposable.Dispose() { }
#endif
}
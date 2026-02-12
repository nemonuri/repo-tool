namespace Nemonuri.ByteChars;

public readonly ref struct RangedReadOnlySpan<T>
{
    public ReadOnlySpan<T> Span {get;}

    public Range Range {get;}

    public RangedReadOnlySpan(ReadOnlySpan<T> span, Range range)
    {
        Span = span;
        Range = range;
    }

    /// <exception cref="System.ArgumentOutOfRangeException">
    public readonly ReadOnlySpan<T> GetSliced() => Span[Range];
}

namespace Nemonuri.ByteChars;

public static partial class FixedSizeTheory
{
    public readonly ref struct SplitSpanResult<TSize, T>
        where TSize : unmanaged, IFixedSizePremise<TSize>
    {
        internal SplitSpanResult(FixedSizeChunkSpan<TSize, T> chunks, Span<T> remainder)
        {
            Chunks = chunks;
            Remainder = remainder;
        }

        public FixedSizeChunkSpan<TSize, T> Chunks {get;}

        public Span<T> Remainder {get;}

        public readonly Enumerator GetEnumerator() => new (this);

        public readonly ref struct Enumerator
        {
            private readonly SplitSpanResult<TSize, T> _source;
            private readonly FixedSizeChunkSpan<TSize, T>.Enumerator _chunkEnumerator;
            

            internal Enumerator(SplitSpanResult<TSize, T> source)
            {
                _source = source;
                _chunkEnumerator = _source.Chunks.GetEnumerator();
            }

            public readonly bool MoveNext()
            {
                if (_chunkEnumerator.MoveNext())
                {
                    return true;
                }
                else if (_chunkEnumerator.IsOnBorder && _source.Remainder.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public readonly Span<T> Current
            {
                get
                {
                    if (!_chunkEnumerator.IsMoved) { return default; }
                    if (_chunkEnumerator.IsOnBorder) { return _source.Remainder; }
                    return _chunkEnumerator.Current;
                }
            }
        }
    }
}

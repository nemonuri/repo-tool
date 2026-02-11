namespace Nemonuri.ByteChars;

public static partial class FixedSizeTheory
{
    public readonly ref struct SplitReadOnlySpanResult<TSize, T>
        where TSize : unmanaged, IFixedSizePremise<TSize>
    {
        internal SplitReadOnlySpanResult(FixedSizeReadOnlyChunkSpan<TSize, T> chunks, ReadOnlySpan<T> remainder)
        {
            Chunks = chunks;
            Remainder = remainder;
        }

        public FixedSizeReadOnlyChunkSpan<TSize, T> Chunks {get;}

        public ReadOnlySpan<T> Remainder {get;}

        public readonly Enumerator GetEnumerator() => new (this);

        public ref struct Enumerator
        {
            private readonly SplitReadOnlySpanResult<TSize, T> _source;
            private readonly FixedSizeReadOnlyChunkSpan<TSize, T>.Enumerator _chunkEnumerator;
            

            internal Enumerator(SplitReadOnlySpanResult<TSize, T> source)
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

            public readonly ReadOnlySpan<T> Current
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
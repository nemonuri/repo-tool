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

        public ref struct Enumerator
        {
            private readonly SplitSpanResult<TSize, T> _source;
            private readonly FixedSizeChunkSpan<TSize, T>.Enumerator _chunkEnumerator;
            private ReadOnlySpan<T> _current;
            

            internal Enumerator(SplitSpanResult<TSize, T> source)
            {
                _source = source;
                _chunkEnumerator = _source.Chunks.GetEnumerator();
                _current = default;
            }

            public bool MoveNext()
            {
                if (_chunkEnumerator.MoveNext())
                {
                    _current = _chunkEnumerator.Current;
                    return true;
                }
                else if (_chunkEnumerator.AreIndexLengthEqual && _source.Remainder.Length > 0)
                {
                    _current = _source.Remainder;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public readonly ReadOnlySpan<T> Current => _current;
        }
    }
}

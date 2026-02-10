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
            private ReadOnlySpan<T> _current;
            

            internal Enumerator(SplitReadOnlySpanResult<TSize, T> source)
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
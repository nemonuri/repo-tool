using Fst = Nemonuri.ByteChars.FixedSizeTheory;

namespace Nemonuri.ByteChars;

public readonly ref struct FixedSizeReadOnlyChunkSpan<TSize, T>
     where TSize : unmanaged, IFixedSizePremise<TSize>
{
    internal FixedSizeReadOnlyChunkSpan(ReadOnlySpan<T> rawValues, int chunkCount)
    {
        RawValues = rawValues;
        Length = chunkCount;
    }

    public readonly ReadOnlySpan<T> RawValues {get;}

    public readonly int Length {get;}

    public ReadOnlySpan<T> this[int chunkIndex] 
    { 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Fst.SliceSpanToChunk<TSize, T>(RawValues, chunkIndex); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AreAllMembersValid() => Fst.AreAllMembersOfReadOnlyChunkSpanValid<TSize, T>(RawValues, Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private readonly FixedSizeReadOnlyChunkSpan<TSize, T> _source;
        private int _index;

        internal Enumerator(FixedSizeReadOnlyChunkSpan<TSize, T> source)
        {
            _source = source;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _source.Length;
        }

        public readonly ReadOnlySpan<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source[_index];
        }
    }
}

using Fst = Nemonuri.ByteChars.FixedSizeTheory;

namespace Nemonuri.ByteChars;


public readonly ref struct FixedSizeChunkSpan<TSize, T>
     where TSize : unmanaged, IFixedSizePremise<TSize>
{
    internal FixedSizeChunkSpan(Span<T> rawValues, int chunkCount)
    {
        RawValues = rawValues;
        Length = chunkCount;
    }

    public readonly Span<T> RawValues {get;}

    public readonly int Length {get;}

    public Span<T> this[int chunkIndex] 
    { 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Fst.SliceSpanToChunk<TSize, T>(RawValues, chunkIndex); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AreAllMembersValid() => CheckAllMembersAreValidCore(throwIfNot: false);

    internal bool CheckAllMembersAreValidCore(bool throwIfNot)
    {
        if (!Fst.CheckFixedSizeIsGreaterThanZeroCore<TSize>(throwIfNot)) {return false;}
        
        //--- test length is valid ---
        if (throwIfNot)
        {
            Guard.IsEqualTo(FixedSizeTheory.UncheckedChunkIndexToSequenceIndex<TSize>(Length), RawValues.Length);
        }
        else
        {
            if (!(FixedSizeTheory.UncheckedChunkIndexToSequenceIndex<TSize>(Length) == RawValues.Length)) { return false; }
        }
        //---|

        return true;
    }

    public Enumerator GetEnumerator() => new(this);

    public ref struct Enumerator
    {
        private readonly FixedSizeChunkSpan<TSize, T> _source;
        private int _index;

        internal Enumerator(FixedSizeChunkSpan<TSize, T> source)
        {
            _source = source;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _source.Length;
        }

        public readonly Span<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source[_index];
        }
    }
}
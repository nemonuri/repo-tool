namespace Nemonuri.ByteChars;

public static partial class FixedSizeTheory
{
    extension<TPremise>(TPremise) /* TPremise */
        where TPremise : unmanaged, IFixedSizePremise<TPremise>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static ReadOnlySpan<T> SliceSpanToChunkUnaligned<T>(ReadOnlySpan<T> span, int spanIndex)
        {
            return span.Slice(spanIndex, GetFixedSize<TPremise>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static ReadOnlySpan<T> SliceSpanToChunk<T>(ReadOnlySpan<T> span, int chunkIndex)
        {
            int spanIndex = UncheckedChunkIndexToSequenceIndex<TPremise>(chunkIndex);
            return SliceSpanToChunkUnaligned<TPremise, T>(span, spanIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static SplitReadOnlySpanResult<TPremise, T> SplitSpan<T>(ReadOnlySpan<T> targetSpan)
        {
            (int chunkCount, _) = DivRemSequenceLength<TPremise>(targetSpan.Length);
            int rawLength = UncheckedChunkIndexToSequenceIndex<TPremise>(chunkCount);
            ReadOnlySpan<T> rawValues = targetSpan[..rawLength];
            ReadOnlySpan<T> remainder = targetSpan[rawLength..];
            return new(new(rawValues, chunkCount), remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CheckAllMembersOfReadOnlyChunkSpanAreValidCore<T>(ReadOnlySpan<T> rawValues, int length, bool throwIfNot)
        {
            if (!CheckFixedSizeIsGreaterThanZeroCore<TPremise>(throwIfNot)) {return false;}
            
            //--- test length is valid ---
            if (throwIfNot)
            {
                Guard.IsEqualTo(UncheckedChunkIndexToSequenceIndex<TPremise>(length), rawValues.Length);
            }
            else
            {
                if (!(UncheckedChunkIndexToSequenceIndex<TPremise>(length) == rawValues.Length)) { return false; }
            }
            //---|

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreAllMembersOfReadOnlyChunkSpanValid<T>(ReadOnlySpan<T> rawValues, int length) =>
            CheckAllMembersOfReadOnlyChunkSpanAreValidCore<TPremise, T>(rawValues, length, throwIfNot: true);
    }

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
    }
}
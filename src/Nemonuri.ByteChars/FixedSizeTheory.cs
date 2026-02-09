namespace Nemonuri.ByteChars;

public static class FixedSizeTheory
{
    extension<TPremise>(TPremise) /* TPremise */
        where TPremise : unmanaged, IFixedSizePremise<TPremise>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFixedSize()
        {
            TPremise th = new();
            return th.FixedSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFixedSizeGreaterThanZero() => GetFixedSize<TPremise>() <= 0;

        /// <exception cref="System.ArgumentOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CheckFixedSizeIsGreaterThanZeroCore(bool throwIfNot)
        {
            if (!IsFixedSizeGreaterThanZero<TPremise>())
            {
                if (throwIfNot)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException
                    (
                        name: $"{typeof(TPremise).ToTypeString()}.{nameof(GetFixedSize)}()",
                        value: GetFixedSize<TPremise>(),
                        message: $"{nameof(IsFixedSizeGreaterThanZero)} should {false}."
                    );
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <exception cref="System.ArgumentOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfFixedSizeIsLessThanOrEqualToZero()
        {
            CheckFixedSizeIsGreaterThanZeroCore<TPremise>(throwIfNot: true);
        }

        /// <param name="sequenceLength">The dividend.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int ChunkCount, int RemainedLength) UncheckedDivRemSequenceLength(int sequenceLength)
        {
            return 
#if NET8_0_OR_GREATER
                Math
#else
                Nemonuri.NetStandards.MathTheory
#endif
                    .DivRem(sequenceLength, GetFixedSize<TPremise>());
        }

        /// <param name="sequenceLength">The dividend.</param>
        /// <exception cref="System.ArgumentOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int ChunkCount, int RemainedLength) DivRemSequenceLength(int sequenceLength)
        {
            Guard.IsGreaterThanOrEqualTo(sequenceLength, 0);
            ThrowIfFixedSizeIsLessThanOrEqualToZero<TPremise>();
            return UncheckedDivRemSequenceLength<TPremise>(sequenceLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UncheckedChunkIndexToSequenceIndex(int chunkIndex) => chunkIndex * GetFixedSize<TPremise>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static Span<T> SliceSpanToChunkUnaligned<T>(Span<T> span, int spanIndex)
        {
            return span.Slice(spanIndex, GetFixedSize<TPremise>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static Span<T> SliceSpanToChunk<T>(Span<T> span, int chunkIndex)
        {
            int spanIndex = UncheckedChunkIndexToSequenceIndex<TPremise>(chunkIndex);
            return SliceSpanToChunkUnaligned<TPremise, T>(span, spanIndex);
        }

#if false
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <returns>
        ///     <see langword="true"/> if <paramref name="result"/> is chunk.
        ///     <see langword="false"/> if <paramref name="result"/> is remainder or empty.
        /// </returns>
        public static bool TrySliceSpanToChunk<T>(Span<T> span, int chunkIndex, out Span<T> result)
        {
            int spanIndex = UncheckedChunkIndexToSequenceIndex<TPremise>(chunkIndex);
            if (!(0 <= spanIndex && spanIndex < span.Length)) // && spanIndex + GetFixedSize<TPremise>()
            {
                // 'spanIndex' is out of range. 'result' is empty.
                result = default; return false;
            }
            else if (!(spanIndex + GetFixedSize<TPremise>() <= span.Length))
            {
                // Not enough to make chunk. 'result' is remainder.
                result = span.Slice(spanIndex); return false;
            }
            else
            {
                result = SliceSpanToChunkUnaligned<TPremise, T>(span, chunkIndex);
                return true;
            }
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <exception cref="System.ArgumentOutOfRangeException" />
        public static SplitSpanResult<TPremise, T> SplitSpan<T>(Span<T> targetSpan)
        {
            (int chunkCount, _) = DivRemSequenceLength<TPremise>(targetSpan.Length);
            int rawLength = UncheckedChunkIndexToSequenceIndex<TPremise>(chunkCount);
            Span<T> rawValues = targetSpan[..rawLength];
            Span<T> remainder = targetSpan[rawLength..];
            return new(new(rawValues, chunkCount), remainder);
        }
    }

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
    }
}

using Nemonuri.Buffers;

namespace Nemonuri.Transcodings;

public static class TranscoderTheory
{
    extension<TSource, TTarget, TConfig, TTranscoder>(TTranscoder)
        where TTranscoder : unmanaged, ITranscoderPremise<TSource, TTarget, TConfig>
    {
        /// <exception cref="System.OverflowException" />
        public static void TranscodeWhileDestinationTooSmall<TBufferWriter>(ReadOnlySpan<TSource> source, ref TBufferWriter dest, TConfig config, out int sourcesRead)
            where TBufferWriter : IBufferWriter<TTarget>
        {
            Guard.IsNotNull(dest);

            TTranscoder th = new();
            sourcesRead = 0;
            ReadOnlySpan<TSource> stepSrc = source;
            int prevDestLength = 0;
            bool prevAdvanced = false;

            while (true)
            {
                var stepDest = dest.GetSpan(prevAdvanced ? 0 : checked( prevDestLength * 2)); // 2147483647 bytes == 2.147483647 GB
                var stepSt = th.Transcode(stepSrc, stepDest, config, out int stepSr, out int stepTw);
                sourcesRead += stepSr;
                if (stepTw > 0)
                {
                    dest.Advance(stepTw);
                    prevAdvanced = true;
                }
                else
                {
                    prevAdvanced = false;
                }

                if (stepSt is not OperationStatus.DestinationTooSmall)
                {
                    return;
                }

                prevDestLength = stepDest.Length;
                stepSrc = stepSrc[stepSr..];
            }
        }

        public static ArraySegment<TTarget> TranscodeToArraySegmentWhileDestinationTooSmall(ReadOnlySpan<TSource> source, TConfig config, out int sourcesRead, int initialCapacity = -1)
        {
            DrainableArrayBuilder<TTarget> buffer = new(initialCapacity < 0 ? source.Length : initialCapacity);
            TranscodeWhileDestinationTooSmall<TSource, TTarget, TConfig, TTranscoder, DrainableArrayBuilder<TTarget>>(source, ref buffer, config, out sourcesRead);
            return buffer.DrainToArraySemgent();
        }
    }

    extension<TSource, TTarget, TTranscoder>(TTranscoder)
        where TTranscoder : unmanaged, ITranscoderPremise<TSource, TTarget>
    {
        /// <exception cref="System.OverflowException" />
        public static void TranscodeWhileDestinationTooSmall<TBufferWriter>(ReadOnlySpan<TSource> source, ref TBufferWriter dest, out int sourcesRead)
            where TBufferWriter : IBufferWriter<TTarget>
            =>
            TranscodeWhileDestinationTooSmall<TSource, TTarget, ValueTuple, ConfiglessTranscoder<TSource, TTarget, TTranscoder>, TBufferWriter>(source, ref dest, default, out sourcesRead);

        public static ArraySegment<TTarget> TranscodeToArraySegmentWhileDestinationTooSmall(ReadOnlySpan<TSource> source, out int sourcesRead, int initialCapacity = -1)
            =>
            TranscodeToArraySegmentWhileDestinationTooSmall<TSource, TTarget, ValueTuple, ConfiglessTranscoder<TSource, TTarget, TTranscoder>>(source, default, out sourcesRead, initialCapacity);
    }
}
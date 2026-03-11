using Nemonuri.Buffers;

namespace Nemonuri.Transcodings;

public static class TranscodingTheory
{
    extension<TSource, TTarget, TPremise>(TPremise)
        where TPremise : unmanaged, ITranscodingPremise<TSource, TTarget>
    {
        public static void TranscodeWhileDestinationTooSmall<TBufferWriter>(ReadOnlySpan<TSource> source, ref TBufferWriter dest, out int sourcesRead)
            where TBufferWriter : IBufferWriter<TTarget>
        {
            Guard.IsNotNull(dest);

            TPremise th = new();
            sourcesRead = 0;
            ReadOnlySpan<TSource> stepSrc = source;
            int prevDestLength = 0;
            bool prevAdvanced = false;

            while (true)
            {
                var stepDest = dest.GetSpan(prevAdvanced ? 0 : prevDestLength * 2);
                var stepSt = th.Transcode(stepSrc, stepDest, out int stepSr, out int stepTw);
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

        public static ArraySegment<TTarget> TranscodeToArraySegmentWhileDestinationTooSmall(ReadOnlySpan<TSource> source, out int sourcesRead, int initialCapacity = -1)
        {
            DrainableArrayBuilder<TTarget> buffer = new(initialCapacity < 0 ? source.Length : initialCapacity);
            TranscodeWhileDestinationTooSmall<TSource, TTarget, TPremise, DrainableArrayBuilder<TTarget>>(source, ref buffer, out sourcesRead);
            return buffer.DrainToArraySemgent();
        }
    }
}
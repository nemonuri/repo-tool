using Nemonuri.Collections;

namespace Nemonuri.Transcodings;

public static class TranscodingTheory
{
    extension<TSource, TTarget, TPremise>(TPremise)
        where TPremise : unmanaged, ITranscodingPremise<TSource, TTarget>
    {
        public static ArraySegment<TTarget> TranscodeWhileDestinationTooSmall(ReadOnlySpan<TSource> source, out int sourcesRead)
        {
            TPremise th = new();
            DrainableArrayBuilder<TTarget> builder = new ();
            sourcesRead = 0;
            int targetWritten = 0;
            ReadOnlySpan<TSource> stepSrc = source;
            int stepDestLength = source.Length;

            while (true)
            {
                var stepDest = builder.AppendSpan(stepDestLength);
                var stepSt = th.Transcode(stepSrc, stepDest, out int stepSr, out int stepTw);
                sourcesRead += stepSr;
                targetWritten += stepTw;
                builder.SetCount(targetWritten);

                if (stepSt is not OperationStatus.DestinationTooSmall)
                {
                    return builder.DrainToArraySemgent();
                }

                sourcesRead += stepSr;
                stepSrc = stepSrc[stepSr..];
            }
        }
    }
}
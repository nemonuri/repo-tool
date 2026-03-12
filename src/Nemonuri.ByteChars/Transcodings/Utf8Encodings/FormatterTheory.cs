using Nemonuri.FixedSizes;

namespace Nemonuri.Transcodings.Utf8Encodings;

public static class FormatterTheory
{
    extension<T, TSize, TFormatter>(TFormatter)
        where T : unmanaged
        where TSize : unmanaged, IFixedSizePremise
        where TFormatter : unmanaged, IFormatterPremise<T, TSize>
    {
        public static OperationStatus FormatUnmanaged(T source, Span<byte> destination, out int sourcesRead, out int targetsWritten)
        {
            FormatterBasedTranscoder<T, TSize, TFormatter> th = new();

            Span<T> sources = stackalloc T[] { source };

            return th.Transcode(sources, destination, out sourcesRead, out targetsWritten);
        }

        public static bool TryFormatUnmanagedToArraySegment(T source, out ArraySegment<byte> result)
        {
            Span<T> sources = stackalloc T[] { source };
            ArraySegment<byte> resultCandidate = TranscoderTheory.TranscodeToArraySegmentWhileDestinationTooSmall<T, byte, FormatterBasedTranscoder<T, TSize, TFormatter>>(sources, out int sourcesRead, FixedSizeTheory.GetFixedSize<TSize>());
            if (sourcesRead < 1)
            {
                // Source was not consumed. fail.

                result = default;
                return false;
            }
            else
            {
                result = resultCandidate;
                return true;
            }
        }
    }
}
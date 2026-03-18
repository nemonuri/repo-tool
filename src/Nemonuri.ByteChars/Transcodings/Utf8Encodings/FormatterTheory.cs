namespace Nemonuri.Transcodings.Utf8Encodings;

#if false
public static class FormatterTheory
{
    extension<T, TFormat, TFormatter>(TFormatter)
        where T : unmanaged
        where TFormatter : unmanaged, IFormatterPremise<T, TFormat>
    {
        public static OperationStatus FormatUnmanaged(T source, Span<byte> destination, FormatConfig<TFormat> config, out int sourcesRead, out int targetsWritten)
        {
            FormatterBasedTranscoder<T, TFormat, TFormatter> th = new();

            Span<T> sources = stackalloc T[] { source };

            return th.Transcode(sources, destination, config, out sourcesRead, out targetsWritten);
        }

        public static bool TryFormatUnmanagedToArraySegment(T source, FormatConfig<TFormat> config, out ArraySegment<byte> result)
        {
            Span<T> sources = stackalloc T[] { source };
            ArraySegment<byte> resultCandidate = TranscoderTheory.TranscodeToArraySegmentWhileDestinationTooSmall<T, byte, FormatConfig<TFormat>, FormatterBasedTranscoder<T, TFormat, TFormatter>>(sources, config, out int sourcesRead, config.MaxLength);
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
#endif
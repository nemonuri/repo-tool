namespace Nemonuri.Transcodings;

public interface ITranscoderPremise<TSource, TTarget>
{
    OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, out int sourcesRead, out int targetsWritten);
}

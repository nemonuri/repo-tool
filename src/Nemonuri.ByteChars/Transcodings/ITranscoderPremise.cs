namespace Nemonuri.Transcodings;

public interface ITranscoderPremise<TSource, TTarget>
{
    OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, out int sourcesRead, out int targetsWritten);
}

public interface ITranscoderPremise<TSource, TTarget, TConfig>
{
    OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, TConfig config, out int sourcesRead, out int targetsWritten);
}

public readonly struct ConfiglessTranscoder<TSource, TTarget, TTranscoder> : ITranscoderPremise<TSource, TTarget, ValueTuple>
    where TTranscoder : unmanaged, ITranscoderPremise<TSource, TTarget>
{
    public OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, ValueTuple _, out int sourcesRead, out int targetsWritten) =>
        (new TTranscoder()).Transcode(source, destination, out sourcesRead, out targetsWritten);
}
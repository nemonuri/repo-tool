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

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct TranscoderHandle<TSource, TTarget, TConfig>
{
    private readonly delegate*<ReadOnlySpan<TSource>, Span<TTarget>, TConfig, out int, out int, OperationStatus> _fp;

    internal TranscoderHandle(delegate*<ReadOnlySpan<TSource>, Span<TTarget>, TConfig, out int, out int, OperationStatus> fp) => _fp = fp;

    public OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, TConfig config, out int sourcesRead, out int targetsWritten) =>
        _fp(source, destination, config, out sourcesRead, out targetsWritten);
}
using System.Buffers;
using System.Diagnostics;

namespace Nemonuri.Transcodings;

public interface ITranscodePremise<TSource, TTarget>
{
    OperationStatus Transcode(ReadOnlySpan<TSource> source, Span<TTarget> destination, out int sourcesRead, out int targetsWritten);
}

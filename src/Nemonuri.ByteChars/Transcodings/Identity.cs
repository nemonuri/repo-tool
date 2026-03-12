using System.Diagnostics;

namespace Nemonuri.Transcodings;

public readonly struct Identity<T> : ITranscoderPremise<T, T>
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<T> destination, out int sourcesRead, out int targetsWritten)
    {
        if (source.Length <= destination.Length)
        {
            source.CopyTo(destination);
            sourcesRead = targetsWritten = source.Length;
            return OperationStatus.Done;
        }
        else
        {
            Debug.Assert( source.Length > destination.Length );

            source[..destination.Length].CopyTo(destination);
            sourcesRead = targetsWritten = destination.Length;
            return OperationStatus.DestinationTooSmall;
        }
    }
}

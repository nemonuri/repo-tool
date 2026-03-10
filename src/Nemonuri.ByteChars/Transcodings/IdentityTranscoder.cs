using System.Diagnostics;

namespace Nemonuri.Transcodings;

public readonly struct Identity<T> : ITranscodePremise<T, T>
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


public readonly struct UncheckedUtf8UnixToWindowsNewLine : ITranscodePremise<byte,byte>
{
    public OperationStatus Transcode(ReadOnlySpan<byte> source, Span<byte> destination, out int sourcesRead, out int targetsWritten)
    {
        // Assume source is valid Utf8. ( Unchecked )

        // Find index of '\n' which is not following '\r'.
        int nIndex = source.
        
        
    }
}
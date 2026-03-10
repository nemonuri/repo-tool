#if false
using Nemonuri.Transcodings;
using Nemonuri.ByteChars.Internal;
using System.Diagnostics;

namespace Nemonuri.ByteChars;

public readonly struct DotNetEncodingBasedByteCharTranscoder : ITranscoder<byte, byte>
{
    private readonly Encoding? _sourceEncoding;

    private readonly Encoding? _targetEncoding;

    public DotNetEncodingBasedByteCharTranscoder(Encoding? sourceEncoding, Encoding? targetEncoding)
    {
        _sourceEncoding = sourceEncoding;
        _targetEncoding = targetEncoding;
    }

    public readonly Encoding SourceEncoding => _sourceEncoding ?? Encoding.Default;

    public readonly Encoding TargetEncoding => _targetEncoding ?? Encoding.Default;


    public OperationStatus Transcode(ReadOnlySpan<byte> source, Span<byte> destination, out int sourcesRead, out int targetsWritten)
    {
        if (SourceEncoding.Equals(TargetEncoding))
        {
            return new IdentityTranscoder<byte>().Transcode(source, destination, out sourcesRead, out targetsWritten);
        }

        if (source.IsEmpty)
        {
            sourcesRead = targetsWritten = 0;
            return OperationStatus.Done;
        }

        ReadOnlySpan<byte> stepSource = source;
        
        while (stepSource.Length > 0)
        {
            int maxRequriedDestLength = TargetEncoding.GetMaxByteCount(SourceEncoding.GetMaxCharCount(stepSource.Length));

            if (maxRequriedDestLength <= destination.Length)
            {
                break;
            }

            Debug.Assert( maxRequriedDestLength > destination.Length );

            int nextSourceLength = Math.Max((int)Math.Floor(stepSource.Length * ((double)destination.Length / maxRequriedDestLength)), 0);
            stepSource = stepSource[..nextSourceLength];
        }

        if (stepSource.IsEmpty)
        {
            sourcesRead = targetsWritten = 0;
            return OperationStatus.DestinationTooSmall;
        }

        int maxRequriedTempCharLength = SourceEncoding.GetMaxCharCount(stepSource.Length);

        char[]? rented = null;
        Span<char> tempChars = 
            (maxRequriedTempCharLength <= (InternalConstants.StackAllocThreshold / sizeof(char))) ?
                stackalloc char[maxRequriedTempCharLength] : 
                (rented = ArrayPool<char>.Shared.Rent(maxRequriedTempCharLength)).AsSpan();
        
        int writtenChars = 
#if NETSTANDARD2_1_OR_GREATER
            SourceEncoding.GetChars(stepSource, tempChars);
#else
            Nemonuri.NetStandards.Text.EncodingTheory.GetChars(SourceEncoding, stepSource, tempChars);
#endif
        var tempCharsSliced = tempChars[..writtenChars];
        int writtenBytes = 
#if NETSTANDARD2_1_OR_GREATER
            TargetEncoding.GetBytes(tempCharsSliced, destination);
#else
            Nemonuri.NetStandards.Text.EncodingTheory.GetBytes(TargetEncoding, tempCharsSliced, destination);
#endif

        if (rented is not null) { ArrayPool<char>.Shared.Return(rented); }

        sourcesRead = stepSource.Length;
        targetsWritten = writtenBytes;

        return (sourcesRead == source.Length) ? OperationStatus.Done : OperationStatus.DestinationTooSmall;
    }
}
#endif
using System.Diagnostics;
using Nemonuri.Transcodings;
using static Nemonuri.ByteChars.ByteCharConstants;

namespace Nemonuri.ByteChars;

public readonly struct UncheckedUtf8UnixToWindowsNewLine : ITranscoderPremise<byte,byte>
{
    public OperationStatus Transcode(ReadOnlySpan<byte> source, Span<byte> destination, out int sourcesRead, out int targetsWritten)
    {
        // Assume source is valid Utf8. ( Unchecked )

        // Find index of '\n' which is not following '\r'.
        Identity<byte> id = default;

        int nIndex = source.IndexOf(AsciiLineFeed);
        if (nIndex < 0) 
        { 
            if (source.Length > 0 && source[^1] == AsciiCarriageReturn)
            {
                int trimCount = 1;
                while (true)
                {
                    int nextTrimCount = trimCount+1;

                    if (!(nextTrimCount < source.Length)) { break; }
                    if (source[^nextTrimCount] != AsciiCarriageReturn) { break; }

                    trimCount = nextTrimCount;
                }

                var nextSrc = source[..^trimCount];
                var accStatus = Transcode(nextSrc, destination, out int accSr, out int accTw);
                sourcesRead = accSr;
                targetsWritten = accTw;
                return accStatus is OperationStatus.Done ? OperationStatus.NeedMoreData : accStatus;
            }
            else
            {
                return id.Transcode(source, destination, out sourcesRead, out targetsWritten);     
            }
        }
        else if (nIndex == 0)
        {
            if (destination.Length < 2)
            {
                sourcesRead = targetsWritten = 0;
                return OperationStatus.DestinationTooSmall;
            }

            destination[0] = AsciiCarriageReturn;
            destination[1] = source[0];

            var nextSrc = source[1..];
            var nextDest = destination[2..];
            var accStatus = Transcode(nextSrc, nextDest, out int accSr, out int accTw);
            sourcesRead = accSr + 1;
            targetsWritten = accTw + 2;

            return accStatus;
        }
        else
        {
            Debug.Assert ( nIndex > 0 );

            int copyLength = nIndex; //- 1;
            var copyDest = destination;
            if (source[nIndex - 1] == AsciiCarriageReturn)
            {
                copyLength += 1;
                copyDest = copyDest[..^(copyDest.Length % 2)];
            }
            var copySrc = source[..copyLength];

            var copyRst = id.Transcode(copySrc, copyDest, out int copyRsr, out int copyRtw);

            if (copyRst == OperationStatus.DestinationTooSmall)
            {
                sourcesRead = copyRsr; 
                targetsWritten = copyRtw;
                return copyRst;
            }

            var nextSrc = source[copyRsr..];
            var nextDest = destination[copyRtw..];
            var accSt = Transcode(nextSrc, nextDest, out int accSr, out int accTw);
            sourcesRead = accSr + copyRsr;
            targetsWritten = accTw + copyRtw;
            return accSt;
        }
    }
}

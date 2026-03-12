using Nemonuri.ByteChars.Internal;
using Nemonuri.FixedSizes;
using static Nemonuri.Buffers.ArrayPoolTheory;

namespace Nemonuri.Transcodings.Utf8Encodings;

public interface IFormatterPremise<T, TSize> 
    where TSize : unmanaged, IFixedSizePremise
{
    StandardFormat StandardFormat {get;}
    bool TryFormat(T value, Span<byte> destination, out int bytesWritten, StandardFormat format = default);
}

public readonly struct DefaultSize : IFixedSizePremise
{
    public int FixedSize => InternalConstants.StackAllocThreshold;
}

public readonly struct FormatterBasedTranscoder<T, TSize, TFormatter> : ITranscoderPremise<T, byte>
    where TSize : unmanaged, IFixedSizePremise
    where TFormatter : unmanaged, IFormatterPremise<T, TSize>
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<byte> destination, out int sourcesRead, out int targetsWritten)
    {
        TFormatter th = new();
        StandardFormat fmt = th.StandardFormat;
        int tempStorageLength = FixedSizeTheory.GetFixedSize<TSize>();

        sourcesRead = 0;
        targetsWritten = 0;

        Span<byte> stepDest = destination;
        byte[]? buffer = null;
        Span<byte> tempStorage = (tempStorageLength < InternalConstants.StackAllocThreshold) ? 
                                    stackalloc byte[tempStorageLength] : (buffer = ArrayPool<byte>.Shared.Rent(tempStorageLength)).AsSpan()[..tempStorageLength];

        foreach (var elem in source)
        {
            if (!th.TryFormat(elem, tempStorage, out int stepBw, fmt))
            {
                /**
                    According to [doc](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=net-10.0#system-buffers-text-utf8formatter-tryformat(system-boolean-system-span((system-byte))-system-int32@-system-buffers-standardformat)),
                    `false` means `destination` is too small.

                    But, this library expects consumer declared enough temp storage size, so return `InvalidData`.
                */

                // Do not touch`destination`, `sourcesRead` and `targetsWritten`.

                ReturnAndNullToShared(ref buffer);
                return OperationStatus.InvalidData;
            }

            // Format successed.

            var stepCopySrc = tempStorage[..stepBw];
            if (stepDest.Length < stepCopySrc.Length)
            {
                // Destination is too small.

                ReturnAndNullToShared(ref buffer);
                return OperationStatus.DestinationTooSmall;
            }

            stepCopySrc.CopyTo(stepDest);
            sourcesRead += 1;
            targetsWritten += stepBw;
            stepDest = stepDest[stepBw..];
        }

        // Done!
        ReturnAndNullToShared(ref buffer);
        return OperationStatus.Done;
    }
}

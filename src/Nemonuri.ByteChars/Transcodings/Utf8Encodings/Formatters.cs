using Nemonuri.ByteChars.Internal;
using Nemonuri.FixedSizes;
using static Nemonuri.Buffers.ArrayPoolTheory;

namespace Nemonuri.Transcodings.Utf8Encodings;

public interface IFormatterPremise<T, TFormat> 
{
    bool TryFormat(T value, Span<byte> destination, TFormat format, out int bytesWritten);
}

public readonly struct FormatConfig<TFormat>(TFormat format, int maxLength)
{
    public TFormat Format {get;} = format;
    public int MaxLength {get;} = maxLength;
}

public readonly struct FormatterBasedTranscoder<T, TFormat, TFormatter> : ITranscoderPremise<T, byte, FormatConfig<TFormat>>
    where TFormatter : unmanaged, IFormatterPremise<T, TFormat>
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<byte> destination, FormatConfig<TFormat> formatConfig, out int sourcesRead, out int targetsWritten)
    {
        TFormatter th = new();
        TFormat format = formatConfig.Format;
        int tempStorageLength = formatConfig.MaxLength;

        sourcesRead = 0;
        targetsWritten = 0;

        Span<byte> stepDest = destination;
        byte[]? buffer = null;
        Span<byte> tempStorage = (tempStorageLength < InternalConstants.StackAllocThreshold) ? 
                                    stackalloc byte[tempStorageLength] : (buffer = ArrayPool<byte>.Shared.Rent(tempStorageLength)).AsSpan()[..tempStorageLength];

        foreach (var elem in source)
        {
            if (!th.TryFormat(elem, tempStorage, format, out int stepBw))
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

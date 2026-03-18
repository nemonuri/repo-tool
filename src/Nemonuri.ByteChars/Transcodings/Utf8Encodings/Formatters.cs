using Nemonuri.ByteChars.Internal;
using Nemonuri.FixedSizes;
using static Nemonuri.Buffers.ArrayPoolTheory;

namespace Nemonuri.Transcodings.Utf8Encodings;

public interface IFormatterPremise<T, TFormat> 
{
    bool TryFormat(T value, Span<byte> destination, TFormat format, out int bytesWritten);
}

public readonly struct FormatConfig<T, TFormat>(TFormat format, MaxLengthHandle<T, TFormat> maxLengthHandle)
{
    public TFormat Format {get;} = format;
    public MaxLengthHandle<T, TFormat> MaxLengthHandle {get;} = maxLengthHandle;
}

public readonly struct FormatterBasedTranscoder<T, TFormat, TFormatter> : ITranscoderPremise<T, byte, FormatConfig<T, TFormat>>
    where TFormatter : unmanaged, IFormatterPremise<T, TFormat>
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<byte> destination, FormatConfig<T, TFormat> formatConfig, out int sourcesRead, out int targetsWritten)
    {
        TFormatter th = new();
        TFormat format = formatConfig.Format;
        Guard.IsTrue(formatConfig.MaxLengthHandle.HasValue);

        sourcesRead = 0;
        targetsWritten = 0;

        Span<byte> stepDest = destination;
        int tempStorageLength; //= formatConfig.MaxLength;
        byte[]? tempBuffer = null;
        //Span<byte> tempStorage; // = (tempStorageLength < InternalConstants.StackAllocThreshold) ? stackalloc byte[tempStorageLength] : (buffer = ArrayPool<byte>.Shared.Rent(tempStorageLength)).AsSpan()[..tempStorageLength];

#pragma warning disable CA2014 // Do not use stackalloc in loops
        foreach (var elem in source)
        {
            tempStorageLength = formatConfig.MaxLengthHandle.GetMaxLength(in elem, in format);
            Span<byte> tempStorage = (tempStorageLength < InternalConstants.StackAllocThreshold) ? stackalloc byte[tempStorageLength] : (tempBuffer = ArrayPool<byte>.Shared.Rent(tempStorageLength)).AsSpan()[..tempStorageLength];
            
            if (!th.TryFormat(elem, tempStorage, format, out int stepBw))
            {
                /**
                    According to [doc](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=net-10.0#system-buffers-text-utf8formatter-tryformat(system-boolean-system-span((system-byte))-system-int32@-system-buffers-standardformat)),
                    `false` means `destination` is too small.

                    But, this library expects consumer declared enough temp storage size, so return `InvalidData`.
                */

                // Do not touch`destination`, `sourcesRead` and `targetsWritten`.

                ReturnAndNullToShared(ref tempBuffer);
                return OperationStatus.InvalidData;
            }

            // Format successed.

            var stepCopySrc = tempStorage[..stepBw];
            if (stepDest.Length < stepCopySrc.Length)
            {
                // Destination is too small.

                ReturnAndNullToShared(ref tempBuffer);
                return OperationStatus.DestinationTooSmall;
            }

            stepCopySrc.CopyTo(stepDest);
            sourcesRead += 1;
            targetsWritten += stepBw;
            stepDest = stepDest[stepBw..];
            ReturnAndNullToShared(ref tempBuffer);
        }
#pragma warning restore CA2014 // Do not use stackalloc in loops

        // Done!
        return OperationStatus.Done;
    }
}


public readonly struct FormatterBasedTranscoder<T, TFormat, TFormatter, TMaxLength> : ITranscoderPremise<T, byte, TFormat>
    where TFormatter : unmanaged, IFormatterPremise<T, TFormat>
    where TMaxLength : unmanaged, IMaxLengthPremise<T, TFormat>
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<byte> destination, TFormat format, out int sourcesRead, out int targetsWritten) =>
        (new FormatterBasedTranscoder<T, TFormat, TFormatter>()).Transcode(source, destination, new(format, MaxLengthTheory.ToHandle<T,TFormat,TMaxLength>()), out sourcesRead, out targetsWritten);
}


public readonly struct FormatterBasedFixedSizeTranscoder<T, TFormat, TFormatter, TFixedSize> : ITranscoderPremise<T, byte, TFormat>
    where TFormatter : unmanaged, IFormatterPremise<T, TFormat>
    where TFixedSize : unmanaged, IFixedSizePremise
{
    public OperationStatus Transcode(ReadOnlySpan<T> source, Span<byte> destination, TFormat format, out int sourcesRead, out int targetsWritten) =>
        (new FormatterBasedTranscoder<T, TFormat, TFormatter, FixedMaxLength<T, TFormat, TFixedSize>>()).Transcode(source, destination, format, out sourcesRead, out targetsWritten);
}

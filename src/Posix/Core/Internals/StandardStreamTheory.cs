using System.Diagnostics.CodeAnalysis;
using static Nemonuri.Posix.Internals.RawFileDescriptorTheory;

namespace Nemonuri.Posix.Internals;

internal static class StandardStreamTheory
{
    public static StandardStream GetIn() => StandardStream.In;

    public static StandardStream GetOut() => StandardStream.Out;

    public static StandardStream GetError() => StandardStream.Error;

    public static bool TryGetFromRawFileDescriptor(nint fildes, [NotNullWhen(true)] out StandardStream? standardStream)
    {
        standardStream = fildes switch
        {
            Unistd.STDERR_FILENO => GetError(),
            Unistd.STDIN_FILENO => GetIn(),
            Unistd.STDOUT_FILENO => GetOut(),
            _ => null
        };
        return standardStream is not null;
    }

    public static StandardStream GetFromRawFileDescriptor(nint fildes) => 
        TryGetFromRawFileDescriptor(fildes, out var standardStream) ? standardStream : 
        throw new ArgumentException(String.Format("Invalid file discriptor. {0} = {1}", nameof(fildes), fildes));
}


using Nemonuri.Posix.Internals;
using static Nemonuri.Posix.Internals.RawFileDescriptorTheory;

namespace Nemonuri.Posix;

public static class PosixFileInfoTheory
{
    private static PosixFileInfo? s_in;

    private static PosixFileInfo? s_out;

    private static PosixFileInfo? s_error;

    public static PosixFileInfo GetIn() => s_in ??= PosixFileInfo.CreateUnclosable(StandardStreamTheory.GetIn(), new(Unistd.STDIN_FILENO));

    public static PosixFileInfo GetOut() => s_out ??= PosixFileInfo.CreateUnclosable(StandardStreamTheory.GetOut(), new(Unistd.STDOUT_FILENO));

    public static PosixFileInfo GetError() => s_error ??= PosixFileInfo.CreateUnclosable(StandardStreamTheory.GetError(), new(Unistd.STDERR_FILENO));

    private static bool TryGetStandardFromRawFileDiscriptor(nint fildes, [NotNullWhen(true)] out PosixFileInfo? posixFileInfo)
    {
        posixFileInfo = fildes switch
        {
            Unistd.STDERR_FILENO => GetError(),
            Unistd.STDIN_FILENO => GetIn(),
            Unistd.STDOUT_FILENO => GetOut(),
            _ => null
        };
        return posixFileInfo is not null;
    }

    public static bool TryGetStandardFromFileDiscriptor(FileDescriptor fileDescriptor, [NotNullWhen(true)] out PosixFileInfo? posixFileInfo) =>
        TryGetStandardFromRawFileDiscriptor(fileDescriptor.ToIntPtr(), out posixFileInfo);
}

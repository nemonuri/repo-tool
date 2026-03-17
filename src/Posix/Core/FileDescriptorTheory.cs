namespace Nemonuri.Posix;

public static class FileDescriptorTheory
{
    public static bool TryToPosixFileInfo(FileDescriptor fd, [NotNullWhen(true)] out PosixFileInfo? posixFileInfo)
    {
        if (fd.TryToGCHandle(out var gcHandle) && GCHandleTheory.TryGetObject(gcHandle, out posixFileInfo))
        {
            return true;
        }

        return PosixFileInfoTheory.TryGetStandardFromFileDiscriptor(fd, out posixFileInfo);
    }
}

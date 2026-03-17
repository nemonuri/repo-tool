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

    public static bool CanWrite(FileDescriptor fd)
    {
        if (!TryToPosixFileInfo(fd, out var pfi)) { return false; }
        return pfi.CanWrite;
    }

    public static bool CanRead(FileDescriptor fd)
    {
        if (!TryToPosixFileInfo(fd, out var pfi)) { return false; }
        return pfi.CanRead;
    }

    public static bool IsClosed(FileDescriptor fd)
    {
        if (!TryToPosixFileInfo(fd, out var pfi)) { return false; }
        return pfi.IsClosed;
    }
}

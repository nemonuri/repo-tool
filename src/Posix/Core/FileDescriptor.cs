using System.Diagnostics;
using Nemonuri.Posix.Internals;

namespace Nemonuri.Posix;

[StructLayout(LayoutKind.Explicit)]
public readonly struct FileDescriptor
{
    [FieldOffset(0)]
    private readonly nint _rawFd;

    [FieldOffset(0)]
    private readonly GCHandle _gcHandle;

    public nint ToIntPtr() => _rawFd;

    public bool IsAtty() => RawFileDescriptorTheory.Unistd.isatty(ToIntPtr()) != 0;

    public bool TryToGCHandle(out GCHandle gcHandle)
    {
        if (IsAtty())
        {
            gcHandle = default;
            return false;
        }
        else
        {
            gcHandle = _gcHandle;
            return true;
        }
    }

    internal FileDescriptor(nint raw)
    {
        _rawFd = raw;
    }

    internal FileDescriptor(GCHandle gcHandle)
    {
        Debug.Assert( RawFileDescriptorTheory.Unistd.isatty(GCHandle.ToIntPtr(gcHandle)) == 0 );
        _gcHandle = gcHandle;
    }
}

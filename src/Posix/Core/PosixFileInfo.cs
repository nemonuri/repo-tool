using System.Diagnostics;
using Nemonuri.Posix.Internals;

namespace Nemonuri.Posix;

public class PosixFileInfo : IDisposable
{
    public static PosixFileInfo Null {get;} = new(null, null, true, true);

    private readonly Stream? _stream;

    private readonly bool _closable;

    private readonly FileDescriptor? _fileDescriptor;

    private int _closed;

    private PosixFileInfo(Stream? stream, FileDescriptor? fileDescriptor, bool closed, bool closable)
    {
        _stream = stream;
        _fileDescriptor = fileDescriptor;
        _closed = closed ? 1 : 0;
        _closable = closable;
    }

    internal static PosixFileInfo CreateUnclosable(Stream stream, FileDescriptor? fileDescriptor)
    {
        Debug.Assert( stream is not null );

        return new(stream, fileDescriptor, false, false);
    }

    public bool IsClosed => _closed is not 0;

    [MemberNotNullWhen(true, nameof(Stream))]
    public bool HasStream => _stream is not null;

    public Stream? Stream => _stream;

    public void Dispose()
    {
        if (!_closable) { return; }
        var closed0 = Interlocked.CompareExchange(ref _closed, 1, 0);
        if (closed0 is 0)
        {
            if 
            (
                _fileDescriptor is { } fd && 
                fd.TryToGCHandle(out var gcHandle)
            )
            {
                GCHandleTheory.FreeIfAllocated(gcHandle);
            }
            _stream?.Dispose();
        }
    }

    public bool CanRead
    {
        get
        {
            if (IsClosed) { return false; }
            if (!HasStream) { return false; }
            if (!Stream.CanRead) { return false; }
            return true;
        }
    }

    public bool CanWrite
    {
        get
        {
            if (IsClosed) { return false; }
            if (!HasStream) { return false; }
            if (!Stream.CanWrite) { return false; }
            return true;
        }
    }

    public FileDescriptor? FileDiscriptorOrNull => _fileDescriptor;
}

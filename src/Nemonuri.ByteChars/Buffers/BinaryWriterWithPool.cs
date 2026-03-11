using CommunityToolkit.HighPerformance;
using Nemonuri.ByteChars.Internal;
using Nemonuri.ByteChars.IO;

namespace Nemonuri.Buffers;

public struct BinaryWriterWithPool : IBufferWriter<byte>, IDisposable
{
    private readonly ArrayPool<byte>? _pool;

    private readonly BinaryWriter? _writer;

    private readonly int _defaultBufferSize;

    private byte[]? _buffer;

    public BinaryWriterWithPool(ArrayPool<byte>? pool, BinaryWriter? writer, int defaultBufferSize)
    {
        _pool = pool;
        _writer = writer;
        _defaultBufferSize = defaultBufferSize;
    }

    private readonly ArrayPool<byte> Pool => _pool ?? ArrayPool<byte>.Shared;

    private readonly BinaryWriter Writer => _writer ?? BinaryWriter.Null;

    private readonly int DefaultBufferSize => _defaultBufferSize > 0 ? _defaultBufferSize : InternalConstants.StackAllocThreshold;

    private readonly int GetNewSize(int sizeHint)
    {
        Guard.IsGreaterThanOrEqualTo(sizeHint, 0);
        return sizeHint is 0 ? DefaultBufferSize : sizeHint;
    }

    public void Advance(int count)
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            BinaryWriterTheory.WriteByteSpan(Writer, buffer.AsSpan()[..count]);
            Pool.Return(buffer);            
        }
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        int newSize = GetNewSize(sizeHint);
        Pool.Resize(ref _buffer, newSize);
        return new(_buffer, 0, newSize);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        int newSize = GetNewSize(sizeHint);
        Pool.Resize(ref _buffer, newSize);
        return new(_buffer, 0, newSize);
    }

    public void Dispose()
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            Pool.Return(buffer);
        }
    }
}
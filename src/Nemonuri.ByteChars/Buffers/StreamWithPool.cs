using CommunityToolkit.HighPerformance;
using Nemonuri.ByteChars.Internal;
using Nemonuri.ByteChars.IO;

namespace Nemonuri.Buffers;

public struct StreamWithByteArrayPool : IBufferWriter<byte>, IDisposable
{
    private readonly ArrayPool<byte>? _pool;

    private readonly Stream? _stream;

    private readonly int _defaultBufferSize;

    private byte[]? _buffer;

    public StreamWithByteArrayPool(ArrayPool<byte>? pool, Stream? stream, int defaultBufferSize)
    {
        _pool = pool;
        _stream = stream;
        _defaultBufferSize = defaultBufferSize;
    }

    private readonly ArrayPool<byte> Pool => _pool ?? ArrayPool<byte>.Shared;

    private readonly Stream Stream => _stream ?? Stream.Null;

    public void Advance(int count)
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            StreamTheory.WriteByteSpan(Stream, buffer.AsSpan()[..count]);
            Pool.Return(buffer);            
        }
    }

    public Memory<byte> GetMemory(int sizeHint = 0) =>
        ArrayPoolTheory.ResizeAndSlice(Pool, ref _buffer, sizeHint, _defaultBufferSize).AsMemory();

    public Span<byte> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;

    public void Dispose()
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            Pool.Return(buffer);
        }
    }
}
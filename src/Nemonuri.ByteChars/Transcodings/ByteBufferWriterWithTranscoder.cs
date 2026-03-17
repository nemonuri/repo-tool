using Nemonuri.Buffers;

namespace Nemonuri.Transcodings;

public struct DisposableByteBufferWriterWithTranscoder<TBufferWriter, TTranscoder> : IBufferWriter<byte>, IDisposable
    where TBufferWriter : IBufferWriter<byte>, IDisposable
    where TTranscoder : unmanaged, ITranscoderPremise<byte, byte>
{
    private TBufferWriter _bufferWriter;

    public DisposableByteBufferWriterWithTranscoder(TBufferWriter bufferWriter)
    {
        _bufferWriter = bufferWriter;
    }

    private readonly ArrayPool<byte> Pool => ArrayPool<byte>.Shared;

    private byte[]? _buffer;

    public void Advance(int count)
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            TranscoderTheory.TranscodeWhileDestinationTooSmall<byte,byte,TTranscoder,TBufferWriter>
                (buffer.AsSpan()[..count], ref _bufferWriter, out int sourcesRead); /* Todo: Handle 'remained' source. */

            Pool.Return(buffer);            
        }
        
    }

    public Memory<byte> GetMemory(int sizeHint = 0) => ArrayPoolTheory.ResizeAndSlice(Pool, ref _buffer, sizeHint, 0).AsMemory();
        
    public Span<byte> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;

    public void Dispose()
    {
        var buffer = Interlocked.Exchange(ref _buffer, null);
        if (buffer is not null)
        {
            Pool.Return(buffer);
        }
        _bufferWriter.Dispose();
    }
}


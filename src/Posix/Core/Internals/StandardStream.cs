// reference: https://github.com/dotnet/runtime/blob/v10.0.3/src/libraries/System.Console/src/System/IO/ConsoleStream.cs

namespace Nemonuri.Posix.Internals;

internal abstract class StandardStream : Stream
{
    public static StandardStream Out {get;} = new OutStream();

    public static StandardStream Error {get;} = new ErrorStream();

    public static StandardStream In {get;} = new InStream();
    

    private Stream? _innerStream;

    private protected abstract Stream CreateNew();

    private Stream InnerStream
    {
        get
        {
            if (_innerStream is null)
            {
                var newStream = CreateNew();
                var innerStream0 = Interlocked.CompareExchange(ref _innerStream, newStream, null);
                if (innerStream0 is not null)
                {
                    newStream.Dispose();
                }
            }

            return _innerStream;
        }
    }

    public override void Write(byte[] buffer, int offset, int count) => InnerStream.Write(buffer, offset, count);

    public override void WriteByte(byte value) => InnerStream.WriteByte(value);

    public override int Read(byte[] buffer, int offset, int count) => InnerStream.Read(buffer, offset, count);

    public override int ReadByte() => InnerStream.ReadByte();

    protected override void Dispose(bool disposing)
    {
        var innerStream0 = Interlocked.Exchange(ref _innerStream, null);
        innerStream0?.Close();
    }

    public override bool CanRead => InnerStream.CanRead;

    public override bool CanWrite => InnerStream.CanWrite;

    public override bool CanSeek => InnerStream.CanSeek;

    public override long Length => InnerStream.Length;

    public override long Position
    {
        get => InnerStream.Position;
        set => InnerStream.Position = value;
    }

    public override void Flush() => InnerStream.Flush();

    public override void SetLength(long value) => InnerStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin) => InnerStream.Seek(offset, origin);

    internal class OutStream() : StandardStream
    {
        private protected override Stream CreateNew() => Console.OpenStandardOutput();
    }

    internal class ErrorStream() : StandardStream
    {
        private protected override Stream CreateNew() => Console.OpenStandardError();
    }

    internal class InStream() : StandardStream
    {
        private static bool _unsupportChecked = false;

        private protected override Stream CreateNew()
        {
            Stream? result = null;

            if (!_unsupportChecked)
            {
                try
                {
                    result = Console.OpenStandardInput();
                }
                catch
                {
                    _unsupportChecked = true;
                }
            }

            if (result is null) { result = Stream.Null; }

            return result;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        /* TODO: Impl 'Null' InStream. */
    }
}

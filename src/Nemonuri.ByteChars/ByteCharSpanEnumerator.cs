using System.Buffers;
using System.Collections;
namespace Nemonuri.ByteChars;

public ref struct ByteCharSpanEnumerator
#if NET9_0_OR_GREATER
    : IEnumerator<Rune>
#endif
{
    private readonly ReadOnlySpan<byte> _source;

    private int _offset;

    public Rune Current {get; private set;}

    public ByteCharSpanEnumerator(ReadOnlySpan<byte> source)
    {
        _source = source;
        _offset = 0;
        Current = default;
    }

    public readonly ByteCharSpanEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (!(_offset >= 0)) { return false; }

        OperationStatus opStatus = Rune.DecodeFromUtf8(_source.Slice(_offset), out Rune result, out int bytesCousumed);
        if (!(opStatus == OperationStatus.Done))
        {
            _offset = -1;
            Current = default;
        }

        _offset += bytesCousumed;
        Current = result;
        return true;
    }

#if NET9_0_OR_GREATER
    void IEnumerator.Reset() => throw new NotSupportedException();

    object IEnumerator.Current => Current;

    void IDisposable.Dispose() {}
#endif
}
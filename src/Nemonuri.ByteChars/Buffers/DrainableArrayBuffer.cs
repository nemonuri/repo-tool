using System.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;

namespace Nemonuri.Buffers;

public class DrainableArrayBuffer<T> : IBuffer<T>
{
/**
    'DrainableArrayBuffer' is boxed version of 'DrainableArrayBuilder'
*/

    private DrainableArrayBuilder<T> _builder;

    public DrainableArrayBuffer(int capacity = 0)
    {
        _builder = new(capacity);
    }

    public int WrittenCount => _builder.Count;

    public int Capacity => _builder.GetinternalArray() is { Length: var len } ? len : 0;

    public int FreeCapacity => Capacity - WrittenCount;

    public void Clear()
    {
        _builder.Clear();
    }

    public ReadOnlySpan<T> WrittenSpan => _builder.AsSpan();

    public ReadOnlyMemory<T> WrittenMemory => _builder.GetinternalArray() is { } ary ? new (ary, 0, WrittenCount) : ReadOnlyMemory<T>.Empty;

    public void Advance(int count)
    {
        Guard.IsBetweenOrEqualTo(count, 0, FreeCapacity);
        _builder.SetInternalCount(_builder.Count + count);
    }

    private T[] GetFreeCapacityEnsuredInternalArray(int sizeHint)
    {
        Guard.IsGreaterThanOrEqualTo(sizeHint, 0);
        _builder.EnsureCapacity(WrittenCount + Math.Max(sizeHint, 1));

        var ary = _builder.GetinternalArray();
        Debug.Assert( ary is not null );

        return ary!;
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        var ary = GetFreeCapacityEnsuredInternalArray(sizeHint);

        return new(ary, WrittenCount, sizeHint is 0 ? FreeCapacity : sizeHint);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        var ary = GetFreeCapacityEnsuredInternalArray(sizeHint);

        return new(ary, WrittenCount, sizeHint is 0 ? FreeCapacity : sizeHint);
    }

    public ArraySegment<T> DrainToArraySemgent() => _builder.DrainToArraySemgent();
}
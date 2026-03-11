using System.Buffers;
using System.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;

namespace Nemonuri.Buffers;

public readonly struct DrainableArrayBuffer<T> : IBuffer<T>
{
/**
    ## Q. What is diffrence of 'DrainableArrayBuilder' and 'DrainableArrayBuffer'?

    A. 'DrainableArrayBuffer' can reset 'Count'.
*/

    private readonly DrainableArrayBuilder<T> _builder;

    public DrainableArrayBuffer(int capacity)
    {
        _builder = new(capacity);
    }

    public readonly int WrittenCount => _builder.Count;

    public readonly int Capacity => _builder.GetinternalArray() is { Length: var len } ? len : 0;

    public readonly int FreeCapacity => Capacity - WrittenCount;

    public readonly void Clear()
    {
        _builder.AsSpan().Clear();
        _builder.SetInternalCount(0);
    }

    public readonly ReadOnlySpan<T> WrittenSpan => _builder.AsSpan();

    public readonly ReadOnlyMemory<T> WrittenMemory => _builder.GetinternalArray() is { } ary ? new (ary, 0, WrittenCount) : ReadOnlyMemory<T>.Empty;

    public void Advance(int count)
    {
        Guard.IsBetweenOrEqualTo(count, 0, FreeCapacity);
        _builder.SetInternalCount(_builder.Count + count);
    }

    private readonly T[] GetFreeCapacityEnsuredInternalArray(int sizeHint)
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
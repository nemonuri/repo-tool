namespace Nemonuri.Buffers;

/**
- Reference: https://github.com/dotnet/runtime/blob/v10.0.3/src/coreclr/tools/Common/System/Collections/Generic/ArrayBuilder.cs
- Reference: https://learn.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1.builder?view=net-10.0
*/

using System.Runtime.CompilerServices;
using Debug = System.Diagnostics.Debug;


public struct DrainableArrayBuilder<T> : IBufferWriter<T>
{
    private T[]? _items;
    private int _count;

    public DrainableArrayBuilder(int capacity)
    {
        _items = new T[capacity];
        _count = 0;
    }

    internal readonly T[]? GetinternalArray() => _items;

    internal void SetInternalCount(int count)
    {
        Debug.Assert( 0 <= count && count <= (_items ?? []).Length );
        _count = count;
    }


    private (T[]? Items, int Count) DrainAllFields()
    {
        T[]? items = Interlocked.Exchange(ref _items, null);
        int count = Interlocked.Exchange(ref _count, 0);
        return (items, count);
    }

    public T[] DrainToArray()
    {
        (var items, var count) = DrainAllFields();

        if (items == null)
        {
            return Array.Empty<T>();
        }
        else if (count != items.Length)
        {
            Array.Resize(ref items, count);
        }

        return items;
    }

    public ArraySegment<T> DrainToArraySemgent()
    {
        (var items, var count) = DrainAllFields();

        if (items == null)
        {
            return new();
        }
        else if (count > items.Length)
        {
            Array.Resize(ref items, count);
        }

        return new(items, 0, count);
    }

    public void CopyTo(T[] destination)
    {
        if (_items != null)
        {
            // Use Array.Copy instead of Span.CopyTo to handle covariant destination
            Array.Copy(_items, destination, _count);
        }
    }

    public void Add(T item)
    {
        if (_items == null || _count == _items.Length)
            Array.Resize(ref _items, 2 * _count + 1);
        _items[_count++] = item;
    }

    public readonly Span<T> AsSpan() => _items.AsSpan(0, _count);

#if false
    public readonly Span<T> AsSpan(int start) => _items.AsSpan(start, _count - start);

    public Span<T> AppendSpan(int length)
    {
        int origCount = _count;
        EnsureCapacity(origCount + length);

        _count = origCount + length;
        return _items.AsSpan(origCount, length);
    }
#endif


    public void AddRange(T[] items)
    {
        AddRange(items, 0, items.Length);
    }

#nullable disable
    public void AddRange(T[] items, int offset, int length)
    {
        if (length == 0)
            return;

        Debug.Assert(length > 0);
        Debug.Assert(items.Length >= offset + length);

        EnsureCapacity(_count + length);
        Array.Copy(items, offset, _items, _count, length);
        _count += length;
    }

    public void AddRange(ArraySegment<T> items) => AddRange(items.Array, items.Offset, items.Count);


    public void AddRange(ReadOnlySpan<T> items)
    {
        var length = items.Length;
        if (length == 0)
            return;

        EnsureCapacity(_count + length);
        items.CopyTo(_items.AsSpan().Slice(_count));
        _count += length;
    }

    public void AddRange(DrainableArrayBuilder<T> items)
    {
        if (items.Count == 0)
            return;
        EnsureCapacity(_count + items.Count);
        Array.Copy(items._items, 0, _items, _count, items.Count);
        _count += items.Count;
    }

    private void ZeroExtend(int numItems)
    {
        Debug.Assert(numItems >= 0);
        EnsureCapacity(_count + numItems);
        _count += numItems;
    }

    public void EnsureCapacity(int requestedCapacity)
    {
        if (requestedCapacity > ((_items != null) ? _items.Length : 0))
        {
            Grow(requestedCapacity);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int requestedCapacity)
    {
        int newCount = Math.Max(2 * _count + 1, requestedCapacity);
        Array.Resize(ref _items, newCount);
    }

    public readonly int Count => _count;

    public readonly T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public readonly bool Contains(T t)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_items[i].Equals(t))
            {
                return true;
            }
        }

        return false;
    }

    public readonly bool Any(Func<T, bool> func)
    {
        for (int i = 0; i < _count; i++)
        {
            if (func(_items[i]))
            {
                return true;
            }
        }

        return false;
    }

    void IBufferWriter<T>.Advance(int count)
    {
        ZeroExtend(count);
    }

    Memory<T> IBufferWriter<T>.GetMemory(int sizeHint)
    {
        Guard.IsGreaterThanOrEqualTo(sizeHint, 0);
        EnsureCapacity(Count + Math.Max(sizeHint, 1));
        return new(_items, Count, sizeHint is 0 ? (_items.Length - Count) : sizeHint);
    }

    Span<T> IBufferWriter<T>.GetSpan(int sizeHint)
    {
        Guard.IsGreaterThanOrEqualTo(sizeHint, 0);
        EnsureCapacity(Count + Math.Max(sizeHint, 1));
        return new(_items, Count, sizeHint is 0 ? (_items.Length - Count) : sizeHint);
    }
#nullable restore

    public void Clear()
    {
        AsSpan().Clear();
        SetInternalCount(0);
    }
}

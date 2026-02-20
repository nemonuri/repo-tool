namespace Nemonuri.ByteChars;

/**
- Reference: https://github.com/dotnet/runtime/blob/v10.0.3/src/coreclr/tools/Common/System/Collections/Generic/ArrayBuilder.cs
*/

using System.Runtime.CompilerServices;
using Debug = System.Diagnostics.Debug;


public struct ArrayBuilder<T>
{
    private T[]? _items;
    private int _count;

    public ArrayBuilder(int capacity)
    {
        _items = new T[capacity];
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

    public readonly Span<T> AsSpan(int start) => _items.AsSpan(start, _count - start);

    public Span<T> AppendSpan(int length)
    {
        int origCount = _count;
        EnsureCapacity(origCount + length);

        _count = origCount + length;
        return _items.AsSpan(origCount, length);
    }

    public void Append(T[] newItems)
    {
        Append(newItems, 0, newItems.Length);
    }

#nullable disable
    public void Append(T[] newItems, int offset, int length)
    {
        if (length == 0)
            return;

        Debug.Assert(length > 0);
        Debug.Assert(newItems.Length >= offset + length);

        EnsureCapacity(_count + length);
        Array.Copy(newItems, offset, _items, _count, length);
        _count += length;
    }

    public void Append(ReadOnlySpan<T> newItems)
    {
        var length = newItems.Length;
        if (length == 0)
            return;

        EnsureCapacity(_count + length);
        newItems.CopyTo(_items.AsSpan().Slice(_count));
        _count += length;
    }

    public void Append(ArrayBuilder<T> newItems)
    {
        if (newItems.Count == 0)
            return;
        EnsureCapacity(_count + newItems.Count);
        Array.Copy(newItems._items, 0, _items, _count, newItems.Count);
        _count += newItems.Count;
    }

    public void ZeroExtend(int numItems)
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
#nullable restore
}

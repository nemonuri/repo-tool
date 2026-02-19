using System.Collections;

namespace Nemonuri.ByteChars;

public readonly struct ImmutableArraySegment<T> : IReadOnlyList<T>
{
/**
- Reference: https://github.com/dotnet/runtime/blob/v10.0.3/src/libraries/System.Private.CoreLib/src/System/ArraySegment.cs
*/

    private readonly ImmutableArray<T> _array;
    private readonly int _offset;
    private readonly int _count;

    public ImmutableArraySegment(ImmutableArray<T> array, int offset, int count)
    {
        if ((uint)offset > (uint)array.Length || (uint)count > (uint)(array.Length - offset))
        {
            if (offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} or {1} is less than 0.", nameof(offset), nameof(count)));
            }
            else
            {
                throw new ArgumentException(string.Format("{0} and {1} do not specify a valid range in {2}.", offset, count, array));
            }
        }
        _array = array;
        _offset = offset;
        _count = count;
    }

    public ImmutableArraySegment(ImmutableArray<T> array)
    {
        _array = array;
        _offset = 0;
        _count = array.Length;
    }

    private void GuardIndexRange(int index)
    {
        Guard.IsLessThan((uint)index, (uint)_count);
    }

    public T this[int index]
    {
        get
        {
            GuardIndexRange(index);
            return _array[_offset + index];
        }
    }

    public ImmutableArray<T> Array => _array;

    public int Offset => _offset;

    public int Count => _count;

    public ref readonly T ItemRef(int index)
    {
        GuardIndexRange(index);
        return ref _array.ItemRef(_offset + index);
    }

    public ReadOnlySpan<T> AsSpan() => _array.AsSpan().Slice(_offset, _count);

    public Span<T> UnsafeAsSpan()
    {
#if NETSTANDARD2_1_OR_GREATER
        ref var offsetRef = ref Unsafe.AsRef(in _array.ItemRef(_offset));
        return MemoryMarshal.CreateSpan(ref offsetRef, _count);
#else
        var layout = Unsafe.As<ImmutableArray<T>, ImmutableArrayLikeLayout<T>>(ref Unsafe.AsRef(in _array));
        return layout.array is { } a ? a.AsSpan().Slice(_offset, _count) : [];
#endif
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
        private readonly ImmutableArraySegment<T> _source;
        private int _index;

        internal Enumerator(ImmutableArraySegment<T> source)
        {
            _source = source;
            _index = -1;
        }

        public readonly T Current => _source[_index];

        public bool MoveNext()
        {
            _index += 1;
            return _index < _source.Count;
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        readonly object? IEnumerator.Current => Current;

        void IDisposable.Dispose()
        {
        }
    }
}

#if !NETSTANDARD2_1_OR_GREATER
internal struct ImmutableArrayLikeLayout<T>
{
    internal readonly T[]? array;
}
#endif
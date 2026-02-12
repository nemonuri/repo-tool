namespace Nemonuri.ByteChars;

public unsafe interface IUnsafePinnedSpanPointer<T> where T : unmanaged
{
    public T* PinnedPointer {get;}

    public int SpanLength {get;}
}

public static unsafe class UnsafePinnedSpanPointerTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> LoadSpan<T>(T* pinnedPointer, int spanLength)
        where T : unmanaged
    {
        return new Span<T>(pinnedPointer, spanLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> LoadSpan<T>(IUnsafePinnedSpanPointer<T> pointer)
        where T : unmanaged
    {
        return LoadSpan(pointer.PinnedPointer, pointer.SpanLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> LoadReadOnlySpan<T>(T* pinnedPointer, int spanLength)
        where T : unmanaged
    {
        return new ReadOnlySpan<T>(pinnedPointer, spanLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> LoadReadOnlySpan<T>(IUnsafePinnedSpanPointer<T> pointer)
        where T : unmanaged
    {
        return LoadReadOnlySpan(pointer.PinnedPointer, pointer.SpanLength);
    }
}

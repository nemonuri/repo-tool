namespace Nemonuri.OCamlDotNet;

public unsafe readonly struct UnsafePinnedSpanPointer<T> : IUnsafePinnedSpanPointer<T>
    where T : unmanaged
{
    public T* PinnedPointer {get;}

    public int SpanLength {get;}

    public UnsafePinnedSpanPointer(T* pinnedPointer, int spanLength)
    {
        PinnedPointer = pinnedPointer;
        SpanLength = spanLength;
    }
}

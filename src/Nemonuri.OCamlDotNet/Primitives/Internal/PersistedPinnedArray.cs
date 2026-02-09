namespace Nemonuri.OCamlDotNet.Internal;

internal class PersistedPinnedArray<T>
{
    public T[] Values {get;}

#if !NET8_0_OR_GREATER
    private readonly GCHandle _gcHandle;
#endif

    public PersistedPinnedArray(int length)
    {
        Guard.IsGreaterThan(length, 0);
#if NET8_0_OR_GREATER   
        Values = GC.AllocateArray<T>(length, pinned: true);
#else
        Values = new T[length];
        _gcHandle = GCHandle.Alloc(Values, GCHandleType.Pinned);
#endif
    }
}

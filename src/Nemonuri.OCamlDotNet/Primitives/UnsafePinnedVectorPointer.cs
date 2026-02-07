namespace Nemonuri.OCamlDotNet;

using System.Numerics;
using static Extensions.UnsafePinnedSpanPointerExtensions;

/// <summary>
/// Assume pointer is pinned.
/// </summary>
public unsafe readonly struct UnsafePinnedVectorPointer<T> : IUnsafePinnedSpanPointer<T>
    where T : unmanaged
{
/**
## System.Numerics.Vector supporting type table

- https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.-ctor?view=netstandard-2.1#remarks
*/
    /* netstandard2.1 에도, 이게 정의되어 있지 않다고? 아니 문서에는 있으면서? */
    // public static bool IsSupported => Vector<T>.IsSupported;

#if !NET8_0_OR_GREATER
    [ThreadStatic]
    private static T[]? s_tempStorage;

    private static T[] TempStorage => s_tempStorage ??= /*!IsSupported ? [] :*/ new T[Vector<T>.Count];
#endif
    
    private readonly T* _pinnedPointer;

    public UnsafePinnedVectorPointer(T* pinnedPointer)
    {
        _pinnedPointer = pinnedPointer;
    }

    public bool IsAnyMemberNull => _pinnedPointer == null;

    public T* PinnedPointer => _pinnedPointer;

    public int SpanLength => Vector<T>.Count;

    public Vector<T> LoadVector()
    {
#if NET8_0_OR_GREATER
        return Vector.Load(_pinnedPointer);
#elif NETSTANDARD2_1_OR_GREATER
        return new Vector<T>(LoadSpan());
#else
        Span<T> loadedSpan = this.LoadSpan();
        loadedSpan.CopyTo(TempStorage);
        return new Vector<T>(TempStorage);
#endif
    }

    public void StoreVector(Vector<T> source)
    {
#if NET8_0_OR_GREATER
        Vector.Store(source, _pinnedPointer);
#else
        source.CopyTo(TempStorage);
        var dest = this.LoadSpan();
        TempStorage.CopyTo(dest);
#endif
    }
}

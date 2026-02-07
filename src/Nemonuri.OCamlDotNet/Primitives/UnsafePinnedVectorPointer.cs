namespace Nemonuri.OCamlDotNet;

using System.Numerics;

/// <summary>
/// Assume pointer is pinned.
/// </summary>
public unsafe readonly struct UnsafePinnedVectorPointer<T> where T : unmanaged
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
    
    private readonly T* _pointer;

    public UnsafePinnedVectorPointer(T* pointer)
    {
        _pointer = pointer;
    }

    public bool IsAnyMemberNull => _pointer == null;

    public T* Pointer => _pointer;

    public int SpanLength => Vector<T>.Count;

    public Vector<T> LoadVector()
    {
#if NET8_0_OR_GREATER
        return Vector.Load(_pointer);
#else
        return new Vector<T>(new Span<T>(Pointer, SpanLength));
#endif
    }

    public void StoreVector(Vector<T> source)
    {
#if NET8_0_OR_GREATER
        Vector.Store(source, _pointer);
#else
        source.CopyTo(TempStorage);
        var dest = new Span<T>(Pointer, SpanLength);
        TempStorage.CopyTo(dest);
#endif
    }
}

namespace Nemonuri.ByteChars.Internal;

internal static class ByteVectorTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector<byte> LoadVector(ReadOnlySpan<byte> chunk)
    {
#if NET8_0_OR_GREATER
        return Vector.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#else
        return Nemonuri.NetStandards.Numerics.VectorTheory.LoadUnsafe(in MemoryMarshal.GetReference(chunk));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void StoreVector(Span<byte> chunk, Vector<byte> vector)
    {
#if NET8_0_OR_GREATER
        vector.StoreUnsafe(ref MemoryMarshal.GetReference(chunk));
#else
        Nemonuri.NetStandards.Numerics.VectorTheory.StoreUnsafe(vector, ref MemoryMarshal.GetReference(chunk));
#endif
    }
}

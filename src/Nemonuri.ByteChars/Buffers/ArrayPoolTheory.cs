using System.Diagnostics;
using CommunityToolkit.HighPerformance;
using static Nemonuri.ByteChars.Internal.InternalConstants;

namespace Nemonuri.Buffers;

public static class ArrayPoolTheory
{
    public static void ReturnAndNullToShared<T>([MaybeNull] ref T[]? rented)
    {
        if (rented is null) { return; }

        ArrayPool<T>.Shared.Return(rented);
        rented = null;
    }

    public unsafe static int GetEnsuredDefaultRentLength<T>(int defaultRentLength)
        where T : unmanaged
        => 
        defaultRentLength > 0 ? defaultRentLength : Math.Max(StackAllocThreshold / sizeof(T), 1);
        

    public static int GetEnsuredRentLength<T>(int sizeHint, int defaultRentLength) where T : unmanaged
    {
        Guard.IsGreaterThanOrEqualTo(sizeHint, 0);
        return sizeHint > 0 ? sizeHint : GetEnsuredDefaultRentLength<T>(defaultRentLength);
    }

    private static ArraySegment<T> ResizeAndSliceCore<T>(ArrayPool<T>? pool, ref T[]? buffer, int positiveLength)
    {
        // Debug.Assert, not Guard.
        Debug.Assert( positiveLength > 0 );

        (pool ?? ArrayPool<T>.Shared).Resize(ref buffer, positiveLength);
        return new(buffer, 0, positiveLength);
    }

    public static ArraySegment<T> ResizeAndSlice<T>(ArrayPool<T>? pool, ref T[]? buffer, int sizeHint, int defaultRentLength) where T : unmanaged
    {
        return ResizeAndSliceCore(pool, ref buffer, GetEnsuredRentLength<T>(sizeHint, defaultRentLength));
    }
}
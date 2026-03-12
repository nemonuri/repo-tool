namespace Nemonuri.Buffers;

public static class ArrayPoolTheory
{
    public static void ReturnAndNullToShared<T>([MaybeNull] ref T[]? rented)
    {
        if (rented is null) { return; }

        ArrayPool<T>.Shared.Return(rented);
        rented = null;
    }
}
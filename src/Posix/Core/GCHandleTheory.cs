namespace Nemonuri.Posix;

public static class GCHandleTheory
{
    public static void FreeIfAllocated(GCHandle handle)
    {
        if (handle.IsAllocated) { handle.Free(); }
    }

    public static bool TryGetObject<T>(GCHandle handle, [NotNullWhen(true)] out T? result) where T : class
    {
        if (handle.IsAllocated && handle.Target is T t)
        {
            result = t;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }
}
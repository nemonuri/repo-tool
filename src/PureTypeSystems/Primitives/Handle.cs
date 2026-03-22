namespace Nemonuri.PureTypeSystems.Primitives;

public interface IHandle
{
    nint ToIntPtr();
}

public static class HandleTheory
{
    public static bool CheckHasValue(IHandle? handle)
    {
        if (handle is null) { return false; }
        return handle.ToIntPtr() != 0;
    }

    public static bool Equals(IHandle? left, IHandle? right)
    {
        if (left == null || right == null) { return false; }
        return left.ToIntPtr() == right.ToIntPtr();
    }

    public static int GetHashCode(IHandle? handle)
    {
        if (handle == null) { return 0; }
        return handle.ToIntPtr().GetHashCode();
    }
}

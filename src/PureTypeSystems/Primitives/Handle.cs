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
}
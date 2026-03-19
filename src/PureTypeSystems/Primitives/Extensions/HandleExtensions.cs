namespace Nemonuri.PureTypeSystems.Primitives.Extensions;


public static class HandleExtensions
{
    extension(IHandle handle)
    {
        public bool HasValue => HandleTheory.CheckHasValue(handle);
    }
}

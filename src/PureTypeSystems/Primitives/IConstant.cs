namespace Nemonuri.PureTypeSystems.Primitives;

public interface IConstant<T>
{
    T Value {get;}
}

#if false
public unsafe readonly struct ConstantHandle<T> : IHandle
{
    private readonly delegate*<T> _fp;

    internal ConstantHandle(delegate*<T> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;
}
#endif

public static class ConstantTheory
{
    extension<T, TConstant>(TConstant)
        where TConstant : IConstant<T>
    {
        public static T GetValue() => System.Activator.CreateInstance<TConstant>().Value;
    }
}
using Hth = Nemonuri.PureTypeSystems.Primitives.HandleTheory;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IConstant<T>
{
    T Value {get;}
}


public unsafe readonly struct ConstantHandle<T> : IHandle, IEquatable<ConstantHandle<T>>
{
    private readonly delegate*<T> _fp;

    internal ConstantHandle(delegate*<T> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public bool Equals(ConstantHandle<T> other) => Hth.Equals(this, other);

    public override bool Equals(object? obj) => obj is ConstantHandle<T> o && Equals(o);

    public override int GetHashCode() => Hth.GetHashCode(this);
}


public static class ConstantTheory
{
    extension<T, TConstant>(TConstant)
        where TConstant : IConstant<T>
    {
        public static T GetValue() => System.Activator.CreateInstance<TConstant>().Value;
    }
}
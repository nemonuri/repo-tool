using System.Diagnostics;
using Hth = Nemonuri.PureTypeSystems.Primitives.HandleTheory;
using Vth = Nemonuri.PureTypeSystems.Primitives.ValueUnitTheory;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IConstant<T> : IArrowPremise<ValueUnit, T>
{
    // T Value {get;}
}


public readonly struct ConstantHandle<T> : IHandle, IEquatable<ConstantHandle<T>>
{
    private readonly ArrowHandle<ValueUnit, T> _arrowHandle;

    internal ConstantHandle(ArrowHandle<ValueUnit, T> arrowHandle)
    {
        Debug.Assert( arrowHandle.PreJudge.IsTautology );
        Debug.Assert( arrowHandle.PostJudge.IsTautology );
        _arrowHandle = arrowHandle;
    }

    public nint ToIntPtr() => _arrowHandle.ToIntPtr();

    public bool Equals(ConstantHandle<T> other) => Hth.Equals(this, other);

    public override bool Equals(object? obj) => obj is ConstantHandle<T> o && Equals(o);

    public override int GetHashCode() => Hth.GetHashCode(this);

    public ArrowHandle<ValueUnit, T> ArrowHandle => _arrowHandle;

    public T Value => ArrowHandle.Apply(Vth.Singleton);
}


public static class ConstantTheory
{
    extension<T, TConstant>(TConstant)
        where TConstant : IArrowPremise<ValueUnit, T>
    {
        public static ConstantHandle<T> ToHandle() => new(ArrowTheory.ToHandle<ValueUnit, T, TConstant>());

        public static T GetValue() => (ToHandle<T, TConstant>()).Value;
    }
}

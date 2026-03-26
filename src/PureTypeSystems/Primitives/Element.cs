using Hth = Nemonuri.PureTypeSystems.Primitives.HandleTheory;
using Nemonuri.PureTypeSystems.Primitives.Expressions;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IElement<T> : IArrowPremise<ValueUnit, T>
{
    // T Value {get;}
}


public readonly struct ElementHandle<T> : IHandle, IEquatable<ElementHandle<T>>
{
    private readonly ArrowHandle<ValueUnit, T> _arrowHandle;

    internal ElementHandle(ArrowHandle<ValueUnit, T> arrowHandle)
    {
        _arrowHandle = arrowHandle;
    }

    public nint ToIntPtr() => _arrowHandle.ToIntPtr();

    public bool Equals(ElementHandle<T> other) => Hth.Equals(this, other);

    public override bool Equals(object? obj) => obj is ElementHandle<T> o && Equals(o);

    public override int GetHashCode() => Hth.GetHashCode(this);

    public ArrowHandle<ValueUnit, T> ArrowHandle => _arrowHandle;

    public T Value => ArrowHandle.Apply(DataLevelTheory.ValueUnit);
}


public static class ElementTheory
{
    extension<T, TElement>(TElement)
        where TElement : IArrowPremise<ValueUnit, T>
    {
        public static ElementHandle<T> ToHandle() => new(ArrowTheory.ToHandle<ValueUnit, T, TElement>());

        public static T GetValue() => (ToHandle<T, TElement>()).Value;
    }
}

namespace Nemonuri.PureTypeSystems.Primitives;

/**
    - a kind is the type of a type constructor
    - reference: https://en.wikipedia.org/wiki/Kind_(type_theory)
*/

public interface IKindPremise
{
    T ToDotNet<T>(T source);
}

public interface IKindPremise<TSource, TTarget>
{
    TTarget ToDotNet(TSource source);
}

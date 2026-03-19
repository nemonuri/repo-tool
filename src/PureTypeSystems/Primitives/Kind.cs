namespace Nemonuri.PureTypeSystems.Primitives;

public interface IKindPremise
{
    T ToDotNet<T>(T source);
}

public interface IKindPremise<TSource, TTarget>
{
    TTarget ToDotNet(TSource source);
}

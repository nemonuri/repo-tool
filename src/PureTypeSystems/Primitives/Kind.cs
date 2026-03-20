using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;
using Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

namespace Nemonuri.PureTypeSystems.Primitives;

/**
    - a kind is the type of a type constructor
    - reference: https://en.wikipedia.org/wiki/Kind_(type_theory)
*/

public interface IKindPremise<TKindLabel, TSource, TTarget>
    where TKindLabel : IKindLabel<TKindLabel>
{
    TTarget Construct(in TSource source);

    TSource Deconstruct(in TTarget target);
}


public interface IKindLabel<TKindLabel>
    where TKindLabel : IKindLabel<TKindLabel>
{
    ImplyHandle<TKindLabel, KindHandle<TKindLabel, TSource, TTarget>> ToImplyHandle<TSource, TTarget>();
}


public unsafe readonly struct KindHandle<TKindLabel, TSource, TTarget>
{
    //private readonly delegate*<in TSource, TTarget> _
}


public readonly struct IdentityKind : IKindLabel<IdentityKind>
{
    public ImplyHandle<IdentityKind, KindHandle<IdentityKind, TSource, TTarget>> ToImplyHandle<TSource, TTarget>()
    {
        throw new NotImplementedException();
    }
}

public readonly struct IdentityKind<T> : IKindPremise<IdentityKind, T, T>
{
    public T Construct(in T source) => source;

    public T Deconstruct(in T target) => target;
}

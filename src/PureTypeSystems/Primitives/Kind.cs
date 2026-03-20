using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.Extensions;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;
using Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

namespace Nemonuri.PureTypeSystems.Primitives;

/**
    - a kind is the type of a type constructor
    - reference: https://en.wikipedia.org/wiki/Kind_(type_theory)
*/

#if false
public interface IKindPremise<TKindLabel, TSource, TTarget>
    where TKindLabel : IKindLabelPremise<TKindLabel>
{
    TTarget Construct(in TSource source);

    TSource Deconstruct(in TTarget target);
}
#endif

public interface IKindLabelPremise<TKindLabel>
    where TKindLabel : IKindLabelPremise<TKindLabel>
{
    bool TryToKind<TSource, TTarget>(out ArrowHandlePair<TSource, TTarget> handlePair);
}

#if false
public readonly struct KindBasedArrowPair<TKindLabel, TSource, TTarget, TKind> : IArrowPairPremise<TSource, TTarget>
    where TKindLabel : IKindLabelPremise<TKindLabel>
    where TKind : unmanaged, IKindPremise<TKindLabel, TSource, TTarget>
{
    public static TTarget Apply(in TSource source) => (new TKind()).Construct(in source);

    public static TSource ContraApply(in TTarget source) => (new TKind()).Deconstruct(in source);

    TSource IArrowPairPremise<TSource, TTarget>.ContraApply(in TTarget post) => ContraApply(in post);

    TTarget IArrowPremise<TSource, TTarget>.Apply(in TSource pre) => Apply(in pre);
}


public readonly struct KindHandlePair<TKindLabel, TSource, TTarget>
{
    private readonly ArrowHandlePair<TSource, TTarget> _handlePair;

    internal KindHandlePair(ArrowHandlePair<TSource, TTarget> handlePair)
    {
        _handlePair = handlePair;
    }

    public TTarget Construct(in TSource source) => _handlePair.Handle.Apply(in source);

    public TSource Deconstruct(in TTarget target) => _handlePair.ContraHandle.Apply(in target);

    public bool TryConstruct(in TSource source, [NotNullWhen(true)] out TTarget? target) =>  _handlePair.Handle.TryApply(in source, out target, out _);

    public bool TryDeconstruct(in TTarget source, [NotNullWhen(true)] out TSource? target) =>  _handlePair.ContraHandle.TryApply(in source, out target, out _);
}
#endif

public static class KindLabelTheory
{
    extension<TKindLabel>(TKindLabel)
        where TKindLabel : unmanaged, IKindLabelPremise<TKindLabel>
    {
        public static ArrowHandlePair<TSource, TTarget> ToKind<TSource, TTarget>()
        {
            if ((new TKindLabel()).TryToKind<TSource, TTarget>(out var hp))
            {
                return hp;
            }
            else
            {
                return ArrowPairTheory.GetFailureHandlePair<TSource, TTarget>();
            }
        }
    }

}

public readonly struct IdentityKind : IKindLabelPremise<IdentityKind>
{
    public static ArrowHandlePair<T,T> ToKind<T>() => KindLabelTheory.ToKind<IdentityKind,T,T>();

    public bool TryToKind<TSource, TTarget>(out ArrowHandlePair<TSource, TTarget> kindHandlePair)
    {
        if (typeof(TSource) != typeof(TTarget)) 
        { 
            kindHandlePair = default;
            return false; 
        }
        else
        {
            ArrowHandlePair<TSource, TSource> hp = ArrowPairTheory.ToHandlePair<TSource, TSource, IdentityPair<TSource>>();
            kindHandlePair = Unsafe.As<ArrowHandlePair<TSource, TSource>, ArrowHandlePair<TSource, TTarget>>(ref hp);
            return true;
        }
    }
}

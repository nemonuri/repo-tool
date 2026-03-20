using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

/**
    - a kind is the type of a type constructor
    - reference: https://en.wikipedia.org/wiki/Kind_(type_theory)
*/



public interface IKindPremise<TKind>
    where TKind : IKindPremise<TKind>
{
    bool TryToPair<TSource, TTarget>(out ArrowHandlePair<TSource, TTarget> handlePair);
}

public static class KindTheory
{
    extension<TKind>(TKind)
        where TKind : unmanaged, IKindPremise<TKind>
    {
        public static ArrowHandlePair<TSource, TTarget> ToPair<TSource, TTarget>()
        {
            if ((new TKind()).TryToPair<TSource, TTarget>(out var hp))
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

public readonly struct IdentityKind : IKindPremise<IdentityKind>
{
    public static ArrowHandlePair<T,T> ToPair<T>() => KindTheory.ToPair<IdentityKind,T,T>();

    public bool TryToPair<TSource, TTarget>(out ArrowHandlePair<TSource, TTarget> kindHandlePair)
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

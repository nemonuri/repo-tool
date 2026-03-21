using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.Extensions;

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

        public static TTarget Cons<TSource, TTarget>(in TSource source)
        {
            return KindTheory.ToPair<TKind, TSource, TTarget>().Handle.Apply(in source);
        }

        public static TSource Decons<TSource, TTarget>(in TTarget target)
        {
            return KindTheory.ToPair<TKind, TSource, TTarget>().ContraHandle.Apply(in target);
        }
    }
}

public readonly struct IdentityKind : IKindPremise<IdentityKind>
{
    public static T Cons<T>(T p) => KindTheory.Cons<IdentityKind,T,T>(in p);
    public static T Decons<T>(T q) => KindTheory.Decons<IdentityKind,T,T>(in q);

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

public readonly struct StrictGuardKind<TJudge> : IKindPremise<StrictGuardKind<TJudge>>
    where TJudge : unmanaged, IJudgePremise
{
    public static Refined<T, TJudge> Cons<T>(T p) => KindTheory.Cons<StrictGuardKind<TJudge>,T,Refined<T, TJudge>>(in p);

    public static T Decons<T>(Refined<T, TJudge> q) => KindTheory.Decons<StrictGuardKind<TJudge>,T,Refined<T, TJudge>>(in q);

    private readonly struct KindImpl<T> : IArrowPairPremise<T, Refined<T, TJudge>>
    {
        public T ContraApply(in Refined<T, TJudge> post) => post.Value;

        public Refined<T, TJudge> Apply(in T pre)
        {
            if ((new TJudge()).Judge(in pre).IsTrue) 
            { 
                return new(pre); 
            }
            else
            {
                throw new ArgumentException(/* TODO */);
            }
        }
    }

    public bool TryToPair<TP, TQ>(out ArrowHandlePair<TP, TQ> handlePair) => ArrowPairTheory.TryToTypeEqualHandlePair<TP, Refined<TP, TJudge>, KindImpl<TP>, TP, TQ>(out handlePair);
}

public readonly struct LooseGuardKind<TJudge> : IKindPremise<LooseGuardKind<TJudge>>
    where TJudge : unmanaged, IJudgePremise
{
    public static Refined<T, TJudge> Cons<T>(T p) => KindTheory.Cons<LooseGuardKind<TJudge>,T,Refined<T, TJudge>>(in p);

    public static T Decons<T>(Refined<T, TJudge> q) => KindTheory.Decons<LooseGuardKind<TJudge>,T,Refined<T, TJudge>>(in q);

    private readonly struct KindImpl<T> : IArrowPairPremise<T, Refined<T, TJudge>>
    {
        public T ContraApply(in Refined<T, TJudge> post) => post.Value;

        public Refined<T, TJudge> Apply(in T pre)
        {
            if ((new TJudge()).Judge(in pre).IsTrueOrThunk) 
            { 
                return new(pre); 
            }
            else
            {
                throw new ArgumentException(/* TODO */);
            }
        }
    }

    public bool TryToPair<TP, TQ>(out ArrowHandlePair<TP, TQ> handlePair) => ArrowPairTheory.TryToTypeEqualHandlePair<TP, Refined<TP, TJudge>, KindImpl<TP>, TP, TQ>(out handlePair);
}

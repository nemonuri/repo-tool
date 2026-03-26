using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.Extensions;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;

/**
    - a kind is the type of a type constructor
    - reference: https://en.wikipedia.org/wiki/Kind_(type_theory)
*/


public interface IKindPremise<TKind> where TKind : IKindPremise<TKind>
{
    bool TryToCons<TP, TQ>(out ArrowHandle<TP, TQ> handle);

    bool TryToDecons<TQ, TP>(out ArrowHandle<TQ, TP> handle);
}


public static class KindTheory
{
    extension<TKind>(TKind)
        where TKind : IKindPremise<TKind>
    {
        private static TKind GetInstance() => RealizerTheory.Realize<TKind>();

        public static ArrowHandle<TP, TQ> ToCons<TP, TQ>()
        {
            if (GetInstance<TKind>().TryToCons<TP, TQ>(out var hp))
            {
                return hp;
            }
            else
            {
                return ArrowTheory.GetFailureHandle<TP, TQ>();
            }
        }

        public static TQ Cons<TP, TQ>(in TP p)
        {
            return ToCons<TKind, TP, TQ>().Apply(in p);
        }

        public static ArrowHandle<TQ, TP> ToDecons<TQ, TP>()
        {
            if (GetInstance<TKind>().TryToDecons<TQ, TP>(out var hp))
            {
                return hp;
            }
            else
            {
                return ArrowTheory.GetFailureHandle<TQ, TP>();
            }
        }

        public static TP Decons<TQ, TP>(in TQ q)
        {
            return ToDecons<TKind, TQ, TP>().Apply(in q);
        }

#if false
        public static TSource Decons<TSource, TTarget>(in TTarget target)
        {
            return KindTheory.ToPair<TKind, TSource, TTarget>().ContraHandle.Apply(in target);
        }
#endif
    }

    
}

#if false
public readonly struct IdentityKind : IKindPremise<IdentityKind>
{
    public static T Cons<T>(T p) => KindTheory.Cons<IdentityKind,T,T>(in p);

    public bool TryToCons<TP, TQ>(out ArrowHandle<TP, TQ> handle)
    {
        handle = ArrowTheory.GetIdentityHandle<TP, TQ>();
        return true;
    }
}
#endif

#if false
public readonly struct ArrowBasedKind<TP, TQ, TArrow> : IKindPremise<ArrowBasedKind<TP, TQ, TArrow>>//, IConstant<ArrowHandle<TP, TQ>>
    where TArrow : unmanaged, IArrowPremise<TP, TQ>
{
    public static TQ Cons(TP tp) => KindTheory.Cons<ArrowBasedKind<TP, TQ, TArrow>, TP, TQ>(in tp);

    public bool TryToCons<TP2, TQ2>(out ArrowHandle<TP2, TQ2> handle)
    {
        return ArrowTheory.TryToTypeEqualHandle<TP, TQ, TArrow, TP2, TQ2>(out handle);
    }

    //ArrowHandle<TP, TQ> IConstant<ArrowHandle<TP, TQ>>.Value => ArrowTheory.ToHandle<TP, TQ, TArrow>();
}

public readonly struct JudgeBasedKind<T, TJudge> : IKindPremise<JudgeBasedKind<T, TJudge>>
    where TJudge : IJudgePremise
{
    public static Refined<T, TJudge> Cons(T p) => KindTheory.Cons<JudgeBasedKind<T, TJudge>, T, Refined<T, TJudge>>(in p);

    public bool TryToCons<TP, TQ>(out ArrowHandle<TP, TQ> handle)
    {
        return ArrowTheory.TryToTypeEqualHandle<T, Refined<T, TJudge>, JudgeBasedArrow<T, TJudge>, TP, TQ>(out handle);
    }
}



public readonly struct StrictGuardKind<TJudge> : IKindPremise<StrictGuardKind<TJudge>>
    where TJudge : unmanaged, IJudgePremise
{
    public static Refined<T, TJudge> Cons<T>(T p) => KindTheory.Cons<StrictGuardKind<TJudge>,T,Refined<T, TJudge>>(in p);

    private readonly struct KindImpl<T> : IArrowPremise<T, Refined<T, TJudge>>
    {
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

    public bool TryToCons<TP, TQ>(out ArrowHandle<TP, TQ> handle) => ArrowTheory.TryToTypeEqualHandle<TP, TQ>()
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
#endif

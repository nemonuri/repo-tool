using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;


namespace Nemonuri.PureTypeSystems.Primitives;

#if false
public readonly record struct ApplyJudgement(Judgement Pre, Judgement? Post)
{
}
#endif


public static class ArrowTheory
{
    public const nint FailurePointer = 0;

    //public static ArrowHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;

    public static ArrowHandle<TP, TQ> GetFailureHandle<TP, TQ>() => default; //ToHandle<TP, Tautology, TQ, Negation, Failure<TP, TQ>>();
        
#if false
    public static bool TryApplyTrue<TP, TQ>
    (
        in ArrowHandle<TP, TQ> implyHandle, 
        in TP? pre, 
        [NotNullWhen(true)] out TQ? post,
        out ApplyJudgement applyJudgement
    )
    {
        if (!JudgeTheory.TryJudgeTrue(implyHandle.PreJudge, in pre, out var preJudgement))
        {
            post = default;
            applyJudgement = new(preJudgement, default);
            return false;
        }

        post = implyHandle.Apply(in pre);

        var ok = JudgeTheory.TryJudgeTrue(implyHandle.PostJudge, in pre, in post, out var postJudgement);
        applyJudgement = new(preJudgement, postJudgement);

        return ok;
    }

    public static bool IsTotalHandle<TP, TQ>(ArrowHandle<TP, TQ> handle)
    {
        return handle.PreJudge.IsTautology && handle.PostJudge.IsTautology;
    }
#endif    

    private static TConsequent ImplInternal<TAntecedent, TConsequent, T>(in TAntecedent ant)
        where T : IArrowPremise<TAntecedent, TConsequent>
    {
        return RealizerTheory.Realize<T>().Apply(in ant);
    }

    extension<TAntecedent, TConsequent, TArrow>(TArrow)
        where TArrow : IArrowPremise<TAntecedent, TConsequent>
    {
        public unsafe static ArrowHandle<TAntecedent, TConsequent> ToHandle()
        {
            if (typeof(TArrow) == typeof(Failure<TAntecedent, TConsequent>))
            {
                return GetFailureHandle<TAntecedent, TConsequent>();
            }
            return new(&ImplInternal<TAntecedent, TConsequent, TArrow>);
        }

        public static bool TryToTypeEqualHandle<TP2, TQ2>(out ArrowHandle<TP2, TQ2> handle)
        {
            if (!(typeof(TP2) == typeof(TAntecedent) && typeof(TQ2) == typeof(TConsequent)))
            {
                handle = default;
                return false;
            }

            var original = ToHandle<TAntecedent, TConsequent, TArrow>();
            handle = Unsafe.As<ArrowHandle<TAntecedent, TConsequent>, ArrowHandle<TP2, TQ2>>(ref original);
            return true;
        }
    }

#if false
    extension<TAntecedent, TPreJudge, TConsequent, TPostJudge, TArrow>(TArrow)
        where TPreJudge : IJudgePremise
        where TPostJudge : IJudgePremise
        where TArrow : IArrowPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge>
    {
        public static ArrowHandle<TAntecedent, TConsequent> ToHandle() =>
            ToHandle<TAntecedent, TConsequent, TArrow>().WithJudges
            (
                JudgeTheory.ToHandle<TPreJudge, TAntecedent>(),
                JudgeTheory.ToHandle<TPostJudge, (TAntecedent, TConsequent)>()
            );
    }



    public unsafe static ArrowHandle<TAntecedent, TConsequent> ToHandleForce<TAntecedent, TConsequent, T>()
        where T : unmanaged
    {
        ArrowHandle<TAntecedent, TConsequent> impHnd = new(&ImplInternal<TAntecedent, TConsequent, T>);
        if ((new T()) is IArrowPremise<TAntecedent, TConsequent>)
        {
            return impHnd;
        }
        else
        {
            return impHnd.WithJudges(JudgeTheory.GetTautologyHandle<TAntecedent>(), JudgeTheory.GetNegationHandle<(TAntecedent, TConsequent)>());
        }
    }
#endif
}

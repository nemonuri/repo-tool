using System.Runtime.CompilerServices;


namespace Nemonuri.PureTypeSystems.Primitives;


public readonly record struct ApplyJudgement(Judgement Pre, Judgement? Post)
{
}


public static class ArrowTheory
{
    public const nint IdentityPointer = 0;

    public static ArrowHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;

    public static ArrowHandle<TP, TQ> GetFailureHandle<TP, TQ>() => ToHandle<TP, Tautology, TQ, Negation, Failure<TP, TQ>>();
        

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

    private static TConsequent ImplInternal<TAntecedent, TConsequent, T>(in TAntecedent ant)
        where T : IArrowPremise<TAntecedent, TConsequent>
    {
        return Activator.CreateInstance<T>().Apply(in ant);
    }

    extension<TAntecedent, TConsequent, TArrow>(TArrow)
        where TArrow : IArrowPremise<TAntecedent, TConsequent>
    {
        public unsafe static ArrowHandle<TAntecedent, TConsequent> ToHandle()
        {
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


#if false
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

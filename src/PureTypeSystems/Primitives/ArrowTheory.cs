namespace Nemonuri.PureTypeSystems.Primitives;


public readonly record struct ApplyJudgement(Judgement Pre, Judgement? Post)
{
}


public static class ArrowTheory
{
    public const nint IdentityPointer = 0;

    public static ArrowHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;

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
        where T : unmanaged, IArrowPremise<TAntecedent, TConsequent>
    {
        return (new T()).Apply(in ant);
    }

    extension<TAntecedent, TConsequent, TArrow>(TArrow)
        where TArrow : unmanaged, IArrowPremise<TAntecedent, TConsequent>
    {
        public unsafe static ArrowHandle<TAntecedent, TConsequent> ToHandle()
        {
            return new(&ImplInternal<TAntecedent, TConsequent, TArrow>);
        }
    }

    extension<TAntecedent, TPreJudge, TConsequent, TPostJudge, TArrow>(TArrow)
        where TPreJudge : unmanaged, IJudgePremise
        where TPostJudge : unmanaged, IJudgePremise
        where TArrow : unmanaged, IArrowPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge>
    {
        public static ArrowHandle<TAntecedent, TConsequent> ToHandle() =>
            ToHandle<TAntecedent, TConsequent, TArrow>().WithJudges
            (
                JudgeTheory.FreeToHandle<TPreJudge, TAntecedent>(),
                JudgeTheory.FreeToHandle<TPostJudge, (TAntecedent, TConsequent)>()
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

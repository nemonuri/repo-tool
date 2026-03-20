namespace Nemonuri.PureTypeSystems.Primitives;


public readonly record struct ApplyJudgement(Judgement Pre, Judgement? Post)
{
}


public static class ImplyTheory
{
    public const nint IdentityPointer = 0;

    public static ImplyHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;

    public static bool TryApplyTrue<TP, TQ>
    (
        in ImplyHandle<TP, TQ> implyHandle, 
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

    public static bool IsTotalHandle<TP, TQ>(ImplyHandle<TP, TQ> handle)
    {
        return handle.PreJudge.IsTautology && handle.PostJudge.IsTautology;
    }

    private static TConsequent ImplInternal<TAntecedent, TConsequent, T>(in TAntecedent ant)
        where T : unmanaged
    {
        if ((new T()) is IImplyPremise<TAntecedent, TConsequent> p)
        {
            return p.Apply(in ant);
        }
        else
        {
            throw new InvalidOperationException(/* TODO */);
        }
    }

    extension<TAntecedent, TConsequent, TImply>(TImply)
        where TImply : unmanaged, IImplyPremise<TAntecedent, TConsequent>
    {
        public unsafe static ImplyHandle<TAntecedent, TConsequent> ToHandle()
        {
            return new(&ImplInternal<TAntecedent, TConsequent, TImply>);
        }
    }

    extension<TAntecedent, TPreJudge, TConsequent, TPostJudge, TImply>(TImply)
        where TPreJudge : unmanaged, IJudgePremise<TAntecedent>
        where TPostJudge : unmanaged, IJudgePremise<(TAntecedent, TConsequent)>
        where TImply : unmanaged, IImplyPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge>
    {
        public static ImplyHandle<TAntecedent, TConsequent> ToHandle() =>
            ToHandle<TAntecedent, TConsequent, TImply>().WithJudges
            (
                JudgeTheory.ToHandle<TAntecedent, TPreJudge>(),
                JudgeTheory.ToHandle<(TAntecedent, TConsequent), TPostJudge>()
            );
    }

    public unsafe static ImplyHandle<TAntecedent, TConsequent> ToHandleForce<TAntecedent, TConsequent, T>()
        where T : unmanaged
    {
        ImplyHandle<TAntecedent, TConsequent> impHnd = new(&ImplInternal<TAntecedent, TConsequent, T>);
        if ((new T()) is IImplyPremise<TAntecedent, TConsequent>)
        {
            return impHnd;
        }
        else
        {
            return impHnd.WithJudges(JudgeTheory.GetTautologyHandle<TAntecedent>(), JudgeTheory.GetNegationHandle<(TAntecedent, TConsequent)>());
        }
    }
}

namespace Nemonuri.PureTypeSystems.Primitives;

public static class ImplyTheory
{
    public const nint IdentityPointer = 0;

    public static ImplyHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;

    public static bool TryApplyWithJudge<TP, TQ>
    (
        in ImplyHandle<TP, TQ> implyHandle, 
        in TP? pre, 
        [NotNullWhen(true)] out TQ? post,
        out ApplyJudgement applyJudgement
    )
    {
        if (!JudgeTheory.TryJudgeTruthy(implyHandle.PreJudge, in pre, out var preJudgement))
        {
            post = default;
            applyJudgement = new(preJudgement, default);
            return false;
        }

        post = implyHandle.Apply(in pre);

        var ok = JudgeTheory.TryJudgeTruthy(implyHandle.PostJudge, in pre, in post, out var postJudgement);
        applyJudgement = new(preJudgement, postJudgement);

        return ok;
    }
}

public readonly record struct ApplyJudgement(Judgement Pre, Judgement? Post)
{
}
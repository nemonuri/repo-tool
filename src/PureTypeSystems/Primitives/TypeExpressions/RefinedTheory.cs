namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public static class RefinedTheory
{
    public static Refined<T> Upcast<T, TJudge>(in Refined<T, TJudge> refined)
        where TJudge : unmanaged, IJudgePremise
    {
        return new(refined.Value, JudgeTheory.FreeToHandle<TJudge, T>());
    }

    public static bool TryDowncast<T, TJudge>(in Refined<T> refined, out Refined<T, TJudge> casted)
        where TJudge : unmanaged, IJudgePremise
    {
/**

    ## Assume

    ∀(tj1 tj2 t: Type).( ((FreeToHandle tj1 t) = (FreeToHandle tj2 t)) → (tj1 = tj2) )

*/

        var handleOfTarget = JudgeTheory.FreeToHandle<TJudge, T>();
        if (!refined.JudgeHandle.Equals(handleOfTarget))
        {
            casted = default;
            return false;
        }

        casted = new(refined.Value);
        return true;
    }

    public static Judgement Judge<T>(in Refined<T> refined)
    {
        return refined.JudgeHandle.Judge(refined.Value);
    }

    public static Judgement Judge<T, TJudge>(in Refined<T, TJudge> refined)
        where TJudge : unmanaged, IJudgePremise
    {
        return Judge(Upcast(in refined));
    }
}

namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public static class RefinedTheory
{
    public static Refined<T, TJudge> TrustMe<T, TJudge>(T value)
        where TJudge : IJudgePremise
    {
        return new(value);
    }

    public static Refined<T> Upcast<T, TJudge>(in Refined<T, TJudge> refined)
        where TJudge : IJudgePremise
    {
        return new(refined.Value, JudgeTheory.ToHandle<TJudge, T>());
    }

    public static bool TryDowncast<T, TJudge>(in Refined<T> refined, out Refined<T, TJudge> casted)
        where TJudge : unmanaged, IJudgePremise
    {
/**

    ## Assume

    ∀(tj1 tj2 t: Type).( ((FreeToHandle tj1 t) = (FreeToHandle tj2 t)) → (tj1 = tj2) )

*/

        var handleOfTarget = JudgeTheory.ToHandle<TJudge, T>();
        if (!refined.JudgeHandle.Equals(handleOfTarget))
        {
            casted = default;
            return false;
        }

        casted = new(refined.Value);
        return true;
    }

    public static JudgeResult Judge<T>(in Refined<T> refined)
    {
        return refined.JudgeHandle.Judge(refined.Value);
    }

    public static JudgeResult Judge<T, TJudge>(in Refined<T, TJudge> refined)
        where TJudge : IJudgePremise
    {
        return Judge(Upcast(in refined));
    }
}

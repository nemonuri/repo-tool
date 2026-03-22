namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public static class ExpressionTheory
{
    public static Data<T> ToData<T>(T value) => new(value);

    public static RefinedData<T> ToRefinedData<T>(Data<T> data, JudgeHandle<T> handle = default) => new(data, handle);

    public static RefinedData<T> ToRefinedData<T>(Refined<T> refined) => new(ToData(refined.Value), refined.JudgeHandle);




    public static Judgement Judge<T>(in Data<T> data, JudgeHandle<T> handle)
    {
        return handle.Judge(data.Value);
    }

    public static Judgement Judge<T, TJudge>(in Data<T> data)
        where TJudge : unmanaged, IJudgePremise
    {
        return Judge(in data, JudgeTheory.FreeToHandle<TJudge, T>());
    }

    // public static Refined<TExpr, Tautology> ToRefined<TExpr>(TExpr Witness) => new(Witness);

#if false
    public static bool TryCheckRefined<TExpr, TJudge>(in Refined<TExpr, TJudge> refined, out TExpr)
        where TJudge : IJudgePremise
    {
        
    }
#endif
}

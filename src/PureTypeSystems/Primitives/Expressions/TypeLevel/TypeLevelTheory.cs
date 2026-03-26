namespace Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel;

public static class TypeLevelTheory
{
    public static Var Var => new();

    public static Empty Empty => Empty.Instance;

    public static Data<T> ToData<T>(T dataValueOrVar)
    {
        return new(dataValueOrVar);
    }

    public static App<THead, TTail> ToApp<THead, TTail>(TTail tail)
        where THead : IKindPremise<THead>
        where TTail : ITypeLevelExpression
    {
        return new(tail);
    }

    public static Guarded<TExpr, TJudge> ToGuarded<TExpr, TJudge>(TExpr expr)
        where TExpr : ITypeLevelExpression
        where TJudge : IJudgePremise
    {
        return new(expr);
    }

    public static bool CanEvaluate<TExpr, T>(TExpr expr, [NotNullWhen(true)] Func<TExpr, T>? evaluator)
        where TExpr : ITypeLevelExpression
    {
        if 
        (
            evaluator is not null &&
            expr.IsConcrete
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool TryToEvaluator<T>(Data<T> data, [NotNullWhen(true)] out Func<Data<T>, T>? evaluator)
    {
        static T Evaluator(Data<T> d) => d.Value;

        evaluator = CanEvaluate(data, Evaluator) ? Evaluator : null;
        return evaluator is not null;
    }

    public static bool TryToRefineEntry<TExpr, TJudge, T>(Guarded<TExpr, TJudge> guarded, Func<TExpr, T>? evaluator, out IndirectRefineEntry<TExpr, T, TJudge> refineEntry)
        where TExpr : ITypeLevelExpression
        where TJudge : IJudgePremise
    {
        if (CanEvaluate(guarded.Expression, evaluator))
        {
            refineEntry = new(guarded.Expression, evaluator);
            return true;
        }
        else
        {
            refineEntry = default;
            return false;
        }
    }



    

#if false
    public static Refined<Data<T>> ToRefinedData<T>(Data<T> data, JudgeHandle<Data<T>> handle = default) => new(data, handle);

    public static App<TKind, Refined<Data<T>>> ToApp<TKind, T>(Refined<Data<T>> dataExpr) => new(dataExpr);

    public static Refined<App<TKind, TExpr>> ToRefinedApp<TKind, TExpr>(App<TKind, TExpr> app, JudgeHandle<App<TKind, TExpr>> handle = default) => new(app, handle);

    public static App<THeadKind, Refined<App<TTailKind, TExpr>>> ToApp<THeadKind, TTailKind, TExpr>(Refined<App<TTailKind, TExpr>> app) => new(app);

    public static App<TKind, TExpr> UnsafeToApp<TKind, TExpr>(TExpr expr) => new(expr);


    public static App<TKind, App<TKind, TExpr>> ToApp<TKind, TExpr>(RefinedApp<TKind, TExpr> appExpr)
    {
        return new(appExpr);
    }

    public static Refined<T> FromData<T>(Data<T> data) => new(data.Value, JudgeTheory.GetTautologyHandle<T>());

    public static Refined<T> FromRefinedData<T>(RefinedData<T> data) => new(data.Data.Value, data.JudgeHandle);

    public static Refined<TExpr> FromRefinedApp<TKind, TExpr>(RefinedApp<TKind, TExpr> app)
    {
        return new (app.)
        
    }
#endif
}

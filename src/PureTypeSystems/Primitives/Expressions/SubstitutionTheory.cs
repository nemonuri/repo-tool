using Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel;

namespace Nemonuri.PureTypeSystems.Primitives.Expressions;

public static class SubstitutionTheory
{
    public static Data<T> SubstituteInDataLevel<T>(Data<Var> template, T dataValue) => TypeLevelTheory.ToData(dataValue);

    public static App<THead, Data<T>> SubstituteInDataLevel<THead, T>(App<THead, Var> template, T dataValue)
        where THead : IKindPremise<THead>
    {
        return TypeLevelTheory.ToApp<THead, Data<T>>(TypeLevelTheory.ToData(dataValue));
    }

    public static Guarded<Data<T>, TJudge> SubstituteInDataLevel<T, TJudge>(Guarded<Var, TJudge> template, T dataValue)
        where TJudge : IJudgePremise
    {
        return TypeLevelTheory.ToGuarded<Data<T>, TJudge>(TypeLevelTheory.ToData(dataValue));
    }


    public static App<THead, TTail> SubstituteInTypeLevel<THead, TTail>(App<THead, Var> template, TTail tail)
        where THead : IKindPremise<THead>
        where TTail : ITypeLevelExpression
    {
        return TypeLevelTheory.ToApp<THead, TTail>(tail);
    }

    public static Guarded<TExpr, TJudge> SubstituteInTypeLevel<TExpr, TJudge>(Guarded<Var, TJudge> template, TExpr expr)
        where TExpr : ITypeLevelExpression
        where TJudge : IJudgePremise
    {
        return TypeLevelTheory.ToGuarded<TExpr, TJudge>(expr);
    }
}

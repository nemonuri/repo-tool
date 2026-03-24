namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;


public interface IGuardPremise<T, TJudge, TExpr> where TJudge : IJudgePremise
{
    T MapTo(in TExpr expr);

    TExpr MapFrom(in T value);
}

public readonly struct DataGuard<T, TJudge> : IGuardPremise<T, TJudge, Data<T>>
    where TJudge : IJudgePremise
{
    public static T MapTo(in Data<T> expr) => expr.Value;

    public static Data<T> MapFrom(in T value) => new(value);


    T IGuardPremise<T, TJudge, Data<T>>.MapTo(in Data<T> expr) => MapTo(in expr);

    Data<T> IGuardPremise<T, TJudge, Data<T>>.MapFrom(in T value) => MapFrom(in value);
}

public interface IAppGuardPremise<T, TJudge, THeadKind, TTailExpr> : IGuardPremise<T, TJudge, App<THeadKind, TTailExpr>>
    where TJudge : IJudgePremise
{
}

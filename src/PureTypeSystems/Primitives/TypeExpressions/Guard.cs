using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;


public interface IGuardPremise<T, TJudge, TExpr> where TJudge : IJudgePremise
{
    T MapTo(in TExpr expr);

    TExpr MapFrom(in T value);
}

public readonly struct DataGuard<T, TJudge> : IGuardPremise<T, TJudge, Data<T>>
    where TJudge : IJudgePremise
{
    public static T MapTo(Data<T> expr) => expr.Value;

    public static Data<T> MapFrom(T value) => new(value);


    T IGuardPremise<T, TJudge, Data<T>>.MapTo(in Data<T> expr) => MapTo(expr);

    Data<T> IGuardPremise<T, TJudge, Data<T>>.MapFrom(in T value) => MapFrom(value);
}

public readonly struct VarGuard<TJudge> : IGuardPremise<ValueUnit, TJudge, Var>
    where TJudge : IJudgePremise
{
    public static ValueUnit MapTo(Var expr) => ValueUnitTheory.Singleton;

    public static Var MapFrom(ValueUnit value) => ExpressionTheory.Var;

    ValueUnit IGuardPremise<ValueUnit, TJudge, Var>.MapTo(in Var expr) => MapTo(expr);

    Var IGuardPremise<ValueUnit, TJudge, Var>.MapFrom(in ValueUnit value) => MapFrom(value);
}

public interface IAppGuardPremise<T, TJudge, THead, TTail> : IGuardPremise<T, TJudge, App<THead, TTail>>
    where TJudge : IJudgePremise
{
}

public static class GuardTheory
{
    extension<T, TJudge, TExpr, TGuard>(TGuard)
        where TJudge : IJudgePremise
        where TGuard : IGuardPremise<T, TJudge, TExpr>
    {
        private static TGuard ToInstance() => RealizerTheory.Realize<TGuard>();

        //public static T MapTo(in TExpr expr) => GuardTheory.ToInstance<T, TJudge, TExpr, TGuard>().MapTo(in expr);

        //public static TExpr MapFrom(in T expr) => GuardTheory.ToInstance<T, TJudge, TExpr, TGuard>().MapFrom(in expr);

        public static bool IsGuardable<TState>
        (
            IRefiner<TState> refiner,
            in TState prevState,
            in TExpr expr,
            out TState nextState
        )
        {
            TGuard inst = GuardTheory.ToInstance<T, TJudge, TExpr, TGuard>();
            T v = inst.MapTo(in expr);
            return refiner.IsRefineable<T, TJudge>(in prevState, in v, out nextState);
        }
    }

    extension<TJudge, TExpr, TGuard>(TGuard)
        where TJudge : IJudgePremise
        where TGuard : IGuardPremise<ValueUnit, TJudge, TExpr>
    {
        
    }
}



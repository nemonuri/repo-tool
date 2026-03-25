namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public readonly record struct KindEntry<TExpr, TGuard>
{
    public TExpr KindExpression {get;}

    internal KindEntry(TExpr kindExpression)
    {
        KindExpression = kindExpression;
    }   
}

public readonly record struct DotNetEntry<T, TJudge>
{
    public T DotNetValue {get;}

    internal DotNetEntry(T dotNetValue)
    {
        DotNetValue = dotNetValue;
    }
}


public readonly struct DataConstructorEntry<T, TJudge, TAssumedJuge, TAssumedExpr, TAssumedGuard>
    where TJudge : IJudgePremise
    where TAssumedJuge : IJudgePremise
    where TAssumedGuard : IGuardPremise<ValueUnit, TAssumedJuge, TAssumedExpr>
{
    public DataConstructorEntry(KindEntry<TAssumedExpr, TAssumedGuard> kind, DotNetEntry<T, TJudge> dotnet)
    {
        AssumedKindEntry = kind;
        DotNetEntry = dotnet;
    }

    public KindEntry<TAssumedExpr, TAssumedGuard> AssumedKindEntry {get;}

    public DotNetEntry<T, TJudge> DotNetEntry {get;}
    

    public bool IsKindEntryUnbound => 
        (typeof(TAssumedExpr) == typeof(Var)) && (typeof(TAssumedGuard) == typeof(VarGuard<Unknown>));

    public bool IsDotNetEntryUnbound => 
        (typeof(T) == typeof(ValueUnit)) && (typeof(TJudge) == typeof(Unknown));
}

public readonly struct KindConstructorEntry<T, TJudge, TExpr, TGuard, TAssumedJuge, TAssumedExpr, TAssumedGuard>
    where TJudge : IJudgePremise
    where TGuard : IGuardPremise<T, TJudge, TExpr>
    where TAssumedJuge : IJudgePremise
    where TAssumedGuard : IGuardPremise<ValueUnit, TAssumedJuge, TAssumedExpr>
{
    public KindConstructorEntry(KindEntry<TAssumedExpr, TAssumedGuard> assumedKindEntry, KindEntry<TExpr, TGuard> kindEntry)
    {
        AssumedKindEntry = assumedKindEntry;
        KindEntry = kindEntry;
    }

    public KindEntry<TAssumedExpr, TAssumedGuard> AssumedKindEntry {get;}

    public KindEntry<TExpr, TGuard> KindEntry {get;}
}

public static class EntryTheory
{
    public static DotNetEntry<ValueUnit, Unknown> UnboundDotNetEntry => new();

    public static DotNetEntry<T, TJudge> ToDotNetEntry<T, TJudge>(T value) 
        where TJudge : IJudgePremise
        => new(value);

    public static KindEntry<Var, VarGuard<TJudge>> ToVarEntry<TJudge>()
        where TJudge : IJudgePremise
        => new();

    public static KindEntry<Data<T>, DataGuard<T, TJudge>> ToDataEntry<T, TJudge>(DotNetEntry<T, TJudge> dotNetEntry)
        where TJudge : IJudgePremise
    {
        Data<T> data = new(dotNetEntry.DotNetValue);
        return new(data);
    }

    public static KindEntry<App<THead, TTail>, TGuard> ToAppEntry<T, TJudge, THead, TTail, TGuard>(TTail tail)
        where TJudge : IJudgePremise
        where TGuard : IAppGuardPremise<T, TJudge, THead, TTail>
    {
        App<THead, TTail> app = new(tail);
        return new(app);
    }

    public static KindEntry<Data<ValueUnit>, DataGuard<ValueUnit, TJudge>> AssumeDataEntry<TJudge>()
        where TJudge : IJudgePremise
    {
        Data<ValueUnit> data = new(ValueUnitTheory.Singleton);
        return new(data);
    }

    public static KindEntry<App<THead, Var>, TGuard> AssumeAppEntry<TJudge, THead, TTail, TGuard>()
        where TJudge : IJudgePremise
        where TGuard : IAppGuardPremise<ValueUnit, TJudge, THead, Var>
    {
        App<THead, Var> app = new(ExpressionTheory.Var);
        return new(app);
    }

    public static bool TryDotNetEntryToRefined<TState, T, TJudge>
    (
        IRefiner<TState> refiner, 
        in TState prevState, 
        in DotNetEntry<T, TJudge> entry, 
        out TState nextState,
        out Refined<T, TJudge> refined
    )
        where TJudge : IJudgePremise
    {
        var v = entry.DotNetValue;
        bool ok = refiner.IsRefineable<T, TJudge>(in prevState, in v, out nextState);
        refined = ok ? new(v) : default;
        return ok;
    }

#if false
    public static bool TryVarEntryToGuarded<TState, TJudge>
    (
        IRefiner<TState> refiner, 
        in TState prevState, 
        in KindEntry<Var, VarGuard<TJudge>> varEntry, 
        out TState nextState,
        out Guarded<Var, VarGuard<TJudge>> guarded
    )
        where TJudge : IJudgePremise
    {
        Var expr = varEntry.KindExpression;
        bool ok = GuardTheory.IsGuardable<ValueUnit, TJudge, Var, VarGuard<TJudge>, TState>(refiner, in prevState, in expr, out nextState);
        guarded = ok ? new(expr) : default;
        return ok;
    }
#endif

    public static bool TryDataEntryToGuarded<TState, T, TJudge>
    (
        IRefiner<TState> refiner, 
        in TState prevState, 
        in KindEntry<Data<T>, DataGuard<T, TJudge>> dataEntry, 
        out TState nextState,
        out Guarded<Data<T>, DataGuard<T, TJudge>> guarded
    )
        where TJudge : IJudgePremise
    {
        Data<T> expr = dataEntry.KindExpression;
        bool ok = GuardTheory.IsGuardable<T, TJudge, Data<T>, DataGuard<T, TJudge>, TState>(refiner, in prevState, in expr, out nextState);
        guarded = ok ? new(expr) : default;
        return ok;
    }

    public static bool TryAppEntryToGuarded<TState, T, TJudge, THead, TTail, TGuard>
    (
        IRefiner<TState> refiner, 
        in TState prevState, 
        in KindEntry<App<THead, TTail>, TGuard> appEntry, 
        out TState nextState,
        out Guarded<App<THead, TTail>, TGuard> guarded
    )
        where TJudge : IJudgePremise
        where TGuard : IAppGuardPremise<T, TJudge, THead, TTail>
    {
        App<THead, TTail> expr = appEntry.KindExpression;
        bool ok = GuardTheory.IsGuardable<T, TJudge, App<THead, TTail>, TGuard, TState>(refiner, in prevState, in expr, out nextState);
        guarded = ok ? new(expr) : default;
        return ok;
    }


}

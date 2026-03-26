namespace Nemonuri.PureTypeSystems.Primitives.Expressions;

public static class DataLevelTheory
{
    public static ValueUnit ValueUnit => System.ValueTuple.Create();

    public static RefineEntry<T, Tautology> ToRefineEntry<T>(T value) => new(value);

    public static IndirectRefineEntry<TSource, TTarget, Tautology> ToIndirectRefineEntry<TSource, TTarget>(TSource source, Func<TSource, TTarget> converter) 
        => new(source, converter);

    public static bool TryToRefined<TState, T, TJudge, TEntry>
    (
        IRefiner<TState> refiner, 
        in TState prevState, 
        in TEntry entry, 
        out TState nextState,
        out Refined<T, TJudge> refined
    )
        where TJudge : IJudgePremise
        where TEntry : IRefineEntry<T, TJudge>
    {
        var v = entry.GetValue();
        bool ok = refiner.IsRefineable<T, TJudge>(in prevState, in v, out nextState);
        refined = ok ? new(v) : default;
        return ok;
    }
    
}

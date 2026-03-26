namespace Nemonuri.PureTypeSystems.Primitives.Expressions;

public interface IRefineEntry<T, TJudge> where TJudge : IJudgePremise
{
    T GetValue();
}

public readonly struct RefineEntry<T, TJudge> : IRefineEntry<T, TJudge>
    where TJudge : IJudgePremise
{
    public RefineEntry(T value)
    {
        Value = value;
    }

    public T Value {get;}

    T IRefineEntry<T, TJudge>.GetValue() => Value;
}

public readonly struct IndirectRefineEntry<TSource, TTarget, TJudge> : IRefineEntry<TTarget, TJudge>
    where TJudge : IJudgePremise
{
    public IndirectRefineEntry(TSource source, Func<TSource, TTarget> converter)
    {
        Source = source;
        Converter = converter;
    }

    public TSource Source {get;}

    public Func<TSource, TTarget> Converter {get;}

    TTarget IRefineEntry<TTarget, TJudge>.GetValue() => Converter(Source);
}

namespace Nemonuri.PureTypeSystems.Primitives;

public readonly record struct Refined<T, TJudge> where TJudge : IJudgePremise
{
    public T Value {get;}

    internal Refined(T value)
    {
        Value = value;
    }
}

#if false
public readonly record struct IndirectlyRefined<T, TJudge, TTarget, TBijection>
    where TJudge : IJudgePremise
    where TBijection : IBijectionPremise<T, TTarget>
{
    public T Value {get;}

    internal IndirectlyRefined(T value)
    {
        Value = value;
    }
}
#endif
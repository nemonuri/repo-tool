namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

#if false
[StructLayout(LayoutKind.Sequential)]
public readonly struct Refined<T, TJudge>(T value) //where TJudge : IJudgePremise<T>
{
    private readonly T _value = value;

    public T Value => _value;
}

public static class RefineTheory
{
    public static Refined<Negation, T> GetContradiction<T>() => default;
}
#endif


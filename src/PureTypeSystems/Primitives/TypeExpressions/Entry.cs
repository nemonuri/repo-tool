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


public readonly struct ComposedEntry<TExpr, TGuard, T, TJudge>
{
    internal ComposedEntry(KindEntry<TExpr, TGuard> kind, DotNetEntry<T, TJudge> dotnet)
    {
        KindEntry = kind;
        DotNetEntry = dotnet;
    }

    public KindEntry<TExpr, TGuard> KindEntry {get;}

    public DotNetEntry<T, TJudge> DotNetEntry {get;}
    

    public bool IsKindEntryUnbound => 
        (typeof(TExpr) == typeof(Var)) && (typeof(TGuard) == typeof(VarGuard<Unknown>));

    public bool IsDotNetEntryUnbound => 
        (typeof(T) == typeof(ValueUnit)) && (typeof(TJudge) == typeof(Unknown));
}


public static class EntryTheory
{

}

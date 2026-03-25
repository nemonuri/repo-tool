namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public readonly struct MonadEntry<TExpr, TGuard, T, TJudge>
{
    internal MonadEntry(Guarded<TExpr, TGuard> kind, Refined<T, TJudge> dotnet)
    {
        Kind = kind;
        DotNet = dotnet;
    }

    public Guarded<TExpr, TGuard> Kind {get;}

    public Refined<T, TJudge> DotNet {get;}
    

    public bool IsKindUnbound => 
        (typeof(TExpr) == typeof(Var)) && (typeof(TGuard) == typeof(VarGuard<Unknown>));

    public bool IsDotNetUnbound => 
        (typeof(T) == typeof(object)) && (typeof(TJudge) == typeof(Unknown));
}


public static class MonadEntryTheory
{
    public static MonadEntry<Var, VarGuard<Unknown>, object?, Unknown> Unbound => default;

    public static MonadEntry<TExpr, TGuard, T, TJudge> BindDotNet<TExpr, TGuard, T, TJudge>(MonadEntry<TExpr, TGuard, object?, Unknown> entry, Refined<T, TJudge> dotnet) 
        => new(entry.Kind, dotnet);

    public static MonadEntry<TExpr, TGuard, T, TJudge> BindKind<TExpr, TGuard, T, TJudge>(MonadEntry<Var, VarGuard<Unknown>, T, TJudge> entry, Guarded<TExpr, TGuard> kind)
        => new(kind, entry.DotNet);
}

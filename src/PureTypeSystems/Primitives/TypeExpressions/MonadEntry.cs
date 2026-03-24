namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public readonly struct MonadEntry<TExpr, TGuard, T, TJudge>
{
    internal MonadEntry(Guarded<TExpr, TGuard> left, Refined<T, TJudge> right)
    {
        Left = left;
        Right = right;
    }

    public Guarded<TExpr, TGuard> Left {get;}

    public Refined<T, TJudge> Right {get;}
    

    public bool IsLeftEmpty => typeof(TExpr) == typeof(Empty);

    public bool IsRightEmpty => typeof(T) == typeof(ValueUnit);
}


namespace Nemonuri.PureTypeSystems.Primitives.Expressions.TypeLevel;

public interface ITypeLevelExpression
{
    bool IsConcrete {get;}
}

public readonly struct Var : ITypeLevelExpression
{
    public bool IsConcrete => false;
}

public class Empty : ITypeLevelExpression
{
    internal static Empty Instance {get;} = new();

    private Empty() {}

    public bool IsConcrete => true;
}

public readonly record struct Data<T> : ITypeLevelExpression
{
    public T Value {get;}

    internal Data(T value)
    {
        Value = value;
    }

    public bool IsConcrete => Value is ITypeLevelExpression v ? v.IsConcrete : true;
}

public readonly record struct App<TKind, TExpr> : ITypeLevelExpression
    where TKind : IKindPremise<TKind>
    where TExpr : ITypeLevelExpression
{
    public TExpr Expression {get;}

    internal App(TExpr expr)
    {
        Expression = expr;
    }

    public bool IsConcrete => Expression.IsConcrete;
}

public readonly record struct Guarded<TExpr, TJudge> : ITypeLevelExpression
    where TExpr : ITypeLevelExpression
    where TJudge : IJudgePremise
{
    public TExpr Expression {get;}

    internal Guarded(TExpr expr)
    {
        Expression = expr;
    }

    public bool IsConcrete => Expression.IsConcrete;
}



#if false
public readonly record struct GuardEntry<TExpr, TGuard> : ITypeLevelExpression
{
    public TExpr Expression {get;}

    internal GuardEntry(TExpr expression)
    {
        Expression = expression;
    }   
}
#endif

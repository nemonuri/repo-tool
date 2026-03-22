namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

//public readonly record struct Expr<TExpr>(TExpr? Value);

//public readonly record struct Var<TVar>(TVar? Value);

public readonly record struct Data<T>
{
    public T Value {get;}

    internal Data(T value)
    {
        Value = value;
    }
}

public readonly record struct App<TKind, TExpr>
{
    public TExpr Expression {get;}

    internal App(TExpr expr)
    {
        Expression = expr;
    }
}

// public readonly record struct Guard<TExpr, TJudge>(TExpr Expression) where TJudge : IJudgePremise;

public readonly record struct Refined<T, TJudge>
{
    public T Value {get;}

    internal Refined(T value)
    {
        Value = value;
    }
}

public readonly record struct Refined<T>
{
    public T Value {get;}
    public JudgeHandle<T> JudgeHandle {get;}

    internal Refined(T value, JudgeHandle<T> judgeHandle)
    {
        Value = value;
        JudgeHandle = judgeHandle;
    }
}

#if false
public readonly record struct RefinedData<T>
{
    public Data<T> Data {get;}
    public JudgeHandle<T> JudgeHandle {get;}

    internal RefinedData(Data<T> data, JudgeHandle<T> judgeHandle)
    {
        Data = data;
        JudgeHandle = judgeHandle;
    }
}


public readonly record struct RefinedApp<TKind, TExpr> 
{
    public TExpr Expression {get;}
    public JudgeHandle<App<TKind, TExpr>> JudgeHandle {get;}

    internal RefinedApp(TExpr expression, JudgeHandle<App<TKind, TExpr>> judgeHandle)
    {
        Expression = expression;
        JudgeHandle = judgeHandle;
    }
}
#endif

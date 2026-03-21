namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

//public readonly record struct Expr<TExpr>(TExpr? Value);

//public readonly record struct Var<TVar>(TVar? Value);

public readonly record struct Data<TData>(TData Value);

public readonly record struct App<TKind, TKindOrData>(TKindOrData Value);

public readonly record struct Guard<TExpr, TJudge>(TExpr Value) where TJudge : IJudgePremise;

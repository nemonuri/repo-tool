namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public readonly record struct Expr<TExpr>(TExpr? Value);

public readonly record struct Var<TVar>(TVar? Value);

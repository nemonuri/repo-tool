namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

//public readonly record struct Expr<TExpr>(TExpr? Value);

//public readonly record struct Var<TVar>(TVar? Value);

public readonly record struct Data<T>(T Value);

public readonly record struct App<TKind, TAppOrData>(TAppOrData Value);

// public readonly record struct Guard<TExpr, TJudge>(TExpr Expression) where TJudge : IJudgePremise;

public readonly record struct Refined<T, TJudge>(T Value);

public readonly record struct Refined<T>(T Value, JudgeHandle<T> JudgeHandle);

public readonly record struct RefinedData<T>(Data<T> Data, JudgeHandle<T> JudgeHandle);

public readonly record struct RefinedApp<TKind, TAppOrData>(TAppOrData Value, object? Unthunker);

namespace Nemonuri.PureTypeSystems.Primitives.TypeExpressions;

public static class ExpressionTheory
{
    public static Data<T> ToData<T>(T value) => new(value);

    public static Refined<Data<T>> ToRefinedData<T>(Data<T> data, JudgeHandle<Data<T>> handle = default) => new(data, handle);

    public static App<TKind, Refined<Data<T>>> ToApp<TKind, T>(Refined<Data<T>> dataExpr) => new(dataExpr);

    public static Refined<App<TKind, TExpr>> ToRefinedApp<TKind, TExpr>(App<TKind, TExpr> app, JudgeHandle<App<TKind, TExpr>> handle = default) => new(app, handle);

    public static App<THeadKind, Refined<App<TTailKind, TExpr>>> ToApp<THeadKind, TTailKind, TExpr>(Refined<App<TTailKind, TExpr>> app) => new(app);

#if false
    public static App<TKind, App<TKind, TExpr>> ToApp<TKind, TExpr>(RefinedApp<TKind, TExpr> appExpr)
    {
        return new(appExpr);
    }

    public static Refined<T> FromData<T>(Data<T> data) => new(data.Value, JudgeTheory.GetTautologyHandle<T>());

    public static Refined<T> FromRefinedData<T>(RefinedData<T> data) => new(data.Data.Value, data.JudgeHandle);

    public static Refined<TExpr> FromRefinedApp<TKind, TExpr>(RefinedApp<TKind, TExpr> app)
    {
        return new (app.)
        
    }
#endif
}

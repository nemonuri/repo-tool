namespace Nemonuri.FStarDotNet.Primitives;

/**
- Pure type system 에서는, Type 을 Instance 보다 더 추상적인 개념으로 다루는 것이 좋다.
*/

public interface IFStarFunction : IFStarInstance<Func<IFStarType?, IFStarType?>>
{
    IFStarType? Invoke(IFStarType? source);
}

public interface IFStarFunctionInvocation<TFunc, TAux> : IFStarType
    where TFunc : IFStarFunction
{
}

public interface IFStarFunction<in TSource, out TResult> : IFStarFunction
    where TSource : IFStarType?
    where TResult : IFStarType?
{
    TResult Invoke(TSource source);
}

public readonly struct FStarFunction<TSource, TResult> : IFStarFunction<TSource, TResult>
    where TSource : IFStarType?
    where TResult : IFStarType?
{
    private readonly Func<TSource, TResult> _func;

    public FStarFunction(Func<TSource, TResult> func)
    {
        Guard.IsNotNull(func);
        _func = func;
    }

    public TResult Invoke(TSource source) => _func(source);

    IFStarType? IFStarFunction.Invoke(IFStarType? source) => source is TSource t ? Invoke(t) : null;

    public Func<IFStarType?, IFStarType?> Value => ((IFStarFunction)this).Invoke;

    object? IFStarInstance.Value => Value;

    public Type GetSolvedDotNetType() => FStarTypeTheory.GetSolvedDotNetType(this);

    public TypeList<Func<IFStarType?, IFStarType?>, EmptyTypeList> GetDotNetTypes() => FStarTypeTheory.GetDotNetTypes(this);

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();
}

public readonly struct IdentityFStarFunction<T> : IFStarFunction<T, T>
    where T : IFStarType?
{
    public T Invoke(T source) => source;

    IFStarType? IFStarFunction.Invoke(IFStarType? source) => source;

    public Func<IFStarType?, IFStarType?> Value => ((IFStarFunction)this).Invoke;

    object? IFStarInstance.Value => Value;

    public Type GetSolvedDotNetType() => FStarTypeTheory.GetSolvedDotNetType(this);

    public TypeList<Func<IFStarType?, IFStarType?>, EmptyTypeList> GetDotNetTypes() => FStarTypeTheory.GetDotNetTypes(this);

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();
}

public static class FStarFunctionTheory
{
    public static FStarFunction<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> func)
        where TSource : IFStarType?
        where TResult : IFStarType?
        => new(func);

    public static IdentityFStarFunction<T> CreateIdentity<T>() where T : IFStarType? => new();

    public static NewSourcePremise<TNewSource> IntroduceNewSource<TNewSource>() where TNewSource : IFStarType? => new();

    public readonly struct NewSourcePremise<TNewSource> where TNewSource : IFStarType?
    {
        public static FinalResultPremise<TFinalResult> IntroduceFinalResult<TFinalResult>() where TFinalResult : IFStarType? => new();

        public static FinalResultPremise<TFinalResult> IntroduceFinalResult<TOldSource, TFinalResult>(IFStarFunction<TOldSource, TFinalResult> func) 
            where TOldSource : IFStarType?
            where TFinalResult : IFStarType? 
            => new();

        public readonly struct FinalResultPremise<TFinalResult> where TFinalResult : IFStarType?
        {
            public FStarFunction<TNewSource, TFinalResult> Cons<TStepResult, TContiuation>(Func<TNewSource, TStepResult> precession, TContiuation contiuation)
                where TStepResult : IFStarType?
                where TContiuation : IFStarFunction<TStepResult, TFinalResult>
            {
                Guard.IsNotNull(precession);
                
                TFinalResult NewFunc(TNewSource source) => contiuation.Invoke(precession(source));

                return new (NewFunc);
            }
        }
    } 
}
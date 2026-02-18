namespace Nemonuri.FStarDotNet.Primitives;

public interface IFStarFunction : IFStarValue<Func<IFStarValue?, IFStarValue?>>
{
    IFStarValue? Invoke(IFStarValue? source);
}

public interface IFStarFunction<in TSource, out TResult> : IFStarFunction
    where TSource : IFStarValue?
    where TResult : IFStarValue?
{
    TResult Invoke(TSource source);
}

public readonly struct FStarFunction<TSource, TResult> : IFStarFunction<TSource, TResult>
    where TSource : IFStarValue?
    where TResult : IFStarValue?
{
    private readonly Func<TSource, TResult> _func;

    public FStarFunction(Func<TSource, TResult> func)
    {
        Guard.IsNotNull(func);
        _func = func;
    }

    public TResult Invoke(TSource source) => _func(source);

    IFStarValue? IFStarFunction.Invoke(IFStarValue? source) => source is TSource t ? Invoke(t) : null;

    Func<IFStarValue?, IFStarValue?> IFStarValue<Func<IFStarValue?, IFStarValue?>>.Value => ((IFStarFunction)this).Invoke;

    object? IFStarValue.Value => ((IFStarValue<Func<IFStarValue?, IFStarValue?>>)this).Value;
}

public readonly struct IdentityFStarFunction<T> : IFStarFunction<T, T>
    where T : IFStarValue?
{
    public T Invoke(T source) => source;

    IFStarValue? IFStarFunction.Invoke(IFStarValue? source) => source;

    Func<IFStarValue?, IFStarValue?> IFStarValue<Func<IFStarValue?, IFStarValue?>>.Value => ((IFStarFunction)this).Invoke;

    object? IFStarValue.Value => ((IFStarValue<Func<IFStarValue?, IFStarValue?>>)this).Value;
}

public static class FStarFunctionTheory
{
    public static FStarFunction<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> func)
        where TSource : IFStarValue?
        where TResult : IFStarValue?
        => new(func);

    public static IdentityFStarFunction<T> CreateIdentity<T>() where T : IFStarValue? => new();

    public static NewSourcePremise<TNewSource> IntroduceNewSource<TNewSource>() where TNewSource : IFStarValue? => new();

    public readonly struct NewSourcePremise<TNewSource> where TNewSource : IFStarValue?
    {
        public static FinalResultPremise<TFinalResult> IntroduceFinalResult<TFinalResult>() where TFinalResult : IFStarValue? => new();

        public static FinalResultPremise<TFinalResult> IntroduceFinalResult<TOldSource, TFinalResult>(IFStarFunction<TOldSource, TFinalResult> func) 
            where TOldSource : IFStarValue?
            where TFinalResult : IFStarValue? 
            => new();

        public readonly struct FinalResultPremise<TFinalResult> where TFinalResult : IFStarValue?
        {
            public FStarFunction<TNewSource, TFinalResult> Cons<TStepResult, TContiuation>(Func<TNewSource, TStepResult> precession, TContiuation contiuation)
                where TStepResult : IFStarValue?
                where TContiuation : IFStarFunction<TStepResult, TFinalResult>
            {
                Guard.IsNotNull(precession);
                
                TFinalResult NewFunc(TNewSource source) => contiuation.Invoke(precession(source));

                return new (NewFunc);
            }
        }
    } 
}
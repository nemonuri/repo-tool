namespace Nemonuri.FStarDotNet.Primitives;

public interface IPremise<TFrame>
    where TFrame : unmanaged, IPremise<TFrame>
{
    
}

public interface IFunctionZeroArityPremise<TFrame, TResult> : IPremise<TFrame>
    where TFrame : unmanaged, IFunctionZeroArityPremise<TFrame, TResult>
{
    TResult Invoke();
}

public interface IFunctionOneArityPremise<TFrame, T, TResult> : IPremise<TFrame>
    where TFrame : unmanaged, IFunctionOneArityPremise<TFrame, T, TResult>
{
    TResult Invoke(T argument);
}

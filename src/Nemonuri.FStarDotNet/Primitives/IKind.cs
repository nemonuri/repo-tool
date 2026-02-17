using CommunityToolkit.Diagnostics;

namespace Nemonuri.FStarDotNet.Primitives;

public interface IFunction<in T1, out T2>
{
    T2 Invoke(T1 t);
}

public class DotNetFunction<T1, T2> : IFunction<T1, T2>
{
    private readonly Func<T1, T2> _func;

    public DotNetFunction(Func<T1, T2> func)
    {
        Guard.IsNotNull(func);
        _func = func;
    }

    public T2 Invoke(T1 t) => _func.Invoke(t);
}

public interface IFStarType<T>
{
    T ReturnAs();
}

public interface IFStarKind<T2> : IFStarType<IFunction<T1, T2>>
{
/**
- Second or higher order logic
*/
    
}


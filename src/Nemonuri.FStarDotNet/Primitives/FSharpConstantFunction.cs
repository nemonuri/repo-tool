using Microsoft.FSharp.Core;

namespace Nemonuri.FStarDotNet.Primitives;

public sealed class FSharpConstantFunction<T> : FSharpFunc<object?, T>
{
    public override T Invoke(object? func)
    {
        throw new NotImplementedException();
    }
}

public interface IDependentTypedFunction<TFunction>
    where TFunction : IDependentTypedFunction<TFunction>
{
    DependentTypedTarget<TFunction, TSource> Invoke<TSource>(TSource source);
}

public readonly struct DependentTypedTarget<TFunction, TSource>
    where TFunction : IDependentTypedFunction<TFunction>
{
    public DependentTypedTarget(object boxedValue)
    {
        BoxedValue = boxedValue;
    }

    public object BoxedValue {get;}
}

public interface IDependentTypedTargetUnboxer<TFunction, TSource>
    where TFunction : IDependentTypedFunction<TFunction>
{
}

public readonly struct DependentTypedTargetUnboxer<TFunction, TSource, TTarget>
    where TFunction : IDependentTypedFunction<TFunction>
{
    private readonly Func<DependentTypedTarget<TFunction, TSource>, TTarget> _unboxer;

    public TTarget Unbox(DependentTypedTarget<TFunction, TSource> target)
    {
        return _unboxer(target);
    }
}

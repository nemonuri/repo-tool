using CommunityToolkit.Diagnostics;

namespace Nemonuri.FStarDotNet.Primitives;

#if false
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
#endif

public interface IFStarTypedValue<T>
{
    T Value {get;}
}


public readonly struct DefaultFStarTypedValue<T> : IFStarTypedValue<T>
{
    public T Value {get;}

    public DefaultFStarTypedValue(T value)
    {
        Value = value;
    }
}

public interface IFStarKind<TType> : IFStarTypedValue<TType>
    where TType : unmanaged, ITypeList
{
}

public readonly struct DefaultFStarKind<TType> : IFStarKind<TType>
    where TType : unmanaged, ITypeList
{
    public TType Value => new();
}

public static class FStarKindTheory
{
    public static DefaultFStarKind<EmptyTypeList> Empty => default;

    public static DefaultFStarKind<TTypeList> Create<TTypeList>(TTypeList tl) 
        where TTypeList : unmanaged, ITypeList
        => new();

    public readonly struct ConsPremise<THead>
    {
        public DefaultFStarKind<TypeList<THead, TTailTypeList>> Invoke<TTailTypeList>(IFStarKind<TTailTypeList> kind) 
            where TTailTypeList : unmanaged, ITypeList
        {
            return Create(new TypeListTheory.ConsPremise<THead>().Invoke(kind.Value));
        }
    }
}

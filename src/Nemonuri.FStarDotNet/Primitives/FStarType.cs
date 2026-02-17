namespace Nemonuri.FStarDotNet.Primitives;

public interface IFStarValue
{
    object? Value {get;}
}

public interface IFStarValue<T> : IFStarValue
{
    new T Value {get;}
}


public readonly struct FStarValue<T> : IFStarValue<T>
{
    public T Value {get;}

    public FStarValue(T value)
    {
        Value = value;
    }

    object? IFStarValue.Value => Value;
}


/**
A F* type is union of some .NET types.
*/

public interface IFStarType : IFStarValue<ITypeList>
{}

public interface IFStarType<TTypeList> : IFStarType
    where TTypeList : ITypeList
{
    new TTypeList Value {get;}
}

public readonly struct FStarType<TTypeList> : IFStarType<TTypeList>
    where TTypeList : unmanaged, ITypeList
{
    public TTypeList Value => new();

    ITypeList IFStarValue<ITypeList>.Value => Value;

    object? IFStarValue.Value => Value;
}

public static class FStarTypeTheory
{
    public static FStarType<EmptyTypeList> Empty => default;

    public static FStarType<TTypeList> Create<TTypeList>(TTypeList tl) 
        where TTypeList : unmanaged, ITypeList
        => new();

    public static HeadPremise<THead> IntroduceHead<THead>() => new();

    public readonly struct HeadPremise<THead>
    {
        public TailTypeListPremise<TTailTypeList> IntroduceTailTypeList<TTailTypeList>(TTailTypeList typeList) where TTailTypeList : unmanaged, ITypeList => new();

        public readonly struct TailTypeListPremise<TTailTypeList> where TTailTypeList : unmanaged, ITypeList
        {
            public FStarType<TypeList<THead, TTailTypeList>> Cons<TFStarKind>(TFStarKind kind)
                where TFStarKind : unmanaged, IFStarType<TTailTypeList>
            {
                return Create(TypeListTheory.IntroduceHead<THead>().Cons(kind.Value));
            }
        }
    }
}

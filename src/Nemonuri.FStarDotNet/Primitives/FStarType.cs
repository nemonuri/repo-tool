using System.Collections;

namespace Nemonuri.FStarDotNet.Primitives;

#region FStarValue

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

#endregion FStarValue


#region FStarType

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

#endregion FStarType


#region FStarEquatableType

public interface IFStarEquatableType : IFStarType
{
    System.Type ToDotNetType();

    IEqualityComparer GetEqualityComparer();

    IFStarValue? CreateFStarValue(object? value);
}

public interface IFStarEquatableType<TDotNetType, TEqualityComparer, TFStarValue> : IFStarEquatableType
    where TEqualityComparer : IEqualityComparer
    where TFStarValue : IFStarValue<TDotNetType>
{
    new TEqualityComparer GetEqualityComparer();

    TFStarValue CreateFStarValue(TDotNetType value);
}

public readonly struct FStarEquatableType<TDotNetType, TEqualityComparer> : IFStarEquatableType<TDotNetType, TEqualityComparer, FStarValue<TDotNetType>>
    where TEqualityComparer : IEqualityComparer
{
    private readonly TEqualityComparer _equalityComparer;

    public FStarEquatableType(TEqualityComparer equalityComparer)
    {
        _equalityComparer = equalityComparer;
    }

    public TEqualityComparer GetEqualityComparer() => _equalityComparer;

    public FStarValue<TDotNetType> CreateFStarValue(TDotNetType value) => new(value);

    public Type ToDotNetType() => typeof(TDotNetType);

    IEqualityComparer IFStarEquatableType.GetEqualityComparer() => GetEqualityComparer();

    IFStarValue? IFStarEquatableType.CreateFStarValue(object? value) => value is TDotNetType v ? CreateFStarValue(v) : null;

    ITypeList IFStarValue<ITypeList>.Value => new TypeList<TDotNetType, EmptyTypeList>();

    object? IFStarValue.Value => ((IFStarValue<ITypeList>)this).Value;
}

#endregion FStarEquatableType


public static class FStarTypeTheory
{
    public static FStarType<EmptyTypeList> Empty => default;

    public static FStarType<TTypeList> Create<TTypeList>(TTypeList tl) 
        where TTypeList : unmanaged, ITypeList
        => new();

    public static FStarType<TypeList<T, EmptyTypeList>> CreateSingleton<T>() => Create(TypeListTheory.CreateSingleton<T>());


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


    public static DotNetTypePremise<TDotNetType> IntroduceDotNetType<TDotNetType>() => new();

    public readonly struct DotNetTypePremise<TDotNetType>
    {
        public bool TrySpecialize(IFStarType? fstType, out FStarType<TypeList<TDotNetType, EmptyTypeList>> specialized)
        {
            if (TypeListTheory.TrySpecialize<TDotNetType>(fstType?.Value, out _))
            {
                specialized = new(); return true;
            }
            else
            {
                specialized = default; return false;
            }
        }

        
    }
}

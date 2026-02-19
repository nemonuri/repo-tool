using System.Collections;

namespace Nemonuri.FStarDotNet.Primitives;


#region FStarType

/**
A F* type is union of some .NET types.
*/

public interface IFStarType //: IFStarValue<ITypeList>
{
    ITypeList? GetDotNetTypes();
}

public interface IFStarType<TTypeList> : IFStarType
    where TTypeList : ITypeList
{
    //new TTypeList Value {get;}
    new TTypeList? GetDotNetTypes();
}

public readonly struct FStarType<TTypeList> : IFStarType<TTypeList>, ITypeList
    where TTypeList : unmanaged, ITypeList
{
    public TTypeList GetDotNetTypes() => new();

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();

    Type? ITypeList.GetHead() => GetDotNetTypes().GetHead();

    ITypeList? ITypeList.GetTail() => GetDotNetTypes().GetTail();
}

public interface ISolvedFStarType : IFStarType
{
    System.Type GetSolvedDotNetType();
}

public interface ISolvedFStarType<TDotNet> : ISolvedFStarType, IFStarType<TypeList<TDotNet, EmptyTypeList>>
{
}

public readonly struct SolvedFStarType<TDotNet> : ISolvedFStarType<TDotNet>, ITypeList
{
    public Type GetSolvedDotNetType() => FStarTypeTheory.GetSolvedDotNetType(this);

    public TypeList<TDotNet, EmptyTypeList> GetDotNetTypes() => FStarTypeTheory.GetDotNetTypes(this);

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();

    Type? ITypeList.GetHead() => GetDotNetTypes().GetHead();

    ITypeList? ITypeList.GetTail() => GetDotNetTypes().GetTail();
}

#endregion FStarType


#region FStarInstance

/** 
- In PTS(Pure type system), 'instance(or value)' is subtype of 'type'
*/

public interface IFStarInstance : IFStarType, ISolvedFStarType
{
    object? Value {get;}
}

public interface IFStarInstance<T> : IFStarInstance, ISolvedFStarType<T>
{
    new T Value {get;}
}


public readonly struct FStarInstance<T> : IFStarInstance<T>
{
    public T Value {get;}

    public FStarInstance(T value)
    {
        Value = value;
    }

    object? IFStarInstance.Value => Value;

    public TypeList<T, EmptyTypeList> GetDotNetTypes() => FStarTypeTheory.GetDotNetTypes(this);

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();

    public Type GetSolvedDotNetType() => FStarTypeTheory.GetSolvedDotNetType(this);
}

#endregion FStarInstance


#region FStarEquatableType


public interface IFStarEquatableType : ISolvedFStarType
{
    IEqualityComparer GetInstanceEqualityComparer();
}

public interface IFStarEquatableType<TDotNetType, TEqualityComparer> : IFStarEquatableType, ISolvedFStarType<TDotNetType>
    where TEqualityComparer : IEqualityComparer
{
    new TEqualityComparer GetInstanceEqualityComparer();
}

public readonly struct FStarEquatableType<TDotNetType, TEqualityComparer> : IFStarEquatableType<TDotNetType, TEqualityComparer>
    where TEqualityComparer : IEqualityComparer
{
    private readonly TEqualityComparer _equalityComparer;

    public FStarEquatableType(TEqualityComparer equalityComparer)
    {
        _equalityComparer = equalityComparer;
    }

    public TEqualityComparer GetInstanceEqualityComparer() => _equalityComparer;

    IEqualityComparer IFStarEquatableType.GetInstanceEqualityComparer() => GetInstanceEqualityComparer();

    public Type GetSolvedDotNetType() => FStarTypeTheory.GetSolvedDotNetType(this);

    public TypeList<TDotNetType, EmptyTypeList> GetDotNetTypes() => FStarTypeTheory.GetDotNetTypes(this);

    ITypeList? IFStarType.GetDotNetTypes() => GetDotNetTypes();
}

#endregion FStarEquatableType


#region FStarEquatableInstance

public interface IFStarEquatableInstance : IFStarInstance, IFStarEquatableType
{
}

public interface IFStarEquatableInstance<T, TEqualityComparer> : IFStarEquatableInstance, IFStarInstance<T>, IFStarEquatableType<T, TEqualityComparer>
    where TEqualityComparer : IEqualityComparer
{
}

public readonly struct FStarEquatableInstance<T, TEqualityComparer> : IFStarEquatableInstance<T, TEqualityComparer>
    where TEqualityComparer : IEqualityComparer
{
#if false
    public T Value {get;}

    private readonly TEqualityComparer _equalityComparer;

    public FStarEquatableInstance(T value, TEqualityComparer equalityComparer)
    {
        Value = value;
        _equalityComparer = equalityComparer;
    }

    public TEqualityComparer GetEqualityComparer() => _equalityComparer;

    IEqualityComparer IFStarEquatableInstance.GetEqualityComparer() => GetEqualityComparer();


    object? IFStarInstance.Value => Value;
#endif
    private readonly FStarEquatableType<T, TEqualityComparer> _fstarEquatableType;

    private readonly FStarInstance<T> _fstarInstance;

    public FStarEquatableInstance(T value, TEqualityComparer equalityComparer)
    {
        _fstarInstance = new(value);
        _fstarEquatableType = new(equalityComparer);
    }

    public T Value => _fstarInstance.Value;

    object? IFStarInstance.Value => Value;

    public TEqualityComparer GetInstanceEqualityComparer() => _fstarEquatableType.GetInstanceEqualityComparer();

    IEqualityComparer IFStarEquatableType.GetInstanceEqualityComparer() => GetInstanceEqualityComparer();

    public Type GetSolvedDotNetType() => _fstarInstance.GetSolvedDotNetType();

    public TypeList<T, EmptyTypeList> GetDotNetTypes() => _fstarInstance.GetDotNetTypes();

    ITypeList? IFStarType.GetDotNetTypes() => _fstarInstance.GetDotNetTypes();
}

#endregion FStarEquatableInstance



public static class FStarTypeTheory
{
    public static FStarInstance<T> CreateInstance<T>(ISolvedFStarType<T> solvedType, T dotNetInstance) => new(dotNetInstance);

    public static System.Type GetSolvedDotNetType<T>(ISolvedFStarType<T> solvedType) => typeof(T);

    public static TypeList<T, EmptyTypeList> GetDotNetTypes<T>(ISolvedFStarType<T> solvedType) => new();


    public static FStarType<EmptyTypeList> Empty => default;

    public static FStarType<TTypeList> CreateUnsolved<TTypeList>(TTypeList tl) 
        where TTypeList : unmanaged, ITypeList
        => new();

    public static SolvedFStarType<TDotNet> CreateSolved<TDotNet>() => new();


    public static HeadPremise<THead> IntroduceHead<THead>() => new();

    public readonly struct HeadPremise<THead>
    {
        public TailTypeListPremise<TTailTypeList> IntroduceTailTypeList<TTailTypeList>(TTailTypeList typeList) where TTailTypeList : unmanaged, ITypeList => new();

        public readonly struct TailTypeListPremise<TTailTypeList> where TTailTypeList : unmanaged, ITypeList
        {
            public FStarType<TypeList<THead, TTailTypeList>> Cons<TFStarKind>(TFStarKind kind)
                where TFStarKind : unmanaged, IFStarType<TTailTypeList>
            {
                return CreateUnsolved(TypeListTheory.IntroduceHead<THead>().Cons(kind.GetDotNetTypes()));
            }
        }
    }

    public static bool TrySolve<TDotNet>(IFStarType? fstType, out SolvedFStarType<TDotNet> solved)
    {
        if (TypeListTheory.TrySpecialize<TDotNet>(fstType?.GetDotNetTypes(), out _))
        {
            solved = new(); return true;
        }
        else
        {
            solved = default; return false;
        }
    }


    public static DotNetEquatablePremise<TDotNet> IntroduceDotNetEquatable<TDotNet>() where TDotNet : IEquatable<TDotNet>
        => new();

    public readonly struct DotNetEquatablePremise<TDotNet> where TDotNet : IEquatable<TDotNet>
    {
        public bool TrySolve(IFStarType? fstType, out SolvedFStarType<TDotNet> solved) => TrySolve<TDotNet>(fstType, out solved);

        public FStarEquatableType<TDotNet, EqualityComparer<TDotNet>> ToFStarEquatableType(ISolvedFStarType<TDotNet> solved) => 
            new(EqualityComparer<TDotNet>.Default);
    }


    public static DotNetStructuralEquatablePremise<TDotNet> IntroduceDotNetStructuralEquatable<TDotNet>() where TDotNet : IStructuralEquatable
        => new();

    public readonly struct DotNetStructuralEquatablePremise<TDotNet> where TDotNet : IStructuralEquatable
    {
        public bool TrySolve(IFStarType? fstType, out SolvedFStarType<TDotNet> solved) => TrySolve<TDotNet>(fstType, out solved);

        public FStarEquatableType<TDotNet, IEqualityComparer> ToFStarEquatableType(ISolvedFStarType<TDotNet> solved) => 
            new(StructuralComparisons.StructuralEqualityComparer);
    }

}

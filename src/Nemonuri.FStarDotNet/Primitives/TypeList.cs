using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace Nemonuri.FStarDotNet.Primitives;

public interface ITypeList
{
    Type? GetHead();

    Type? GetTail();
}

internal readonly struct TypeList : ITypeList
{
    private readonly Type? _first;

    private readonly Type? _second;


    public TypeList(Type? first, Type? second)
    {
        _first = first;
        _second = second;
    }

    public Type? GetHead() => _first;
    public Type? GetTail() => _second;

    public static TypeList Create<THead, TTail>() => new(typeof(THead), typeof(TTail));

    public static TypeList Create<THead>() => new(typeof(THead), null);
}

public readonly struct TypeList<THead, TTail> : ITypeList
{
    //private readonly static TypeList s_typePair = TypeList.Create<THead, TTail>();

    //Type? ITypeList.GetHead() => s_typePair.GetHead();

    //Type? ITypeList.GetTail() => s_typePair.GetTail();

    public Type? GetHead() => typeof(THead);

    public Type? GetTail() => typeof(TTail);
}

/*
public readonly struct TypeList<THead> : ITypeList
{
    private readonly static TypeListImpl s_typePair = TypeListImpl.Create<THead>();

    Type? ITypeList.GetHead() => s_typePair.GetHead();

    Type? ITypeList.GetTail() => s_typePair.GetTail();
}
*/

public readonly struct EmptyTypeList : ITypeList
{
    public Type? GetHead() => null;

    public Type? GetTail() => null;
}

public static class TypeListTheory
{
    public static bool TryGetTypeList(Type? type, [NotNullWhen(true)] out ITypeList? typeList)
    {
        if 
        (
            type is not null    &&
            type.IsValueType    &&
            Activator.CreateInstance(type) is ITypeList v
        )
            { typeList = v; }
        else
            { typeList = null; }

        return typeList is not null;
    }

    public static bool Contains<T>(ITypeList typeList) => Contains(typeList, typeof(T));

    public static bool Contains(ITypeList typeList, Type t)
    {
        Guard.IsNotNull(typeList);
        Guard.IsNotNull(t);
        if (t == typeList.GetHead()) { return true; }

        var tail = typeList.GetTail();
        if (TryGetTypeList(typeList.GetTail(), out var tailAsTypePair))
        {
            return Contains(tailAsTypePair, t);
        }

        return t == tail;
    }

    public static bool IsSingleton(ITypeList typeList)
    {
        Guard.IsNotNull(typeList);
        return typeList.GetHead() is not null && typeList.GetTail() is null;
    }

    public static EmptyTypeList Empty => default;

    public static TypeList
}


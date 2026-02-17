using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace Nemonuri.FStarDotNet.Primitives;

public interface ITypeList
{
    Type GetHead();

    Type GetTail();
}

public readonly struct TypePair : ITypeList
{
    private readonly Type? _first;

    private readonly Type? _second;


    public TypePair(Type? first, Type? second)
    {
        _first = first;
        _second = second;
    }

    public Type GetHead() => TypePairTheory.ReturnVoidIfNull(_first);
    public Type GetTail() => TypePairTheory.ReturnVoidIfNull(_second);

    public static TypePair Create<T1, T2>() => new(typeof(T1), typeof(T2));

    public static TypePair Create<T1>() => new(typeof(T1), TypeConstants.Void);
}

public readonly struct TypePair<T1, T2> : ITypeList
{
    private readonly static TypePair s_typePair = TypePair.Create<T1, T2>();

    Type ITypeList.GetHead() => s_typePair.GetHead();

    Type ITypeList.GetTail() => s_typePair.GetTail();
}

public readonly struct TypePair<T1> : ITypeList
{
    private readonly static TypePair s_typePair = TypePair.Create<T1>();

    Type ITypeList.GetHead() => s_typePair.GetHead();

    Type ITypeList.GetTail() => s_typePair.GetTail();
}

public static class TypePairTheory
{
    internal static Type ReturnVoidIfNull(Type? type) => type ?? TypeConstants.Void;

    public static bool TryGetTypePair(Type type, [NotNullWhen(true)] out ITypeList? typePair)
    {
        Guard.IsNotNull(type);
        if (type != TypeConstants.Void && type.IsValueType && Activator.CreateInstance(type) is ITypeList v)
            { typePair = v; }
        else
            { typePair = null; }
        return typePair is not null;
    }

    public static Type GetFirstOrVoid(Type type) => TryGetTypePair(type, out var typePair) ? typePair.GetHead() : TypeConstants.Void;

    public static Type GetSecondOrVoid(Type type) => TryGetTypePair(type, out var typePair) ? typePair.GetTail() : TypeConstants.Void;

    public static bool Contains(ITypeList typePair, Type t)
    {
        Guard.IsNotNull(typePair);
        Guard.IsNotNull(t);
        if (t == TypeConstants.Void) { return false; }
        if (typePair.GetHead() == t) { return true; }

        var tail = typePair.GetTail();
        if (!TryGetTypePair(typePair.GetTail(), out var tailAsTypePair))
        {
            return tail == TypeConstants.Void;
        }

        return Contains(tailAsTypePair, t);
    }
}


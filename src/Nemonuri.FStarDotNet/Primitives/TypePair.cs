namespace Nemonuri.FStarDotNet.Primitives;

public interface ITypePair
{
    Type GetFirst();

    Type GetSecond();
}

public readonly struct TypePair<T1, T2> : ITypePair
{
    private readonly static TypePair s_typePair = TypePair.Create<T1, T2>();

    Type ITypePair.GetFirst() => s_typePair.GetFirst();

    Type ITypePair.GetSecond() => s_typePair.GetSecond();
}

public readonly struct TypePair : ITypePair
{
    private readonly Type? _first;

    private readonly Type? _second;

    private static Type Return(Type? type) => type ?? TypeConstants.ValueUnit;

    private static void ThrowIfVoid(Type? type)
    {
        if (type == null) {return;}
        if (type == TypeConstants.Void)
        {
            throw new NotSupportedException();
        }
    }

    public TypePair(Type? first, Type? second)
    {
        ThrowIfVoid(first);
        ThrowIfVoid(second);
        _first = first;
        _second = second;
    }

    public Type GetFirst() => Return(_first);
    public Type GetSecond() => Return(_second);

    public static TypePair Create<T1, T2>() => new(typeof(T1), typeof(T2));
}

public static class TypePairTheory
{
    internal static Type GetFirst(Type type)
    {
        if 
        (
            type.IsValueType &&
            Activator.CreateInstance(type) is ITypePair typePair
        )
        {
            return typePair.GetFirst();
        }
        
        return TypeConstants.ValueUnit;
    }

    public static bool IsTypePair(Type type)
    {
        return 
            type.IsGenericType &&
            (type.GetGenericTypeDefinition() == typeof(TypePair<,>));
    }

    public static bool IsTypePair<T>() => Per<T>.IsTypePair;

    public static void GuardIsTypePair(Type type)
    {
        CommunityToolkit.Diagnostics.Guard.IsTrue(IsTypePair(type));
    }

    public static Type GetFirstElement(Type typePair)
    {
        GuardIsTypePair(typePair);
        return typePair.GetGenericArguments()[0];
    }

    public static Type GetSecondElement(Type typePair)
    {
        GuardIsTypePair(typePair);
        return typePair.GetGenericArguments()[1];
    }

    public static bool Contains(Type typePair, Type t)
    {
        GuardIsTypePair(typePair);
        if (GetFirstElement(typePair).IsAssignableFrom(t))
        {
            return true;
        }
        var tail = 
    }

    private static class Per<T>
    {
        public static bool IsTypePair {get;} = IsTypePair(typeof(T));
    }
}

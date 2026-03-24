namespace Nemonuri.PureTypeSystems.Primitives;

public class Invalid
{
    public Invalid()
    {
        throw new InvalidOperationException(String.Format("Type '{0}' cannot be instantiated.", typeof(Invalid)));
    }

    public static void Throw()
    {
        throw new InvalidOperationException(String.Format("Type '{0}' cannot be used.", typeof(Invalid)));
    }

    public static T Throw<T>()
    {
        throw new InvalidOperationException(String.Format("Type '{0}' cannot be used.", typeof(Invalid)));
    }

    public override bool Equals(object? obj) => Throw<bool>();

    public override int GetHashCode() => Throw<int>();

    public override string ToString() => Throw<string>();
}
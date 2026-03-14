namespace Nemonuri.PureTypeSystems.Primitives;

public readonly struct Bracket<T>(T value) : IFixedBracket<T, Bracket<T>>
{
    public T Value {get;} = value;

    Bracket<T> IFixedBracket<T, Bracket<T>>.ToFixedBraket() => this;

    Bracket<T> IBracket<T>.ToBraket() => this;
}

public interface IBracket<T>
{
    Bracket<T> ToBraket();
}

public interface IFixedBracket<T, TFixed> : IBracket<T>
    where TFixed : IFixedBracket<T, TFixed>
{
    TFixed ToFixedBraket();
}

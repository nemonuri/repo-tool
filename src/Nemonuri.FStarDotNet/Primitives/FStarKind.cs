
namespace Nemonuri.FStarDotNet.Primitives;


public interface IFStarKind : IFStarValue
{
    IFStarValue? GetHead();

    IFStarKind? GetTail();
}

public readonly struct EmptyFStarKind : IFStarKind
{
    public IFStarValue? GetHead() => null;

    public IFStarKind? GetTail() => null;

    object? IFStarValue.Value => null;
}

public readonly struct FStarKindFunction<THead, TTail> : IFStarKind
    where THead : unmanaged, IFStarValue
    where TTail : unmanaged, IFStarKind
{
    public IFStarValue? GetHead() => new THead();

    public IFStarKind? GetTail() => (typeof(TTail) == typeof(EmptyFStarKind)) ? null : new TTail();

    object? IFStarValue.Value => throw new NotImplementedException();
}

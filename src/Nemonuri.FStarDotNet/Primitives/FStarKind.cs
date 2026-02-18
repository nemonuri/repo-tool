
namespace Nemonuri.FStarDotNet.Primitives;


public interface IFStarKind
{
    
}

public interface IFStarKind<THead, TTail, TFStarType> : IFStarKind
    where THead : IFStarValue?
    where TFStarType : IFStarType
{
    
}

public readonly struct UnitFStarKind<TType> : IFStarKind<IFStarValue?, TType>
    where TType : IFStarType?
{

}

public readonly struct FStarKindFunction<THead, TTail> : IFStarKind
    where THead : unmanaged, IFStarValue
    where TTail : unmanaged, IFStarKind
{
    public IFStarValue? GetHead() => new THead();

    public IFStarKind? GetTail() => (typeof(TTail) == typeof(EmptyFStarKind)) ? null : new TTail();

    object? IFStarValue.Value => throw new NotImplementedException();
}

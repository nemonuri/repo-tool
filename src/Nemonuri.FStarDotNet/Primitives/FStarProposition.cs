
namespace Nemonuri.FStarDotNet.Primitives;

public readonly struct FStarProposition<TFullFStarType> : IFStarUnit, IFStarType, IComparable
{
    public Microsoft.FSharp.Core.Unit? Value => null;

    object? IFStarValue.Value => Value;

    /// <summary>
    /// Get non-erased F* type.
    /// </summary>
    public IFStarType ToFullFStarType() => FStarTypeTheory.CreateSingleton<TFullFStarType>();

    ITypeList IFStarValue<ITypeList>.Value => ToFullFStarType().Value;

/**
- Reference: https://github.com/dotnet/fsharp/blob/c4ccf58cd107213374237e3910288d4b6b183abb/src/FSharp.Core/prim-types.fs#L25
*/
    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj switch
    {
        null => true,
        Microsoft.FSharp.Core.Unit => true,
        IFStarUnit => true,
        _ => false
    };

    public int CompareTo(object? obj) => 0;
}

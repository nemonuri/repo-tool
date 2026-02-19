
namespace Nemonuri.FStarDotNet.Primitives;

#if false
public readonly struct FStarProposition<TFullFStarType> : IFStarInstance, IFStarType, IComparable
{
    public Microsoft.FSharp.Core.Unit? Value => null;

    object? IFStarInstance.Value => Value;

    /// <summary>
    /// Get non-erased F* type.
    /// </summary>
    public IFStarType ToFullFStarType() => FStarTypeTheory.CreateSolved<TFullFStarType>();

    ITypeList IFStarInstance<ITypeList>.Value => ToFullFStarType().Value;

/**
- Reference: https://github.com/dotnet/fsharp/blob/c4ccf58cd107213374237e3910288d4b6b183abb/src/FSharp.Core/prim-types.fs#L25
*/
    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj switch
    {
        null => true,
        Microsoft.FSharp.Core.Unit => true,
        IFStarInstance => true,
        _ => false
    };

    public int CompareTo(object? obj) => 0;
}
#endif
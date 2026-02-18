namespace Nemonuri.FStarDotNet.Primitives;

public class FStarUnit : IFStarValue, IFStarType, IComparable
{
    public static FStarUnit Shared {get;} = new();

    public FStarUnit() {}

    public object? Value => null;

    /// <summary>
    /// Get non-erased F* type.
    /// </summary>
    public virtual IFStarType ToFullFStarType() => FStarTypeTheory.Empty;

    ITypeList IFStarValue<ITypeList>.Value => ToFullFStarType().Value;

/**
- Reference: https://github.com/dotnet/fsharp/blob/c4ccf58cd107213374237e3910288d4b6b183abb/src/FSharp.Core/prim-types.fs#L25
*/
    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj switch
    {
        null => true,
        FStarUnit => true,
        _ => false
    };

    public int CompareTo(object? obj) => 0;
}

public class FStarProposition<T> : FStarUnit
{
    public override IFStarType ToFullFStarType() => FStarTypeTheory.CreateSingleton<T>();
}

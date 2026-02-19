namespace Nemonuri.FStarDotNet.Primitives;

public interface IFStarRefinement<T> where T : IFStarType
{
}

public static class FStarRefinementTheory
{
    public static T GetRefinement<T>(IFStarRefinement<T> refinement) where T : IFStarType, new() => new();
}

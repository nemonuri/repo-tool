using Microsoft.FSharp.Core;
using Microsoft.FSharp.Quotations;

namespace Nemonuri.FStarDotNet.Primitives;

public interface IRefinement<T> where T : IFStarValue
{
    FSharpExpr<FSharpFunc<T, IFStarValue?>> Condition {get;}
}

public readonly struct Refinement<T> : IRefinement<T> where T : IFStarValue
{
    public Refinement(FSharpExpr<FSharpFunc<T, IFStarValue?>> condition)
    {
        Condition = condition;
    }

    public FSharpExpr<FSharpFunc<T, IFStarValue?>> Condition {get;}
}
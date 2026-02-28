namespace Nemonuri.FStarDotNet.Primitives;

public readonly unsafe struct UnsafeBijection<TSource, TTarget>
{
    public UnsafeBijection(delegate*<TSource, TTarget> pure, delegate*<TTarget, TSource> extract)
    {
        Pure = pure;
        Extract = extract;
    }

    public static UnsafeBijection<TSource, TTarget> Default {get;set;}

    delegate*<TSource, TTarget> Pure {get;}
    delegate*<TTarget, TSource> Extract {get;}

    public bool IsAnyMemberNull => Pure == null || Extract == null;
}

public record Bijection<TSource, TTarget>(Func<TSource, TTarget> Pure, Func<TTarget, TSource> Extract)
{
    public static Bijection<TSource, TTarget>? Default {get;set;}
}

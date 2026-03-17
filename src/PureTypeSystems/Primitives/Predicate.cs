namespace Nemonuri.PureTypeSystems.Primitives;

public interface IPredicatePremise<T>
{
    bool Judge(in T? arg);
}

public readonly struct PredicateBasedRefiner<T, TPredicate> : IRefinerPremise<T>
    where TPredicate : unmanaged, IPredicatePremise<T>
{
    public Judgement Judge(in T? pre, out T? post)
    {
        if ((new TPredicate()).Judge(in pre))
        {
            post = pre;
            return Judgement.True;
        }
        else
        {
            post = default;
            return Judgement.False;
        }
    }
}

public static class PredicateTheory
{
    public static Judgement JudgeAsRefiner<T, TPredicate>(in TPredicate predicate, in T? pre, out T? post)
        where TPredicate : IPredicatePremise<T>
    {
        if (predicate.Judge(in pre))
        {
            post = pre;
            return Judgement.True;
        }
        else
        {
            post = default;
            return Judgement.False;
        }
    }
}
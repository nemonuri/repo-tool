namespace Nemonuri.PureTypeSystems.Primitives;

#if false
public interface IArrowPairPremise<TAntecedent, TConsequent> : 
    IArrowPremise<TAntecedent, TConsequent>
{
    TAntecedent ContraApply(in TConsequent post);
}

public interface IArrowPairPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge, TContraPreJudge, TContraPostJudge> :
    IArrowPairPremise<TAntecedent, TConsequent>,
    IArrowPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge> 
    where TPreJudge : IJudgePremise
    where TPostJudge : IJudgePremise
    where TContraPreJudge : IJudgePremise
    where TContraPostJudge : IJudgePremise
{
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct ArrowHandlePair<TAnt, TCon>
{
    private readonly ArrowHandle<TAnt, TCon> _handle;

    private readonly ArrowHandle<TCon, TAnt> _contraHandle;

    internal ArrowHandlePair(ArrowHandle<TAnt, TCon> handle, ArrowHandle<TCon, TAnt> contraHandle)
    {
        _handle = handle;
        _contraHandle = contraHandle;
    }

    public ArrowHandle<TAnt, TCon> Handle => _handle;

    public ArrowHandle<TCon, TAnt> ContraHandle => _contraHandle;
}

public readonly struct ContraArrow<TAnt, TCon, TImplyPair> : IArrowPremise<TCon, TAnt>
    where TImplyPair : unmanaged, IArrowPairPremise<TAnt, TCon>
{
    public TAnt Apply(in TCon pre)
    {
        return (new TImplyPair()).ContraApply(in pre);
    }
}


public readonly struct IdentityPair<T> : IArrowPairPremise<T, Tautology, T, Tautology, Tautology, Tautology>
{
    public static T Apply(in T pre) => Identity<T>.Apply(in pre);

    public static T ContraApply(in T post) => Apply(in post);

    T IArrowPairPremise<T, T>.ContraApply(in T post) => ContraApply(in post);

    T IArrowPremise<T, T>.Apply(in T pre) => Apply(in pre);
}

public readonly struct FailurePair<TAnt, TCon> : 
    IArrowPairPremise<TAnt, Tautology, TCon, Negation, Tautology, Negation>
{
    public static TCon Apply(in TAnt ant) => Failure<TAnt, TCon>.Apply(in ant);

    public static TAnt ContraApply(in TCon con) => Failure<TCon, TAnt>.Apply(in con);

    TAnt IArrowPairPremise<TAnt, TCon>.ContraApply(in TCon post) => ContraApply(in post);

    TCon IArrowPremise<TAnt, TCon>.Apply(in TAnt pre) => Apply(in pre);
}
#endif
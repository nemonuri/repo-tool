using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public static class ArrowPairTheory
{
    private static ArrowHandle<TConsequent, TAntecedent> ToContraHandle<TAntecedent, TConsequent, TImplyPair>()
        where TImplyPair : unmanaged, IArrowPairPremise<TAntecedent, TConsequent>
    {
        return ArrowTheory.ToHandle<TConsequent, TAntecedent, ContraArrow<TAntecedent, TConsequent, TImplyPair>>();
    }

    extension<TAntecedent, TConsequent, TImplyPair>(TImplyPair)
        where TImplyPair : unmanaged, IArrowPairPremise<TAntecedent, TConsequent>
    {
        public static ArrowHandlePair<TAntecedent, TConsequent> ToHandlePair()
        {
            return new
            (
                ArrowTheory.ToHandle<TAntecedent, TConsequent, TImplyPair>(),
                ToContraHandle<TAntecedent, TConsequent, TImplyPair>()
            );
        }

        public static ArrowHandlePair<T1, T2> ToTypeEqualHandlePair<T1, T2>()
        {
            if (!(typeof(T1) == typeof(TAntecedent) && typeof(T2) == typeof(TConsequent)))
            {
                throw new InvalidCastException(/* TODO */);
            }

            var original = ToHandlePair<TAntecedent, TConsequent, TImplyPair>();
            return Unsafe.As<ArrowHandlePair<TAntecedent, TConsequent>,ArrowHandlePair<T1, T2>>(ref original);
        }
    }

    extension<TAntecedent, TPreJudge, TConsequent, TPostJudge, TContraPreJudge, TContraPostJudge, TImplyPair>(TImplyPair)
        where TPreJudge : unmanaged, IJudgePremise
        where TPostJudge : unmanaged, IJudgePremise
        where TContraPreJudge : unmanaged, IJudgePremise
        where TContraPostJudge : unmanaged, IJudgePremise
        where TImplyPair : unmanaged, IArrowPairPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge, TContraPreJudge, TContraPostJudge>
    {
        public static ArrowHandlePair<TAntecedent, TConsequent> ToHandlePair()
        {
            var handle = ArrowTheory.ToHandle<TAntecedent, TPreJudge, TConsequent, TPostJudge, TImplyPair>();
            var contraHandle = ToContraHandle<TAntecedent, TConsequent, TImplyPair>()
                                .WithJudges
                                (
                                    JudgeTheory.FreeToHandle<TContraPreJudge, TConsequent>(),
                                    JudgeTheory.FreeToHandle<TContraPostJudge, (TConsequent, TAntecedent)>()
                                );
            return new(handle, contraHandle);
        }
    }

    public static ArrowHandlePair<TP, TQ> GetFailureHandlePair<TP, TQ>()
    {
        return ToHandlePair<TP, Tautology, TQ, Negation, Tautology, Negation, FailurePair<TP, TQ>>();
    }
}

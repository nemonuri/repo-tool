using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;


public static class UnthunkerTheory
{
    extension<TCon, TIntro>(TIntro)
        where TIntro : unmanaged, IIntroducerPremise<JudgeHandle<TCon>>
    {
        public static ArrowHandle<TAnt, JudgeHandle<TCon>> Unthunk<TAnt>(JudgeHandle<TCon> hint) => (new TIntro()).Introduce<TAnt>(hint);
    }

    extension<THead, TTail, TIntro>(TIntro)
        where TIntro : unmanaged, IIntroducerPremise<ArrowHandle<THead, TTail>>
    {
        public static ArrowHandle<TAnt, ArrowHandle<THead, TTail>> Unthunk<TAnt>(in ArrowHandle<THead, TTail> hint) => (new TIntro()).Introduce<TAnt>(in hint);
    }

    public static ArrowHandle<TP, JudgeHandle<TQ>> Unthunk<TP, TQ>(IIntroducerPremise<JudgeHandle<TQ>> intro, JudgeHandle<TQ> hint)
    {
        return intro.Introduce<TP>(hint);
    }

    public static ArrowHandle<TP, ArrowHandle<THead, TTail>> Unthunk<TP, THead, TTail>(IIntroducerPremise<ArrowHandle<THead, TTail>> intro, in ArrowHandle<THead, TTail> hint)
    {
        return intro.Introduce<TP>(in hint);
    }

    public static bool TryUnthunk<TP, TQ>(object? unthunker, JudgeHandle<TQ> hint, out ArrowHandle<TP, JudgeHandle<TQ>> handle)
    {
        if (unthunker is IIntroducerPremise<JudgeHandle<TQ>> premise)
        {
            handle = premise.Introduce<TP>(hint);
            return true;
        }
        else
        {
            handle = default;
            return false;
        }
    }
}

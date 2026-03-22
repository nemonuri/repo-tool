using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;


public static class UnthunkerTheory
{
#if false
    extension<T, TArrow>(TArrow)
        where TArrow : unmanaged, IArrowPremise<ValueUnit, JudgeHandle<T>>
    {
        public static JudgeHandle<T> Unthunk() => ConstantTheory.ToHandle<JudgeHandle<T>, TArrow>().Value;
    }

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
#endif

    public static JudgeHandle<T> Unthunk<T>(IArrowPremise<ValueUnit, JudgeHandle<T>> arrow)
    {
        return arrow.Apply(ValueUnitTheory.Singleton);
    }

    public static ArrowHandle<TP, JudgeHandle<TQ>> Unthunk<TP, TQ>(IIntroducerPremise<JudgeHandle<TQ>> intro, JudgeHandle<TQ> hint)
    {
        return intro.Introduce<TP>(hint);
    }

    public static ArrowHandle<TP, ArrowHandle<THead, TTail>> Unthunk<TP, THead, TTail>(IIntroducerPremise<ArrowHandle<THead, TTail>> intro, in ArrowHandle<THead, TTail> hint)
    {
        return intro.Introduce<TP>(in hint);
    }

    public static bool TryUnthunk<T>(object? unthunker, out JudgeHandle<T> handle)
    {
        if (unthunker is IArrowPremise<ValueUnit, JudgeHandle<T>> arrow)
        {
            handle = Unthunk(arrow);
            return true;
        }
        else
        {
            handle = default;
            return false;
        }
    }

    public static bool TryUnthunk<TP, TQ>(object? unthunker, JudgeHandle<TQ> hint, out ArrowHandle<TP, JudgeHandle<TQ>> handle)
    {
        if (unthunker is IIntroducerPremise<JudgeHandle<TQ>> intro)
        {
            handle = Unthunk<TP, TQ>(intro, hint);
            return true;
        }
        else
        {
            handle = default;
            return false;
        }
    }

    public static bool TryUnthunk<TP, THead, TTail>(object? unthunker, in ArrowHandle<THead, TTail> hint, out ArrowHandle<TP, ArrowHandle<THead, TTail>> handle)
    {
        if (unthunker is IIntroducerPremise<ArrowHandle<THead, TTail>> intro)
        {
            handle = Unthunk<TP, THead, TTail>(intro, hint);
            return true;
        }
        else
        {
            handle = default;
            return false;
        }
    }
}

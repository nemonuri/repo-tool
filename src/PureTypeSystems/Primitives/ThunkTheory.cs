using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;


public static class ThunkTheory
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
}

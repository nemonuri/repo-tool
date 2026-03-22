namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IIntroducer<TCon>
{
    ArrowHandle<TAnt, TCon> Introduce<TAnt>(in TCon hint);
}

#if false
public static class IntroducerTheory
{
    extension<TCon, TSpec>(TSpec)
        where TSpec : IIntroducerPremise<JudgeHandle<TCon>>
    {
        public static ArrowHandle<TAnt, JudgeHandle<TCon>> Introduce<TAnt>(JudgeHandle<TCon> hint) => (new TSpec()).Introduce<TAnt>(hint);
    }

    extension<THead, TTail, TSpec>(TSpec)
        where TSpec : IIntroducerPremise<ArrowHandle<THead, TTail>>
    {
        public static ArrowHandle<TAnt, ArrowHandle<THead, TTail>> Introduce<TAnt>(in ArrowHandle<THead, TTail> hint) => (new TSpec()).Introduce<TAnt>(in hint);
    }
}

public unsafe readonly struct IntroducerHandle<TCon, TAnt> : IHandle
{
    private readonly delegate*<in TCon, ArrowHandle<TAnt, TCon>> _fp;

    internal IntroducerHandle(delegate*<in TCon, ArrowHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ArrowHandle<TAnt, TCon> Introduce(in TCon hint) => _fp(in hint);
}
#endif
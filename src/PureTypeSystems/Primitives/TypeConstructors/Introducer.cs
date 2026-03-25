namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IIntroducer<TQ>
{
    bool TryIntroduce<TP>(in TQ hint, [NotNullWhen(true)] out IArrow<TP, TQ>? arrow);
}

#if false
public static class IntroducerTheory
{
    extension<TQ, TIntro>(TIntro)
        where TIntro : IIntroducer<TQ>
    {
        public static MethodHandle<TIntro, TP, TQ> IntroduceMethod<TP>(in TIntro self, in TQ hint) 
        {
            return new(self.Introduce<(TIntro, TP)>(in hint));
        }
    }
}

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
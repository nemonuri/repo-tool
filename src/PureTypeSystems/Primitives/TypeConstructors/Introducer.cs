namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IIntroducerPremise<TCon>
{
    ImplyHandle<TAnt, TCon> Introduce<TAnt>(in TCon hint);
}

public static class IntroducerTheory
{
    extension<TCon, TSpec>(TSpec)
        where TSpec : unmanaged, IIntroducerPremise<JudgeHandle<TCon>>
    {
        public static ImplyHandle<TAnt, JudgeHandle<TCon>> Introduce<TAnt>(JudgeHandle<TCon> hint) => (new TSpec()).Introduce<TAnt>(hint);
    }

    extension<THead, TTail, TSpec>(TSpec)
        where TSpec : unmanaged, IIntroducerPremise<ImplyHandle<THead, TTail>>
    {
        public static ImplyHandle<TAnt, ImplyHandle<THead, TTail>> Introduce<TAnt>(in ImplyHandle<THead, TTail> hint) => (new TSpec()).Introduce<TAnt>(in hint);
    }
}

public unsafe readonly struct IntroducerHandle<TCon, TAnt> : IHandle
{
    private readonly delegate*<in TCon, ImplyHandle<TAnt, TCon>> _fp;

    internal IntroducerHandle(delegate*<in TCon, ImplyHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ImplyHandle<TAnt, TCon> Introduce(in TCon hint) => _fp(in hint);
}

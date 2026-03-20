namespace Nemonuri.PureTypeSystems.Primitives;

public interface ISpecializerPremise<TCon>
{
    ArrowHandle<TAnt, TCon> Specialize<TAnt>(in TCon con);
}

public static class SpecializerTheory
{
    extension<TCon, TSpec>(TSpec)
        where TSpec : unmanaged, ISpecializerPremise<JudgeHandle<TCon>>
    {
        public static ArrowHandle<TAnt, JudgeHandle<TCon>> Specialize<TAnt>(JudgeHandle<TCon> con) => (new TSpec()).Specialize<TAnt>(con);
    }

    extension<THead, TTail, TSpec>(TSpec)
        where TSpec : unmanaged, ISpecializerPremise<ArrowHandle<THead, TTail>>
    {
        public static ArrowHandle<TAnt, ArrowHandle<THead, TTail>> Specialize<TAnt>(in ArrowHandle<THead, TTail> con) => (new TSpec()).Specialize<TAnt>(in con);
    }
}

public unsafe readonly struct SpecializerHandle<TCon, TAnt> : IHandle
{
    private readonly delegate*<in TCon, ArrowHandle<TAnt, TCon>> _fp;

    internal SpecializerHandle(delegate*<in TCon, ArrowHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ArrowHandle<TAnt, TCon> Specialize(in TCon con) => _fp(in con);
}
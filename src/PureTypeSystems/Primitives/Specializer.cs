namespace Nemonuri.PureTypeSystems.Primitives;

public interface ISpecializerPremise<TCon>
{
    ImplyHandle<TAnt, TCon> Specialize<TAnt>(in TCon con);
}

public static class SpecializerTheory
{
    extension<TCon, TSpec>(TSpec)
        where TSpec : unmanaged, ISpecializerPremise<JudgeHandle<TCon>>
    {
        public static ImplyHandle<TAnt, JudgeHandle<TCon>> Specialize<TAnt>(JudgeHandle<TCon> con) => (new TSpec()).Specialize<TAnt>(con);
    }

    extension<THead, TTail, TSpec>(TSpec)
        where TSpec : unmanaged, ISpecializerPremise<ImplyHandle<THead, TTail>>
    {
        public static ImplyHandle<TAnt, ImplyHandle<THead, TTail>> Specialize<TAnt>(in ImplyHandle<THead, TTail> con) => (new TSpec()).Specialize<TAnt>(in con);
    }
}

public unsafe readonly struct SpecializerHandle<TCon, TAnt> : IHandle
{
    private readonly delegate*<in TCon, ImplyHandle<TAnt, TCon>> _fp;

    internal SpecializerHandle(delegate*<in TCon, ImplyHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ImplyHandle<TAnt, TCon> Specialize(in TCon con) => _fp(in con);
}
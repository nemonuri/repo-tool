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

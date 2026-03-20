namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IEliminatorPremise<TAnt>
{
    ImplyHandle<TAnt, TCon> ToImplyHandle<TCon>(in TAnt hint);
}


public unsafe readonly struct EliminatorHandle<TAnt, TCon> : IHandle
{
    private readonly delegate*<in TAnt, ImplyHandle<TAnt, TCon>> _fp;

    internal EliminatorHandle(delegate*<in TAnt, ImplyHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ImplyHandle<TAnt, TCon> ToImplyHandle(in TAnt hint) => _fp(in hint);
}

public static class EliminatorTheory
{
    extension<TAnt, TEliminator>(TEliminator)
        where TEliminator : unmanaged, IEliminatorPremise<TAnt>
    {
        public unsafe static EliminatorHandle<TAnt, TCon> ToHandle<TCon>()
        {
            static ImplyHandle<TAnt, TCon> Impl(in TAnt hint) => (new TEliminator()).ToImplyHandle<TCon>(in hint);

            return new(&Impl);
        }
    }

    public static bool TryEliminate<TAnt, TCon>(EliminatorHandle<TAnt, TCon> eliminator, in TAnt subject, [NotNullWhen(true)] out TCon? result)
    {
        var impHnd = eliminator.ToImplyHandle(in subject);
        return ImplyTheory.TryApplyTrue(in impHnd, in subject, out result, out _);
    }
}

namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IEliminatorPremise<TAnt>
{
    ArrowHandle<TAnt, TCon> ToArrowHandle<TCon>(in TAnt hint);
}


public unsafe readonly struct EliminatorHandle<TAnt, TCon> : IHandle
{
    private readonly delegate*<in TAnt, ArrowHandle<TAnt, TCon>> _fp;

    internal EliminatorHandle(delegate*<in TAnt, ArrowHandle<TAnt, TCon>> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public ArrowHandle<TAnt, TCon> ToImplyHandle(in TAnt hint) => _fp(in hint);
}

public static class EliminatorTheory
{
    extension<TAnt, TEliminator>(TEliminator)
        where TEliminator : unmanaged, IEliminatorPremise<TAnt>
    {
        public unsafe static EliminatorHandle<TAnt, TCon> ToHandle<TCon>()
        {
            static ArrowHandle<TAnt, TCon> Impl(in TAnt hint) => (new TEliminator()).ToArrowHandle<TCon>(in hint);

            return new(&Impl);
        }

        public static TCon Eliminate<TCon>(in TAnt ant)
        {
            var elim = ToHandle<TAnt, TEliminator, TCon>();
            return Eliminate(elim, in ant);
        }
    }

    public static TCon Eliminate<TAnt, TCon>(EliminatorHandle<TAnt, TCon> eliminator, in TAnt subject)
    {
        var impHnd = eliminator.ToImplyHandle(in subject);
        return impHnd.Apply(in subject);
    }

    public static bool TryEliminate<TAnt, TCon>(EliminatorHandle<TAnt, TCon> eliminator, in TAnt subject, [NotNullWhen(true)] out TCon? result)
    {
        var impHnd = eliminator.ToImplyHandle(in subject);
        return ArrowTheory.TryApplyTrue(in impHnd, in subject, out result, out _);
    }
}

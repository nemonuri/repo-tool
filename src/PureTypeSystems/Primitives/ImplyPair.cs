namespace Nemonuri.PureTypeSystems.Primitives;

public interface IImplyPairPremise<TAntecedent, TConsequent> : 
    IImplyPremise<TAntecedent, TConsequent>
{
    TAntecedent ContraApply(in TConsequent post);
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct ImplyHandlePair<TAnt, TCon>
{
    private readonly ImplyHandle<TAnt, TCon> _handle;

    private readonly ImplyHandle<TCon, TAnt> _contraHandle;

    internal ImplyHandlePair(ImplyHandle<TAnt, TCon> handle, ImplyHandle<TCon, TAnt> contraHandle)
    {
        _handle = handle;
        _contraHandle = contraHandle;
    }

    public ImplyHandle<TAnt, TCon> Handle => _handle;

    public ImplyHandle<TCon, TAnt> ContraHandle => _contraHandle;
}

public readonly struct ContraImply<TAnt, TCon, TImplyPair> : IImplyPremise<TCon, TAnt>
    where TImplyPair : unmanaged, IImplyPairPremise<TAnt, TCon>
{
    public TAnt Apply(in TCon pre)
    {
        return (new TImplyPair()).ContraApply(in pre);
    }
}

namespace Nemonuri.PureTypeSystems.Primitives;

public static class ImplyPairTheory
{
    extension<TAnt, TCon, TImplyPair>(TImplyPair)
        where TImplyPair : unmanaged, IImplyPairPremise<TAnt, TCon>
    {
        
    }
}

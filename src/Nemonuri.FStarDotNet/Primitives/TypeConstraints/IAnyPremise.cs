namespace Nemonuri.FStarDotNet.Primitives.TypeConstraints;

public interface IAnyPremise<TFrame, THead, TTail> : IPremise<TFrame>
    where TFrame : unmanaged, IAnyPremise<TFrame, THead, TTail>
{
    
}
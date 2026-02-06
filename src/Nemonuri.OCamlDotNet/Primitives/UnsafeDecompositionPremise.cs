namespace Nemonuri.OCamlDotNet;

public unsafe readonly struct UnsafeDecompositionPremise<TComposed, TDecomposed>
{
    public readonly delegate*<ref TComposed, int> GetLength;
    public readonly delegate*<ref TComposed, int, ref TDecomposed> GetItemRef;

    public UnsafeDecompositionPremise(delegate*<ref TComposed, int> getLength, delegate*<ref TComposed, int, ref TDecomposed> getItemRef)
    {
        GetLength = getLength;
        GetItemRef = getItemRef;
    }

    public bool IsAnyMemberNull => GetLength == null || GetItemRef == null;
}


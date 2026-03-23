using Nemonuri.PureTypeSystems.Primitives.Extensions;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IArrowPremise<TAntecedent, TConsequent>
{
    TConsequent Apply(in TAntecedent pre);
}

#if false
public interface IArrowPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge> : IArrowPremise<TAntecedent, TConsequent>
    where TPreJudge : IJudgePremise
    where TPostJudge : IJudgePremise
{
}
#endif



public readonly struct Failure<TP, TQ> : IArrowPremise<TP, TQ>
{
    public static TQ Apply(in TP pre) => throw new InvalidOperationException(/* TODO */);

    TQ IArrowPremise<TP, TQ>.Apply(in TP pre) => Failure<TP, TQ>.Apply(in pre);
}

public readonly struct Identity<T> : IArrowPremise<T, T>
{
    public static T Apply(in T pre) => pre;

    T IArrowPremise<T, T>.Apply(in T pre) => Apply(in pre);
}



[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ArrowHandle<TP, TQ> : IHandle
{
    private readonly delegate*<in TP, TQ> _fp;

    internal ArrowHandle(delegate*<in TP, TQ> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public bool IsFailure => !this.HasValue;

    public TQ Apply(in TP pre)
    {
        if (IsFailure)
        {
            return Failure<TP, TQ>.Apply(in pre);
        }
        else
        {
            return _fp(in pre);
        }
    }
}


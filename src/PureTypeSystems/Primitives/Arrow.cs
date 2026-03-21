using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IArrowPremise<TAntecedent, TConsequent>
{
    TConsequent Apply(in TAntecedent pre);
}

public interface IArrowPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge> : IArrowPremise<TAntecedent, TConsequent>
    where TPreJudge : IJudgePremise
    where TPostJudge : IJudgePremise
{
}


public readonly struct Identity<T> : IArrowPremise<T, Tautology, T, Tautology>
{
    public static T Apply(in T pre) => pre;

    T IArrowPremise<T, T>.Apply(in T pre) => Apply(in pre);
}

public readonly struct Failure<TP, TQ> : IArrowPremise<TP, Tautology, TQ, Negation>
{
    public static TQ Apply(in TP pre) => throw new InvalidOperationException(/* TODO */);

    TQ IArrowPremise<TP, TQ>.Apply(in TP pre) => Failure<TP, TQ>.Apply(in pre);
}


[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ArrowHandle<TP, TQ> : IHandle
{
    private readonly JudgeHandle<TP> _preJudge;

    private readonly JudgeHandle<(TP, TQ)> _postJudge;

    private readonly delegate*<in TP, TQ> _fp;

    private ArrowHandle(JudgeHandle<TP> preJudge, JudgeHandle<(TP, TQ)> postJudge, delegate*<in TP, TQ> fp)
    {
        _preJudge = preJudge;
        _postJudge = postJudge;
        _fp = fp;
    }

    internal ArrowHandle(delegate*<in TP, TQ> fp) : 
        this(default, default, fp)
    {}

    public ArrowHandle<TP, TQ> WithJudges(JudgeHandle<TP> preJudge, JudgeHandle<(TP, TQ)> postJudge)
    {
        return new(preJudge, postJudge, _fp);
    }

    public JudgeHandle<TP> PreJudge => _preJudge;

    public JudgeHandle<(TP Pre, TQ Post)> PostJudge => _postJudge;

    public nint ToIntPtr() => (nint)_fp;

    private bool IsIdentityInternal => ToIntPtr() == ArrowTheory.IdentityPointer;

    public bool IsIdentity => _preJudge.IsTautology && _postJudge.IsTautology && IsIdentityInternal;

    public TQ Apply(in TP pre)
    {
        if (IsIdentityInternal)
        {
            if (typeof(TP) == typeof(TQ))
            {
                var result = Identity<TP>.Apply(in pre);
                return Unsafe.As<TP, TQ>(ref result);
            }
            else
            {
                return Failure<TP, TQ>.Apply(in pre);
            }
        }
        else
        {
            return _fp(in pre);
        }
    }
}


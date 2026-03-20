using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IImplyPremise<TAntecedent, TConsequent>
{
    TConsequent Apply(in TAntecedent pre);
}

public interface IImplyPremise<TAntecedent, TPreJudge, TConsequent, TPostJudge> : IImplyPremise<TAntecedent, TConsequent>
    where TPreJudge : IJudgePremise<TAntecedent>
    where TPostJudge : IJudgePremise<(TAntecedent, TConsequent)>
{
}


public readonly struct Identity<T> : IImplyPremise<T, Tautology<T>, T, Tautology<(T,T)>>
{
    public static T Apply(in T pre) => pre;

    T IImplyPremise<T, T>.Apply(in T pre) => Apply(in pre);
}

public readonly struct Failure<TP, TQ> : IImplyPremise<TP, Tautology<TP>, TQ, Negation<(TP,TQ)>>
{
    public static TQ Apply(in TP pre) => throw new InvalidOperationException(/* TODO */);

    TQ IImplyPremise<TP, TQ>.Apply(in TP pre) => Failure<TP, TQ>.Apply(in pre);
}


[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ImplyHandle<TP, TQ> : IHandle
{
    private readonly JudgeHandle<TP> _preJudge;

    private readonly JudgeHandle<(TP, TQ)> _postJudge;

    private readonly delegate*<in TP, TQ> _fp;

    private ImplyHandle(JudgeHandle<TP> preJudge, JudgeHandle<(TP, TQ)> postJudge, delegate*<in TP, TQ> fp)
    {
        _preJudge = preJudge;
        _postJudge = postJudge;
        _fp = fp;
    }

    internal ImplyHandle(delegate*<in TP, TQ> fp) : 
        this(default, default, fp)
    {}

    public ImplyHandle<TP, TQ> WithJudges(JudgeHandle<TP> preJudge, JudgeHandle<(TP, TQ)> postJudge)
    {
        return new(preJudge, postJudge, _fp);
    }

    public JudgeHandle<TP> PreJudge => _preJudge;

    public JudgeHandle<(TP Pre, TQ Post)> PostJudge => _postJudge;

    public nint ToIntPtr() => (nint)_fp;

    private bool IsIdentityInternal => ToIntPtr() == ImplyTheory.IdentityPointer;

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


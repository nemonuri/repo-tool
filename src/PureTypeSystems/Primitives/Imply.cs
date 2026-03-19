using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;


public interface IImplyPremise<TAntecedent, TConsequent>
{
    JudgeHandle<TAntecedent> PreJudge {get;}

    JudgeHandle<(TAntecedent Pre, TConsequent Post)> PostJudge {get;}

    TConsequent Apply(in TAntecedent pre);
}

public readonly struct Identity<T> : IImplyPremise<T,T>
{
    public static T Apply(in T pre) => pre;

    JudgeHandle<T> IImplyPremise<T, T>.PreJudge => JudgeTheory.GetTautologyHandle<T>();

    JudgeHandle<(T Pre, T Post)> IImplyPremise<T, T>.PostJudge => JudgeTheory.GetTautologyHandle<(T,T)>();

    T IImplyPremise<T, T>.Apply(in T pre) => Identity<T>.Apply(in pre);
}

public static class ImplyTheory
{
    public const nint IdentityPointer = 0;

    public static ImplyHandle<TP, TQ> GetIdentityHandle<TP, TQ>() => default;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ImplyHandle<TP, TQ> : IHandle
{
    private readonly JudgeHandle<TP> _preJudge;

    private readonly JudgeHandle<(TP, TQ)> _postJudge;

    private readonly delegate*<in TP, TQ> _fp;

    public ImplyHandle(JudgeHandle<TP> preJudge, JudgeHandle<(TP, TQ)> postJudge, delegate*<in TP, TQ> fp)
    {
        _preJudge = preJudge;
        _postJudge = postJudge;
        _fp = fp;
    }

    public ImplyHandle(delegate*<in TP, TQ> fp) : 
        this(default, default, fp)
    {}

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
                throw new InvalidOperationException(/* TODO */);
            }
        }
        else
        {
            return _fp(in pre);
        }
    }
}
using System.Runtime.CompilerServices;
using static Nemonuri.PureTypeSystems.Primitives.Extensions.JudgeHandleExtensions;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IJudgePremise
{
    Judgement Judge<T>(in T expr);
}

public readonly struct Tautology : IJudgePremise
{
    public static Judgement Judge<T>(in T _) => Judgement.True;

    Judgement IJudgePremise.Judge<T>(in T expr) => Judge(in expr);
}

public readonly struct Negation : IJudgePremise
{
    public static Judgement Judge<T>(in T _) => Judgement.False;

    Judgement IJudgePremise.Judge<T>(in T pre) => Judge(in pre);
}

public readonly struct Thunk : IJudgePremise
{
    public static Judgement Judge<T>(in T _) => Judgement.Thunk;

    Judgement IJudgePremise.Judge<T>(in T pre) => Judge(in pre);
}

public interface IJudgePremise<T>
{
    /// <summary>
    /// Judge 'expr' is constructable and has type 'T', in given premise.
    /// </summary>
    Judgement Judge(in T? expr);
}

public readonly struct BoundBasedFreeJudge<T1, TJudge> : IJudgePremise
    where TJudge : unmanaged, IJudgePremise<T1>
{
    public static Judgement Judge<T2>(in T2 expr)
    {
        if (JudgeTheory.TryFreeJudge<T1, T2, TJudge>((new TJudge()), in expr, out var jm))
        {
            return jm;
        }
        else
        {
            return Judgement.False;
        }
    }

    Judgement IJudgePremise.Judge<T2>(in T2 expr) => Judge(in expr);
}


[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct JudgeHandle<T> : IHandle
{
    private readonly delegate*<in T?, Judgement> _fp;

    internal JudgeHandle(delegate*<in T?, Judgement> fp)
    {
        _fp = fp;
    }

    public nint ToIntPtr() => (nint)_fp;

    public bool IsTautology => ToIntPtr() == JudgeTheory.TautologyPointer;

    public Judgement Judge(in T? pre)
    {
        if (IsTautology)
        {
            return Tautology.Judge(in pre);
        }
        else
        {
            return _fp(in pre);
        }
    }
}

/**
    reference: https://plato.stanford.edu/entries/logic-intuitionistic/#FormSystMathMath
*/

public readonly struct ForAll<TContext, T> : IJudgePremise<ArrowHandle<TContext, T>>
{
    public static Judgement Judge(in ArrowHandle<TContext, T> item)
    {
        var pre = item.PreJudge;
        var post = item.PostJudge;

        if (pre.IsTautology) 
        { 
            return post switch
            {
                { IsTautology: true } => Judgement.True,
                { IsNegation: true } => Judgement.False,
                { IsThunk: true } => Judgement.Thunk,
                _ => Judgement.Unknown,
            };
        }
        else if (pre.IsNegation) 
        { 
            return Judgement.True; 
        }
        else if (pre.IsThunk)
        {
            return post switch
            {
                { IsTautology: true } => Judgement.True,
                { IsNegation: true } => Judgement.Thunk,
                { IsThunk: true } => Judgement.Thunk,
                _ => Judgement.Unknown,
            };
        }
        else
        {
            return Judgement.Unknown;
        }
    }

    Judgement IJudgePremise<ArrowHandle<TContext, T>>.Judge(in ArrowHandle<TContext, T> pre) => Judge(in pre);
}

public readonly struct Exist<T, TContext> : IJudgePremise<ArrowHandle<T, TContext>>
{
    public static Judgement Judge(in ArrowHandle<T, TContext> item)
    {
        var pre = item.PreJudge;
        var post = item.PostJudge;

        if (pre.IsTautology) 
        { 
            return post switch
            {
                { IsTautology: true } => Judgement.True,
                { IsNegation: true } => Judgement.False,
                { IsThunk: true } => Judgement.Thunk,
                _ => Judgement.Unknown,
            };
        }
        else if (pre.IsNegation) 
        { 
            return Judgement.False; 
        }
        else if (pre.IsThunk)
        {
            return post switch
            {
                { IsTautology: true } => Judgement.Thunk,
                { IsNegation: true } => Judgement.False,
                { IsThunk: true } => Judgement.Thunk,
                _ => Judgement.Unknown,
            };
        }
        else
        {
            return Judgement.Unknown;
        }
    }

    Judgement IJudgePremise<ArrowHandle<T, TContext>>.Judge(in ArrowHandle<T, TContext> pre) => Judge(in pre);
}

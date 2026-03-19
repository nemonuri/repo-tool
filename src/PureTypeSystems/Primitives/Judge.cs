using System.Runtime.InteropServices;
using static Nemonuri.PureTypeSystems.Primitives.Extensions.JudgeHandleExtensions;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IJudgePremise<T>
{
    Judgement Judge(in T? pre);
}

public readonly struct Tautology<T> : IJudgePremise<T>
{
    public static Judgement Judge(in T? _) => Judgement.True;

    Judgement IJudgePremise<T>.Judge(in T? pre) => Tautology<T>.Judge(in pre);
}

public readonly struct Negation<T> : IJudgePremise<T>
{
    public static Judgement Judge(in T? _) => Judgement.False;

    Judgement IJudgePremise<T>.Judge(in T? pre) => Negation<T>.Judge(in pre);
}

public readonly struct Thunk<T> : IJudgePremise<T>
{
    public static Judgement Judge(in T? _) => Judgement.Thunk;

    Judgement IJudgePremise<T>.Judge(in T? pre) => Thunk<T>.Judge(in pre);
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
            return Tautology<T>.Judge(in pre);
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

public readonly struct ForAll<TContext, T> : IJudgePremise<ImplyHandle<TContext, T>>
{
    public static Judgement Judge(in ImplyHandle<TContext, T> item)
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

    Judgement IJudgePremise<ImplyHandle<TContext, T>>.Judge(in ImplyHandle<TContext, T> pre) => Judge(in pre);
}

public readonly struct Exist<T, TContext> : IJudgePremise<ImplyHandle<T, TContext>>
{
    public static Judgement Judge(in ImplyHandle<T, TContext> item)
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

    Judgement IJudgePremise<ImplyHandle<T, TContext>>.Judge(in ImplyHandle<T, TContext> pre) => Judge(in pre);
}

using static Nemonuri.PureTypeSystems.Primitives.Extensions.JudgeHandleExtensions;
using Nemonuri.PureTypeSystems.Primitives.TypeExpressions;
using Hth = Nemonuri.PureTypeSystems.Primitives.HandleTheory;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IJudgePremise
{
    /// <summary>
    /// Judge 'expr' is constructable and has type 'T', in given premise.
    /// </summary>
    Judgement Judge<T>(in T expr);
}

public readonly struct Tautology : IJudgePremise, IConstant<Judgement>
{
    private static Judgement Judgement => Judgement.True;

    public static Judgement Judge<T>(in T _) => Judgement;

    Judgement IJudgePremise.Judge<T>(in T expr) => Judge(in expr);

    Judgement IArrowPremise<ValueUnit, Judgement>.Apply(in ValueUnit _) => Judgement;
}

public readonly struct Negation : IJudgePremise, IConstant<Judgement>
{
    private static Judgement Judgement => Judgement.False;

    public static Judgement Judge<T>(in T _) => Judgement;

    Judgement IJudgePremise.Judge<T>(in T pre) => Judge(in pre);

    Judgement IArrowPremise<ValueUnit, Judgement>.Apply(in ValueUnit _) => Judgement;
}

public readonly struct Testable : IJudgePremise, IConstant<Judgement>
{
    private static Judgement Judgement => Judgement.Testable;

    public static Judgement Judge<T>(in T _) => Judgement;

    Judgement IJudgePremise.Judge<T>(in T pre) => Judge(in pre);

    Judgement IArrowPremise<ValueUnit, Judgement>.Apply(in ValueUnit _) => Judgement;
}

public readonly struct JudgeBasedArrow<T, TJudge> : IArrowPremise<T, Refined<T, TJudge>>
    where TJudge : IJudgePremise
{
    public static Refined<T, TJudge> Apply(in T pre) => new(pre);

    Refined<T, TJudge> IArrowPremise<T, Refined<T, TJudge>>.Apply(in T pre) => Apply(in pre);
}

public interface IJudgePremise<T> : IJudgePremise, IArrowPremise<T, Judgement>
{
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
public readonly struct JudgeHandle<T> : IHandle, IEquatable<JudgeHandle<T>>
{
    //private readonly delegate*<in T?, Judgement> _fp;
    private readonly ArrowHandle<T, Judgement> _arrowHandle;

    internal JudgeHandle(ArrowHandle<T, Judgement> arrowHandle)
    {
        _arrowHandle = arrowHandle;
    }

    public nint ToIntPtr() => _arrowHandle.ToIntPtr();

    public ArrowHandle<T, Judgement> ArrowHandle => _arrowHandle;

    public bool IsUnknown => ArrowHandle.IsFailure;

    public Judgement Judge(in T pre)
    {
        if (IsUnknown)
        {
            return Judgement.Unknown;
        }
        else
        {
            return ArrowHandle.Apply(in pre);
        }
    }

    public bool Equals(JudgeHandle<T> other) => Hth.Equals(this, other);

    public override bool Equals(object? obj) => obj is JudgeHandle<T> o && Equals(o);

    public override int GetHashCode() => Hth.GetHashCode(this);
}

#if false

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

    Judgement IJudgePremise.Judge<T2>(in T2 expr) => JudgeTheory.FreeJudge<ArrowHandle<TContext, T>, T2, ForAll<TContext, T>>(this, in expr);
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

    Judgement IJudgePremise.Judge<T2>(in T2 expr) => JudgeTheory.FreeJudge<ArrowHandle<T, TContext>, T2, Exist<T, TContext>>(this, in expr);
}

#endif
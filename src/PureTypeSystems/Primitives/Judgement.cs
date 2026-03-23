using System.Runtime.InteropServices;

namespace Nemonuri.PureTypeSystems.Primitives;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct Judgement
{
    internal Judgement(bool determinate, bool truthy)
    {
        Determinate = determinate;
        Truthy = truthy;
    }

    internal bool Determinate {get;}

    internal bool Truthy {get;}

    public static Judgement Unknown => new(false,false);

    public static Judgement Testable => new(false,true);

    public static Judgement False => new(true,false);

    public static Judgement True => new(true,true);

    public bool IsUnknown => this == Unknown;

    public bool IsTestable => this == Testable;

    public bool IsFalse => this == False;

    public bool IsTrue => this == True;
}


public static class JudgementTheory
{
    public static Judgement FromBoolean(bool source) => source ? Judgement.True : Judgement.False;
}

#if false
public readonly struct JudgementIsTrue : IJudgePremise<Judgement>
{
    public static Judgement Judge(in Judgement pre) => JudgementTheory.FromBoolean(pre == Judgement.True);

    Judgement IJudgePremise<Judgement>.Judge(in Judgement pre) => Judge(in pre);
}

public readonly struct JudgementIsFalse : IJudgePremise<Judgement>
{
    public static Judgement Judge(in Judgement pre) => JudgementTheory.FromBoolean(pre == Judgement.False);

    Judgement IJudgePremise<Judgement>.Judge(in Judgement pre) => Judge(in pre);
}

public readonly struct JudgementIsTrue<T> : IJudgePremise<(T, Judgement)>
{
    public static Judgement Judge(in (T, Judgement Judgement) pre) => JudgementIsTrue.Judge(in pre.Judgement);

    Judgement IJudgePremise<(T, Judgement)>.Judge(in (T, Judgement) pre) => Judge(in pre);
}

public readonly struct JudgementIsFalse<T> : IJudgePremise<(T, Judgement)>
{
    public static Judgement Judge(in (T, Judgement Judgement) pre) => JudgementIsFalse.Judge(in pre.Judgement);

    Judgement IJudgePremise<(T, Judgement)>.Judge(in (T, Judgement) pre) => Judge(in pre);
}
#endif

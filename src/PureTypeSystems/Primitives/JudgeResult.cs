using System.Diagnostics;
using System.Runtime.InteropServices;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

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

public readonly struct JudgeResult
{
    private JudgeResult(Judgement judgement, IEnumerableIntroducer<TestResult>? testerIntroducer)
    {
        Judgement = judgement;
        UncheckedTesterIntroducer = testerIntroducer;
    }

    internal static JudgeResult GuardNotTestableAndCreate(Judgement judgement)
    {
        Debug.Assert( !judgement.IsTestable );
        return new(judgement, null);
    }

    public Judgement Judgement {get;}

    public IEnumerableIntroducer<TestResult>? UncheckedTesterIntroducer {get;}

    public static JudgeResult CreateUnknown() => default;

    public static JudgeResult CreateTrue() => new(Judgement.True, null);

    public static JudgeResult CreateFalse() => new(Judgement.False, null);

    public static JudgeResult CreateTestable(IEnumerableIntroducer<TestResult> introducer) => new(Judgement.Testable, introducer);

    [MemberNotNullWhen(false, nameof(UncheckedTesterIntroducer))]
    private bool IsTesterNullOrInvalid => UncheckedTesterIntroducer is null || !UncheckedTesterIntroducer.Introducables.Any();

    public bool IsUnknown => Judgement.IsUnknown || (Judgement.IsTestable && IsTesterNullOrInvalid);

    [MemberNotNullWhen(true, nameof(UncheckedTesterIntroducer))]
    public bool IsTestable => Judgement.IsTestable && !IsTesterNullOrInvalid;

    public bool IsFalse => Judgement.IsFalse;

    public bool IsTrue => Judgement.IsTrue;

    public bool TryGetTesterIntroducer([NotNullWhen(true)] out IEnumerableIntroducer<TestResult>? testerIntroducer)
    {
        testerIntroducer = IsTestable ? UncheckedTesterIntroducer : null;
        return testerIntroducer is not null;
    }
}

public static class JudgementTheory
{
    public static Judgement FromBoolean(bool source) => source ? Judgement.True : Judgement.False;

    public static Judgement Intersect(Judgement first, Judgement second)
    {
        return (first, second) switch
        {
            ({ IsFalse: true }, _) or (_, { IsFalse: true }) => Judgement.False,
            ({ IsUnknown: true }, _) or (_, { IsUnknown: true }) => Judgement.Unknown,
            ({ IsTestable: false }, { IsTestable: false }) => Judgement.True,
            _ => Judgement.Testable
        };
    }

    public static Judgement Union(Judgement first, Judgement second)
    {
        return (first, second) switch
        {
            ({ IsTrue: true }, _) or (_, { IsTrue: true }) => Judgement.True,
            ({ IsUnknown: true }, _) or (_, { IsUnknown: true }) => Judgement.Unknown,
            ({ IsTestable: false }, { IsTestable: false }) => Judgement.False,
            _ => Judgement.Testable
        };
    }
}

public static class JudgeResultTheory
{
    public static JudgeResult FromBoolean(bool source) => source ? JudgeResult.CreateTrue() : JudgeResult.CreateFalse();

    public static JudgeResult Intersect(JudgeResult first, JudgeResult second)
    {
        static JudgeResult NotTestablePath(Judgement first, Judgement second) => JudgeResult.GuardNotTestableAndCreate(JudgementTheory.Intersect(first, second));
        static JudgeResult TestablePath(IEnumerableIntroducer<TestResult> first, IEnumerableIntroducer<TestResult> second)
        {
            return JudgeResult.CreateTestable(EnumerableIntroducerTheory.Intersect<TestResult>(first, second, TestResultTheory.IntersectHandle));
        }
         
        var jm = JudgementTheory.Intersect(first.Judgement, second.Judgement);

        if (jm.IsTestable)
        {
            return (first.TryGetTesterIntroducer(out var fi), second.TryGetTesterIntroducer(out var si)) switch
            {
                (true, true) => TestablePath(fi!, si!),
                (true, false) => TestablePath(fi!, fi!),
                (false, true) => TestablePath(si!, si!),
                _ => NotTestablePath(first.Judgement, second.Judgement)
            };
        }
        else
        {
            return NotTestablePath(first.Judgement, second.Judgement);
        }
    }

    public static JudgeResult Union(JudgeResult first, JudgeResult second)
    {
        static JudgeResult NotTestablePath(Judgement first, Judgement second) => JudgeResult.GuardNotTestableAndCreate(JudgementTheory.Union(first, second));
        static JudgeResult TestablePath(IEnumerableIntroducer<TestResult> first, IEnumerableIntroducer<TestResult> second)
        {
            return JudgeResult.CreateTestable(EnumerableIntroducerTheory.Union<TestResult>(first, second, TestResultTheory.UnionHandle));
        }

        var jm = JudgementTheory.Union(first.Judgement, second.Judgement);

        if (jm.IsTestable)
        {
            return (first.TryGetTesterIntroducer(out var fi), second.TryGetTesterIntroducer(out var si)) switch
            {
                (true, true) => TestablePath(fi!, si!),
                (true, false) => TestablePath(fi!, fi!),
                (false, true) => TestablePath(si!, si!),
                _ => NotTestablePath(first.Judgement, second.Judgement)
            };
        }
        else
        {
            return NotTestablePath(first.Judgement, second.Judgement);
        }
    }
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

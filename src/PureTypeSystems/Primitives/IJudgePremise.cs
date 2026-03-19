using System.Runtime.InteropServices;

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

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct JudgeHandle<T>(delegate*<in T?, Judgement> fp) : IHandle
{
    private readonly delegate*<in T?, Judgement> _fp = fp;

    public nint ToIntPtr() => (nint)_fp;

    public bool IsTautology => ToIntPtr() == JudgeTheory.TautologyPointer;

    Judgement Judge(in T? pre)
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

public static class JudgeTheory
{
    public const nint TautologyPointer = 0;

    public static JudgeHandle<T> GetTautologyHandle<T>() => default;
}
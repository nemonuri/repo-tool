using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public static class JudgeTheory
{
    public const nint TautologyPointer = 0;

    public static JudgeHandle<T> GetTautologyHandle<T>() => default;

    public static JudgeHandle<T> GetNegationHandle<T>() => FreeToHandle<Negation, T>();

    public static bool IsNegationHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetNegationHandle<T>().ToIntPtr();

    public static JudgeHandle<T> GetThunkHandle<T>() => FreeToHandle<Thunk, T>();

    public static bool IsThunkHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetThunkHandle<T>().ToIntPtr();

    public static bool TryJudgeTrue<T>(JudgeHandle<T> judgeHandle, [NotNullWhen(true)] in T? pre, out Judgement judgement) =>
        (judgement = judgeHandle.Judge(in pre)) == Judgement.True;

    public static bool TryJudgeTrue<T1, T2>(JudgeHandle<(T1,T2)> judgeHandle, in T1 pre, [NotNullWhen(true)] in T2 post, out Judgement judgement)
    {
        var pair = (pre, post);
        judgement = judgeHandle.Judge(in pair);
        return judgement == Judgement.True;
    }

    public static bool TryFreeJudge<T1, T2, TJudge>(in TJudge boundJudge, in T2 expr, out Judgement judgement)
        where TJudge : IJudgePremise<T1>
    {
        if (typeof(T1) != typeof(T2)) { judgement = Judgement.False; return false; }
        judgement = boundJudge.Judge(in Unsafe.As<T2,T1>(ref Unsafe.AsRef(in expr)));
        return true;
    }

    public static Judgement FreeJudge<T1, T2, TJudge>(in TJudge boundJudge, in T2 expr)
        where TJudge : IJudgePremise<T1>
    {
        TryFreeJudge<T1, T2, TJudge>(in boundJudge, in expr, out var jm);
        return jm;
    }

    extension<TJudge>(TJudge)
        where TJudge : unmanaged, IJudgePremise
    {
        public unsafe static JudgeHandle<T> FreeToHandle<T>()
        {
            static Judgement Impl(in T? item) => (new TJudge()).Judge(in item);

            if (typeof(TJudge) == typeof(Tautology))
            {
                return GetTautologyHandle<T>();
            }
            else
            {
                return new(&Impl);
            }
        }
    }

    extension<T, TJudge>(TJudge)
        where TJudge : unmanaged, IJudgePremise<T>
    {
        public unsafe static JudgeHandle<T> BoundToHandle() => FreeToHandle<BoundBasedFreeJudge<T, TJudge>, T>();
    }
}

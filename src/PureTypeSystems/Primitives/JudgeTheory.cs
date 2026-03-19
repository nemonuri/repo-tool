namespace Nemonuri.PureTypeSystems.Primitives;

public static class JudgeTheory
{
    public const nint TautologyPointer = 0;

    public static JudgeHandle<T> GetTautologyHandle<T>() => default;

    public static JudgeHandle<T> GetNegationHandle<T>() => ToHandle<T, Negation<T>>();

    public static bool IsNegationHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetNegationHandle<T>().ToIntPtr();

    public static JudgeHandle<T> GetThunkHandle<T>() => ToHandle<T, Thunk<T>>();

    public static bool IsThunkHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetThunkHandle<T>().ToIntPtr();

    public static bool TryJudgeTruthy<T>(JudgeHandle<T> judgeHandle, [NotNullWhen(true)] in T? pre, out Judgement judgement) =>
        (judgement = judgeHandle.Judge(in pre)).Truthy;

    public static bool TryJudgeTruthy<T1, T2>(JudgeHandle<(T1,T2)> judgeHandle, in T1 pre, [NotNullWhen(true)] in T2 post, out Judgement judgement)
    {
        var pair = (pre, post);
        judgement = judgeHandle.Judge(in pair);
        return judgement.Truthy;
    }

    extension<T, TJudge>(TJudge)
        where TJudge : unmanaged, IJudgePremise<T>
    {
        public unsafe static JudgeHandle<T> ToHandle()
        {
            static Judgement Impl(in T? item) => (new TJudge()).Judge(in item);

            if (typeof(TJudge) == typeof(Tautology<T>))
            {
                return GetTautologyHandle<T>();
            }
            else
            {
                return new(&Impl);
            }
        }
    }
}

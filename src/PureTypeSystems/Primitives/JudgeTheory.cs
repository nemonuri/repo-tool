using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;

public static class JudgeTheory
{
    public const nint UnknownPointer = 0;

    public static JudgeHandle<T> GetUnknownHandle<T>() => default;

    public static JudgeHandle<T> GetNegationHandle<T>() => ToHandle<Negation, T>();

    public static bool IsNegationHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetNegationHandle<T>().ToIntPtr();

#if false
    public static JudgeHandle<T> GetTestableHandle<T>() => ToHandle<Testable, T>();

    public static bool IsThunkHandle<T>(JudgeHandle<T> judgeHandle) => 
        judgeHandle.ToIntPtr() == GetTestableHandle<T>().ToIntPtr();
#endif

    public static bool TryJudgeTrue<T>(JudgeHandle<T> judgeHandle, [NotNullWhen(true)] in T pre, out JudgeResult judgeResult) =>
        (judgeResult = judgeHandle.Judge(in pre)).IsTrue;

    public static bool TryJudgeTrue<T1, T2>(JudgeHandle<(T1,T2)> judgeHandle, in T1 pre, [NotNullWhen(true)] in T2 post, out JudgeResult judgement)
    {
        var pair = (pre, post);
        judgement = judgeHandle.Judge(in pair);
        return judgement.IsTrue;
    }

    public static bool TryFreeJudge<T1, T2, TJudge>(in TJudge boundJudge, in T2 expr, out JudgeResult judgeResult)
        where TJudge : IJudgePremise<T1>
    {
        if (typeof(T1) != typeof(T2)) { judgeResult = JudgeResult.CreateFalse(); return false; }
        judgeResult = boundJudge.Apply(in Unsafe.As<T2,T1>(ref Unsafe.AsRef(in expr)));
        return true;
    }

    public static JudgeResult FreeJudge<T1, T2, TJudge>(in TJudge boundJudge, in T2 expr)
        where TJudge : IJudgePremise<T1>
    {
        TryFreeJudge<T1, T2, TJudge>(in boundJudge, in expr, out var jm);
        return jm;
    }

    extension<TJudge>(TJudge) where TJudge : IJudgePremise
    {
        public unsafe static JudgeHandle<T> ToHandle<T>()
        {
            static JudgeResult Impl(in T item) => RealizerTheory.Realize<TJudge>().Judge(in item);

            if (typeof(TJudge) == typeof(Unknown))
            {
                return GetUnknownHandle<T>();
            }

            ArrowHandle<T, JudgeResult> arrowHandle = new(&Impl);
            return new(arrowHandle);
        }
    }

    public static JudgeHandle<T> IntroduceJudge<T>(IIntroducer<JudgeResult> introducer)
    {
        var handle = introducer.Introduce<T>(default);
        return new(handle);
    }


#if false
    extension<T, TJudge>(TJudge)
        where TJudge : unmanaged, IJudgePremise<T>
    {
        public unsafe static JudgeHandle<T> BoundToHandle() => FreeToHandle<BoundBasedFreeJudge<T, TJudge>, T>();
    }
#endif
}

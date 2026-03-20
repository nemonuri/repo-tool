namespace Nemonuri.PureTypeSystems.Primitives.Extensions;

public static class JudgeHandleExtensions
{
    extension<T>(JudgeHandle<T> handle)
    {
        public bool IsNegation => JudgeTheory.IsNegationHandle(handle);

        public bool IsThunk => JudgeTheory.IsThunkHandle(handle);
    }
}

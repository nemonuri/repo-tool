namespace Nemonuri.PureTypeSystems.Primitives.Extensions;

#if false
public static class ArrowHandleExtensions
{
    extension<TP, TQ>(in ArrowHandle<TP, TQ> handle)
    {
        public bool TryApply(in TP? pre, [NotNullWhen(true)] out TQ? post, out ApplyJudgement applyJudgement)
        {
            return ArrowTheory.TryApplyTrue(in handle, in pre, out post, out applyJudgement);
        }
    }
}
#endif
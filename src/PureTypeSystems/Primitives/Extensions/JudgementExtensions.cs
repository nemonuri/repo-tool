namespace Nemonuri.PureTypeSystems.Primitives.Extensions;

public static class JudgementExtensions
{
    extension(Judgement jm)
    {
        public bool IsTrue => jm == Judgement.True;

        public bool IsTrueOrThunk => jm.Truthy;
    }
}
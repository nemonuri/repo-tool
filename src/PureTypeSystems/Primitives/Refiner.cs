namespace Nemonuri.PureTypeSystems.Primitives;

public interface IRefiner<TState>
{
    bool IsRefineable<T, TJudge>(in TState prevState, in T p, out TState nextState) where TJudge : IJudgePremise;
}

public class DefaultRefiner : IRefiner<bool>
{
    public static DefaultRefiner Instance {get;} = new();

    private DefaultRefiner() {}

    public bool IsRefineable<T, TJudge>(in bool prevState, in T p, out bool nextState) where TJudge : IJudgePremise
    {
        if (prevState == false)
        {
            nextState = false;
            return false;
        }

        bool result = JudgeTheory.IsTrueOrTestable<TJudge, T>(in p);
        nextState = result;
        return result;
    }
}
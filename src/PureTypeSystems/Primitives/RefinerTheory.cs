namespace Nemonuri.PureTypeSystems.Primitives;

public static class RefinerTheory
{
    extension<TPre, TPost, TRefiner>(TRefiner)
        where TRefiner : unmanaged, IRefinerPremise<TPre, TPost>
    {

        public unsafe static RefinerHandle<TPre, TPost> ToHandle()
        {
            static Judgement JudgeImpl(in TPre? pre, out TPost? post) => (new TRefiner()).Judge(in pre, out post);

            return new(&JudgeImpl);
        }
    }

    extension<T, TRefiner>(TRefiner)
        where TRefiner : unmanaged, IRefinerPremise<T>
    {

        public unsafe static RefinerHandle<T, T> ToHandle()
        {
            static Judgement JudgeImpl(in T? pre, out T? post) => (new TRefiner()).Judge(in pre, out post);

            return new(&JudgeImpl);
        }
    }
}

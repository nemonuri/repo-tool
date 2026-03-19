using System.Runtime.CompilerServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public static class RefinerTheory
{
    extension<T, TRefiner>(TRefiner)
        where TRefiner : unmanaged, IRefinerPremise<T>
    {
        public unsafe static RefinerHandle<T, T> ToHandle()
        {
            static Judgement Impl(in T? pre, out T? post)
            {
                var jm = (new TRefiner()).Judge(in pre);
                if (jm == Judgement.True)
                {
                    post = pre;
                }
                else
                {
                    post = default;
                }
                return jm;
            }

            return new(&Impl);
        }

        public unsafe static RefinerHandle<TPre, T> Assume<TPre>()
        {
            static Judgement Impl(in TPre? pre, out T? post)
            {
                var rf = new TRefiner();
                if (rf is IRefinerPremise<TPre, T> introduced)
                {
                    return introduced.Judge(in pre, out post);
                }
                else if (typeof(TPre) == typeof(T))
                {
                    return ToHandle<T, TRefiner>().Judge(in Unsafe.As<TPre?, T?>(ref Unsafe.AsRef(in pre)), out post);
                }
                else
                {
                    post = default;
                    return Judgement.Unknown;
                }
            }

            return new(&Impl);
        }


    }


    extension<TPre, TPost, TRefiner>(TRefiner)
        where TRefiner : unmanaged, IRefinerPremise<TPre, TPost>
    {
        public unsafe static RefinerHandle<TPre, TPost> ToHandle()
        {
            static Judgement Impl(in TPre? pre, out TPost? post) => (new TRefiner()).Judge(in pre, out post);

            return new(&Impl);
        }
    }
}

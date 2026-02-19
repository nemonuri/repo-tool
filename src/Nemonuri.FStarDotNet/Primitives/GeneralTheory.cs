namespace Nemonuri.FStarDotNet.Primitives;

public static partial class GeneralTheory
{
    public static Premise0<T> Introduce<T>(T t) => new();
    public readonly struct Premise0<T0>
    {
        public Premise1<T> Introduce<T>(T t) => new();
        public readonly struct Premise1<T1>
        {
            public Premise2<T> Introduce<T>(T t) => new();
            public readonly struct Premise2<T2>
            {
                public Premise3<T> Introduce<T>(T t) => new();
                public readonly struct Premise3<T3>
                {
                    public Premise4<T> Introduce<T>(T t) => new();
                    public readonly struct Premise4<T4>
                    {
                        public Premise5<T> Introduce<T>(T t) => new();
                        public readonly struct Premise5<T5>
                        {
                            
                        }
                    }
                }
            }
        }
    }
}

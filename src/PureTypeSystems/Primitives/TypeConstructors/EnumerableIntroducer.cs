namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public interface IEnumerableIntroducer<TQ> : IIntroducer<TQ>
{
    IEnumerable<Type> Introducables {get;}
}

public static class EnumerableIntroducerTheory
{
    private class Intersected<T> : IEnumerableIntroducer<T>
    {
        private readonly IEnumerableIntroducer<T> _first;

        private readonly IEnumerableIntroducer<T> _second;

        public Intersected(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second)
        {
            _first = first;
            _second = second;
        }

        [field: MaybeNull]
        public IEnumerable<Type> Introducables => field ??= _first.Introducables.Intersect(_second.Introducables);

        public bool TryIntroduce<TP>(in T hint, [NotNullWhen(true)] out IArrow<TP, T>? arrow)
        {
            throw new NotImplementedException();
        }

#if false
        public ArrowHandle<TP, T> Introduce<TP>(in T hint)
        {
            var firstResult = _first.Introduce<TP>(in hint);
            if (firstResult.IsFailure) 
            { 
                return ArrowTheory.GetFailureHandle<TP,T>(); 
            }

            var secondResult = _second.Introduce<TP>(in hint);
            if (secondResult.IsFailure)
            {
                return ArrowTheory.GetFailureHandle<TP,T>(); 
            }


        }
#endif
    }

    public static IEnumerableIntroducer<T> Intersect<T>(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second)
    {
        
    }
}
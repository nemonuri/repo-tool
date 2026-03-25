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

        private readonly ArrowHandle<(T, T), T> _resultIntersect;

        public Intersected(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second, ArrowHandle<(T, T), T> resultIntersect)
        {
            _first = first;
            _second = second;
            _resultIntersect = resultIntersect;
        }

        [field: MaybeNull]
        public IEnumerable<Type> Introducables => field ??= _first.Introducables.Intersect(_second.Introducables);

        public bool TryIntroduce<TP>(in T hint, [NotNullWhen(true)] out IArrow<TP, T>? arrow)
        {
            if (!_first.TryIntroduce<TP>(in hint, out var firstResult))
            {
                arrow = default;
                return false;
            }

            if (!_second.TryIntroduce<TP>(in hint, out var secondResult))
            {
                arrow = default;
                return false;
            }

            T Impl(TP p)
            {
                return _resultIntersect.Apply((firstResult.Apply(in p), secondResult.Apply(in p)));
            }

            arrow = new DefaultArrow<TP, T>(Impl);
            return true;
        }
    }

    public static IEnumerableIntroducer<T> Intersect<T>(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second, ArrowHandle<(T, T), T> resultIntersect)
    {
        if (ReferenceEquals(first, second)) { return first; }
        return new Intersected<T>(first, second, resultIntersect);
    }

    private class Unioned<T> : IEnumerableIntroducer<T>
    {
        private readonly IEnumerableIntroducer<T> _first;

        private readonly IEnumerableIntroducer<T> _second;

        private readonly ArrowHandle<(T, T), T> _resultUnion;

        public Unioned(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second, ArrowHandle<(T, T), T> resultUnion)
        {
            _first = first;
            _second = second;
            _resultUnion = resultUnion;
        }

        [field: MaybeNull]
        public IEnumerable<Type> Introducables => field ??= _first.Introducables.Union(_second.Introducables);

        public bool TryIntroduce<TP>(in T hint, [NotNullWhen(true)] out IArrow<TP, T>? arrow)
        {
            var firstResult = _first.TryIntroduce<TP>(in hint, out var r1) ? r1 : default;
            var secondResult = _first.TryIntroduce<TP>(in hint, out var r2) ? r2 : default;

            T Impl(TP p)
            {
                return _resultUnion.Apply((firstResult.Apply(in p), secondResult.Apply(in p)));
            }

            arrow = (firstResult, secondResult) switch
            {
                (null, null) => null,
                ({ } f, null) => f,
                (null, { } s) => s,
                ({ } f, { } s) => new DefaultArrow<TP, T>(Impl)
            };

            return arrow is not null;
        }
    }


    public static IEnumerableIntroducer<T> Union<T>(IEnumerableIntroducer<T> first, IEnumerableIntroducer<T> second, ArrowHandle<(T, T), T> resultUnion)
    {
        if (ReferenceEquals(first, second)) { return first; }
        return new Unioned<T>(first, second, resultUnion);
    }
}
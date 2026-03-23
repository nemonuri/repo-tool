namespace Nemonuri.PureTypeSystems.Primitives;

public interface IMethodPremise<TContext, TP, TQ> : IArrowPremise<(TContext, TP), TQ>
{
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct MethodHandle<TContext, TP, TQ> : IHandle
{
    private readonly ArrowHandle<(TContext, TP), TQ> _arrowHandle;

    internal MethodHandle(ArrowHandle<(TContext, TP), TQ> arrowHandle)
    {
        _arrowHandle = arrowHandle;
    }

    public ArrowHandle<(TContext, TP), TQ> ArrowHandle => _arrowHandle;

    public nint ToIntPtr() => _arrowHandle.ToIntPtr();

    public TQ Apply(in TContext context, in TP p)
    {
        var pair = (context, p);
        return ArrowHandle.Apply(in pair);
    }
}

public static class MethodTheory
{
    extension<TContext, TP, TQ, TMethod>(TMethod)
        where TMethod : IArrowPremise<(TContext, TP), TQ>
    {
        public static MethodHandle<TContext, TP, TQ> ToHandle()
        {
            var handle = ArrowTheory.ToHandle<(TContext, TP), TQ, TMethod>();
            return new(handle);
        }
    }
}
namespace Nemonuri.PureTypeSystems.Primitives;

// TODO: Remove

public interface ICurriedTypeFuncPremise<TContext>
{
    T Invoke<T>();
}

public interface ICurriedTypeFuncPremise<TContext, TAux>
{
    T Invoke<T>(TAux aux);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct CurriedTypeFuncHandle<TContext,T>(delegate*<T> fp)
{
    private readonly delegate*<T> _fp = fp;

    public bool HasValue => _fp != null;

    public T Invoke() => _fp();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct CurriedTypeFuncHandle<TContext,TAux,T>(delegate*<TAux,T> fp)
{
    private readonly delegate*<TAux,T> _fp = fp;

    public bool HasValue => _fp != null;

    public T Invoke(TAux aux) => _fp(aux);
}

public static class CurriedTypeFuncTheory
{
    extension<TContext, TPremise>(TPremise)
        where TPremise : unmanaged, ICurriedTypeFuncPremise<TContext>
    {
        public unsafe static CurriedTypeFuncHandle<TContext,T> ToHandle<T>()
        {
            static T Impl() => (new TPremise()).Invoke<T>();

            return new(&Impl);
        }
    }

    extension<TContext, TAux, TPremise>(TPremise)
        where TPremise : unmanaged, ICurriedTypeFuncPremise<TContext, TAux>
    {
        public unsafe static CurriedTypeFuncHandle<TContext,TAux,T> ToHandle<T>()
        {
            static T Impl(TAux aux) => (new TPremise()).Invoke<T>(aux);

            return new(&Impl);
        }
    }
}
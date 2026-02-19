namespace Nemonuri.FStarDotNet.Primitives;


public interface IFStarKind : IFStarType
{
}

public interface ISolvedFStarKind : IFStarKind
{
    IFStarType? GetSolvedFStarType();
}

public interface ISolvedFStarKind<TTypeList> : ISolvedFStarKind
    where TTypeList : unmanaged, ITypeList
{
    new FStarType<TTypeList> GetSolvedFStarType();
}


public readonly struct SolvedFStarKind<TTypeList> : ISolvedFStarKind<TTypeList>
    where TTypeList : unmanaged, ITypeList
{
    public FStarType<TTypeList> GetSolvedFStarType() => new();

    IFStarType? ISolvedFStarKind.GetSolvedFStarType() => GetSolvedFStarType();

    public ITypeList? GetDotNetTypes() => GetSolvedFStarType().GetDotNetTypes();
}


public interface IUnsolvedFStarKind : IFStarKind
{
    IFStarKind? Specialize(IFStarType? source);
}

public interface IUnsolvedFStarKind<in TSource> : IUnsolvedFStarKind
    where TSource : IFStarType
{
    IFStarKind? Specialize(TSource source);
}

public interface IUnsolvedFStarKind<in TSource, out TContiuation> : IUnsolvedFStarKind<TSource>
    where TSource : IFStarType
    where TContiuation : IFStarKind
{
    new TContiuation Specialize(TSource source);
}


public readonly struct UnsolvedFStarKind<TSource, TContiuation> : IUnsolvedFStarKind<TSource, TContiuation>
    where TSource : unmanaged, IFStarType
    where TContiuation : unmanaged, IFStarKind
{
    public TContiuation Specialize(TSource source) => new();

    IFStarKind? IUnsolvedFStarKind<TSource>.Specialize(TSource source) => Specialize(new());

    public IFStarKind? Specialize(IFStarType? source) => source is TSource v ? Specialize(v) : null;

    public ITypeList? GetDotNetTypes() => null;
}



#if false

public interface IFStarKind<TValue, TSpecializer> : IFStarKind
    where TValue : IFStarInstance?
    where TSpecializer : IFStarFunction<TValue, IFStarKind>?
{
    bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer? specializer);
}

public readonly struct EmptyFStarKind : IFStarKind
{
    public bool TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarInstance?, IFStarKind>? specializer)
    {
        specializer = null; return false;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    public ITypeList Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarInstance.Value => Value;
}

public readonly struct EmptyFStarKindSpecializer : IFStarFunction<IFStarInstance?, IFStarKind>
{
    public IFStarKind Invoke(IFStarInstance? source) => new EmptyFStarKind();

    IFStarInstance? IFStarFunction.Invoke(IFStarInstance? source) => ((IFStarFunction<IFStarInstance?, IFStarKind>)this).Invoke(source);

    Func<IFStarInstance?, IFStarInstance?> IFStarInstance<Func<IFStarInstance?, IFStarInstance?>>.Value => ((IFStarFunction)this).Invoke;

    object? IFStarInstance.Value => ((IFStarInstance<Func<IFStarInstance?, IFStarInstance?>>)this).Value;
}

public readonly struct SolvedFStarKind<TType> : IFStarKind<IFStarInstance?, EmptyFStarKindSpecializer>
    where TType : unmanaged, IFStarType
{
    public bool TryGetSpecializer([NotNullWhen(true)] out EmptyFStarKindSpecializer specializer)
    {
        specializer = default; return false;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out TType solved)
    {
        solved = new(); return true;
    }

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarInstance?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = r; return s;
    }

    bool IFStarKind.TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        var s = TryGetSolvedFStarType(out var r);
        solved = r; return s;
    }

    ITypeList IFStarInstance<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarInstance.Value => ((IFStarInstance<ITypeList>)this).Value;
}

public readonly struct UnsolvedFStarKind<TValue, TSpecializer> : IFStarKind<TValue, TSpecializer>
    where TValue : IFStarInstance?
    where TSpecializer : unmanaged, IFStarFunction<TValue, IFStarKind>
{
    public bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer specializer)
    {
        specializer = new(); return true;
    }

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarInstance?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = FStarKindTheory.IntroduceFStarValue<TValue>().IntroduceFStarKindSpecializer<TSpecializer>().WrapSpecializer(r); 
        return s;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    ITypeList IFStarInstance<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarInstance.Value => ((IFStarInstance<ITypeList>)this).Value;
}

public readonly struct InefficientUnsolvedFStarKind<TValue, TSpecializer> : IFStarKind<TValue, TSpecializer>
    where TValue : IFStarInstance?
    where TSpecializer : IFStarFunction<TValue, IFStarKind>
{
    private readonly TSpecializer? _specializer;

    public InefficientUnsolvedFStarKind(TSpecializer? specializer)
    {
        _specializer = specializer;
    }

    public bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer? specializer) => (specializer = _specializer) is not null;

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarInstance?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = FStarKindTheory.IntroduceFStarValue<TValue>().IntroduceFStarKindSpecializer<TSpecializer>().WrapSpecializer(r); 
        return s;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    ITypeList IFStarInstance<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarInstance.Value => ((IFStarInstance<ITypeList>)this).Value;
}

public static class FStarKindTheory
{
    public static ITypeList ToFStarValue(IFStarKind? kind)
    {
        if 
        (
            kind is not null                            &&
            kind.TryGetSolvedFStarType(out var solved)  &&
            solved?.Value is { } v
        )
        {
            return v;
        }
        else
        {
            return TypeListTheory.Empty;
        }
    }


    public static FStarValuePremise<TValue> IntroduceFStarValue<TValue>() where TValue : IFStarInstance? => new();

    public readonly struct FStarValuePremise<TValue> where TValue : IFStarInstance?
    {
        public FStarKindSpecializerPremise<TSpecializer> IntroduceFStarKindSpecializer<TSpecializer>() where TSpecializer : IFStarFunction<TValue, IFStarKind>? => new();

        public readonly struct FStarKindSpecializerPremise<TSpecializer> where TSpecializer : IFStarFunction<TValue, IFStarKind>?
        {
            public IFStarFunction<IFStarInstance?, IFStarKind> WrapSpecializer(TSpecializer? specializer) => new WrappedSpecializer(specializer);

            private readonly struct WrappedSpecializer(TSpecializer? specializer) : IFStarFunction<IFStarInstance?, IFStarKind>
            {
                public IFStarKind Invoke(IFStarInstance? source)
                {
                    if 
                    (
                        source is TValue v0                 &&
                        specializer?.Invoke(v0) is { } v1
                    )
                    {
                        return v1;
                    }
                    else
                    {
                        return new EmptyFStarKind();
                    }
                }

                IFStarInstance? IFStarFunction.Invoke(IFStarInstance? source) => Invoke(source);

                public Func<IFStarInstance?, IFStarInstance?> Value => specializer?.Value ?? ((IFStarInstance<Func<IFStarInstance?, IFStarInstance?>>)new IdentityFStarFunction<IFStarInstance?>()).Value;

                object? IFStarInstance.Value => Value;
            }
        }
    }
}

#endif
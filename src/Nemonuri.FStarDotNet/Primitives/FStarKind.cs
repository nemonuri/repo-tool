using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.FStarDotNet.Primitives;


public interface IFStarKind : IFStarType
{
    bool TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarValue?, IFStarKind>? specializer);

    bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved);
}

public interface IFStarKind<TValue, TSpecializer> : IFStarKind
    where TValue : IFStarValue?
    where TSpecializer : IFStarFunction<TValue, IFStarKind>?
{
    bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer? specializer);
}

public readonly struct EmptyFStarKind : IFStarKind
{
    public bool TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarValue?, IFStarKind>? specializer)
    {
        specializer = null; return false;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    public ITypeList Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarValue.Value => Value;
}

public readonly struct EmptyFStarKindSpecializer : IFStarFunction<IFStarValue?, IFStarKind>
{
    public IFStarKind Invoke(IFStarValue? source) => new EmptyFStarKind();

    IFStarValue? IFStarFunction.Invoke(IFStarValue? source) => ((IFStarFunction<IFStarValue?, IFStarKind>)this).Invoke(source);

    Func<IFStarValue?, IFStarValue?> IFStarValue<Func<IFStarValue?, IFStarValue?>>.Value => ((IFStarFunction)this).Invoke;

    object? IFStarValue.Value => ((IFStarValue<Func<IFStarValue?, IFStarValue?>>)this).Value;
}

public readonly struct SolvedFStarKind<TType> : IFStarKind<FStarUnit?, EmptyFStarKindSpecializer>
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

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarValue?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = r; return s;
    }

    bool IFStarKind.TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        var s = TryGetSolvedFStarType(out var r);
        solved = r; return s;
    }

    ITypeList IFStarValue<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarValue.Value => ((IFStarValue<ITypeList>)this).Value;
}

public readonly struct UnsolvedFStarKind<TValue, TSpecializer> : IFStarKind<TValue, TSpecializer>
    where TValue : IFStarValue?
    where TSpecializer : unmanaged, IFStarFunction<TValue, IFStarKind>
{
    public bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer specializer)
    {
        specializer = new(); return true;
    }

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarValue?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = FStarKindTheory.IntroduceFStarValue<TValue>().IntroduceFStarKindSpecializer<TSpecializer>().WrapSpecializer(r); 
        return s;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    ITypeList IFStarValue<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarValue.Value => ((IFStarValue<ITypeList>)this).Value;
}

public readonly struct InefficientUnsolvedFStarKind<TValue, TSpecializer> : IFStarKind<TValue, TSpecializer>
    where TValue : IFStarValue?
    where TSpecializer : IFStarFunction<TValue, IFStarKind>
{
    private readonly TSpecializer? _specializer;

    public InefficientUnsolvedFStarKind(TSpecializer? specializer)
    {
        _specializer = specializer;
    }

    public bool TryGetSpecializer([NotNullWhen(true)] out TSpecializer? specializer) => (specializer = _specializer) is not null;

    bool IFStarKind.TryGetSpecializer([NotNullWhen(true)] out IFStarFunction<IFStarValue?, IFStarKind>? specializer)
    {
        var s = TryGetSpecializer(out var r);
        specializer = FStarKindTheory.IntroduceFStarValue<TValue>().IntroduceFStarKindSpecializer<TSpecializer>().WrapSpecializer(r); 
        return s;
    }

    public bool TryGetSolvedFStarType([NotNullWhen(true)] out IFStarType? solved)
    {
        solved = null; return false;
    }

    ITypeList IFStarValue<ITypeList>.Value => FStarKindTheory.ToFStarValue(this);

    object? IFStarValue.Value => ((IFStarValue<ITypeList>)this).Value;
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


    public static FStarValuePremise<TValue> IntroduceFStarValue<TValue>() where TValue : IFStarValue? => new();

    public readonly struct FStarValuePremise<TValue> where TValue : IFStarValue?
    {
        public FStarKindSpecializerPremise<TSpecializer> IntroduceFStarKindSpecializer<TSpecializer>() where TSpecializer : IFStarFunction<TValue, IFStarKind>? => new();

        public readonly struct FStarKindSpecializerPremise<TSpecializer> where TSpecializer : IFStarFunction<TValue, IFStarKind>?
        {
            public IFStarFunction<IFStarValue?, IFStarKind> WrapSpecializer(TSpecializer? specializer) => new WrappedSpecializer(specializer);

            private readonly struct WrappedSpecializer(TSpecializer? specializer) : IFStarFunction<IFStarValue?, IFStarKind>
            {
                public IFStarKind Invoke(IFStarValue? source)
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

                IFStarValue? IFStarFunction.Invoke(IFStarValue? source) => Invoke(source);

                public Func<IFStarValue?, IFStarValue?> Value => specializer?.Value ?? ((IFStarValue<Func<IFStarValue?, IFStarValue?>>)new IdentityFStarFunction<IFStarValue?>()).Value;

                object? IFStarValue.Value => Value;
            }
        }
    }
}

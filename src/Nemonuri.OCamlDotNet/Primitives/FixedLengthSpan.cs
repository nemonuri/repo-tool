using static Nemonuri.OCamlDotNet.FixedSizeTheory;

namespace Nemonuri.OCamlDotNet;

public readonly ref struct FixedLengthSpan<TSize, T>
    where TSize : unmanaged, IFixedSizePremise<TSize>
{
    public readonly Span<T> Values {get;}

    public bool IsLengthValid => Values.Length == GetFixedSize<TSize>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FixedLengthSpan(Span<T> values)
    {
        // Do net validate length.
        Values = values;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixedLengthSpan<TSize, T> UncheckedCreate(Span<T> values)
    {
        return new(values);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixedLengthSpan<TSize, T> Create(Span<T> values)
    {
        Guard.IsEqualTo(values.Length, GetFixedSize<TSize>());
        return UncheckedCreate(values);
    }
}
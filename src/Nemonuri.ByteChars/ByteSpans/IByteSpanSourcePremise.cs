namespace Nemonuri.ByteChars.ByteSpans;

public unsafe interface IByteSpanSourcePremise<TSource>
#if NET9_0_OR_GREATER
    where TSource : allows ref struct
#endif
{
    /// <summary>
    /// left <= right
    /// </summary>
    bool LessThanOrEqualAll(TSource left, TSource right);

    /// <summary>
    /// left == right
    /// </summary>
    bool EqualsAll(TSource left, TSource right);

    TSource Add(TSource left, TSource right);

    TSource Subtract(TSource left, TSource right);

    TSource Modulus(TSource left, TSource right);

    bool TryDecomposeToReadOnlyByteSpan(TSource source, out ReadOnlySpan<byte> readOnlyByteSpan);

    bool TryDecomposeToByteSpan(TSource source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux);

    delegate*<ReadOnlySpan<byte>, object?, TSource> ComposeFromByteSpan {get;}

    TSource GetTemporaryConstant(byte value);
}

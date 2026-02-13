namespace Nemonuri.ByteChars;

public unsafe interface IByteCharPremise<TSelf, TOperand>
    where TSelf : unmanaged, IByteCharPremise<TSelf, TOperand>
#if NET9_0_OR_GREATER
    where TOperand : allows ref struct
#endif
{
    /// <summary>
    /// left <= right
    /// </summary>
    bool LessThanOrEqualAll(TOperand left, TOperand right);

    /// <summary>
    /// left == right
    /// </summary>
    bool EqualsAll(TOperand left, TOperand right);

    TOperand Add(TOperand left, TOperand right);

    TOperand Subtract(TOperand left, TOperand right);

    TOperand Modulus(TOperand left, TOperand right);

    bool TryDecomposeToReadOnlyByteSpan(TOperand source, out ReadOnlySpan<byte> readOnlyByteSpan);

    bool TryDecomposeToByteSpan(TOperand source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux);

    delegate*<ReadOnlySpan<byte>, object?, TOperand> ComposeFromByteSpan {get;}

    TOperand GetTemporaryConstant(byte value);
}

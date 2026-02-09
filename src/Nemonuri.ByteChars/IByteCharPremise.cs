namespace Nemonuri.ByteChars;

public interface IByteCharPremise<TSelf, TOperand>
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

    bool TryUnsafeDecomposeToByteSpan(TOperand composed, out Span<byte> unsafeBytes);

    TOperand GetTemporaryConstant(byte value);
}

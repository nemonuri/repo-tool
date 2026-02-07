namespace Nemonuri.OCamlDotNet;

public interface IByteCharOperationPremise<TSelf, TOperand>
    where TSelf : unmanaged, IByteCharOperationPremise<TSelf, TOperand>
{
    /// <summary>
    /// left <= right
    /// </summary>
    bool LessThanOrEqualAll(TOperand left, TOperand right);

    /// <summary>
    /// left == right
    /// </summary>
    bool EqualsAll(TOperand left, TOperand right);

    TOperand AddAll(TOperand left, TOperand right);

    TOperand SubtractAll(TOperand left, TOperand right);

    TOperand ModulusAll(TOperand left, TOperand right);

    //bool TryGetUnsafeDecompositionPremise<TDecomposed>(out UnsafeDecompositionPremise<TOperand, TDecomposed> premise);

    bool TryUnsafeDecomposeToByteSpan(TOperand composed, out Span<byte> unsafeBytes);

    TOperand GetConstant(byte value);
}

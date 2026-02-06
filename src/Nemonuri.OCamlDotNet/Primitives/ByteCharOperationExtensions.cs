namespace Nemonuri.OCamlDotNet;

public static unsafe class ByteCharOperationExtensions
{
    extension<TOperator, TOperand>(TOperator)
        where TOperator : unmanaged, IByteCharOperationPremise<TOperator, TOperand>
    {
        public static void DoAdd(ref TOperand b1, TOperand b2)
        {
            TOperator op = new();

            op.Add(ref b1, b2);
        }
    }
}


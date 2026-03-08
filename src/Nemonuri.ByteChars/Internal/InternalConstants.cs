namespace Nemonuri.ByteChars.Internal;

internal static class InternalConstants
{
    /// <summary>
    /// stackalloc 하기에 적절한 메모리 크기. 단위는 byte. <br/>
    /// </summary>
    /// <remarks>
    /// Microsoft 는 주로 256 byte를 사용한다. 이유가 있겠지.
    /// </remarks>
    internal const int StackAllocThreshold = 256;

    /// <summary>
    /// "-2147483648"
    /// </summary>
    internal const int MaxDecimalInt32ByteStringLength = 11;
}
using System.Numerics;

namespace Nemonuri.ByteChars.Internal;

internal readonly struct StackLimitSizePremise : IFixedSizePremise<StackLimitSizePremise>
{
    public readonly int FixedSize => ByteStringConstants.StackAllocThreshold;
}

internal readonly struct ByteVectorSizePremise : IFixedSizePremise<ByteVectorSizePremise>
{
    public readonly int FixedSize => Vector<byte>.Count;
}

internal readonly struct Base1E9Premise : IFixedSizePremise<Base1E9Premise>
{
/**
## 1E9 진법?

### 10진법 표현

> 참고: 3자리 ',' 구분자 사용

- System.Int32.MaxValue =  2,147,483,647
- System.Int32.MaxValue = -2,147,483,648
- 1E9                   =  1,000,000,000

따라서, 절대값이 10진법으로 9자리 이하의 숫자로 표현되는 값은, System.Int32 타입으로 안전하게 나타낼 수 있다!
*/
    public readonly int FixedSize => 9;
}

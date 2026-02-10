using System.Numerics;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Nemonuri.NetStandards.Numerics;

public static class BigIntegerTheory
{
#if false
/** 
- Reference: https://github.com/dotnet/dotnet/blob/e80c4eea9c18a3e46a3898ad1c3223d88095c722/src/runtime/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigIntegerCalculator.Utils.cs#L16C13-L16C33

왜 stackalloc 한계를 256 (= 64 * sizeof(uint)) 바이트로 잡았을까?
- 난 byte string 에 대해 이 값을 512 바이트로 정했는데, 괜찮으려나?
*/
    internal const int StackAllocThreshold = 64 * sizeof(uint);
#endif

#if NETSTANDARD2_0
    internal const uint kuMaskHighBit = unchecked((uint)int.MinValue);
    internal const int kcbitUint = 32;
    internal const int kcbitUlong = 64;

    /// <summary>
    /// <see cref="System.Numerics.BigInteger(System.ReadOnlySpan{byte}, bool, bool)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor?view=net-10.0#system-numerics-biginteger-ctor(system-readonlyspan((system-byte))-system-boolean-system-boolean)">
    /// (doc)
    /// </a>
    /// </summary>
    public static BigInteger Create(ReadOnlySpan<byte> value, bool isUnsigned = false, bool isBigEndian = false)
    {
/** 뭐지. netstadard2.1 에서는 맨 앞의 '##'를 '전처리문'으로 인식하네;; */
/**
들어가기
-------

```cs
byte[] value = CreateRandomByteArray();
Debug.Assert( new BigInteger((byte[])value) == new BigInteger((ReadOnlySpan<byte>)value, isUnsigned: false, isBigEndian: false) ); // pass
```

즉, 내가 구현해야 할 것은, `(isUnsigned, isBigEndian) != (false, false)` 인 상황을 처리하는 코드!

- Reference: https://github.com/dotnet/runtime/blob/9ffface2f3fa6fbbb427793c3230b1626a1fdd84/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L276

Todo: Allocation 줄이기
----------------------

- `value`의 길이가 sizeof(long) 이하일 경우, 불필요한 Array 생성을 하지 않는 BigInteger 생성자를 호출할 수 있기는 하다.
- 그런데 여러 경우의 수를 따져봐야 하니 나중에...
*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetMostSignificantByteIndex(ReadOnlySpan<byte> value, bool isBigEndian)
        {
            Debug.Assert(value.Length > 0);
            return isBigEndian ? 0 : value.Length-1;
        }

        if (value.Length == 0) { return BigInteger.Zero; }

        if (isUnsigned == false && isBigEndian == false) { return new BigInteger(value.ToArray()); }

        // Get most significant byte
        byte msByte = value[GetMostSignificantByteIndex(value, isBigEndian)];

        const byte MostSignificantBitMask = 0b_1000_0000;

        bool lookLikeNegative = (msByte & MostSignificantBitMask) != 0;

        bool needExtraZero = isUnsigned && lookLikeNegative;

        byte[] clonedValue = !needExtraZero ? value.ToArray() : [..value, (byte)0];

        if (isBigEndian)
        {
            clonedValue.AsSpan().Slice(0, value.Length).Reverse();
        }

        return new BigInteger(value.ToArray());
    }
#endif
}
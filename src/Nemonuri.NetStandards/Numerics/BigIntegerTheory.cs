using System.Numerics;

namespace Nemonuri.NetStandards.Numerics;

public static class BigIntegerTheory
{
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
/**
- Reference: https://github.com/dotnet/runtime/blob/9ffface2f3fa6fbbb427793c3230b1626a1fdd84/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L276
*/
        int byteCount = value.Length;

        bool isNegative;
        if (byteCount > 0)
        {
            byte mostSignificantByte = isBigEndian ? value[0] : value[byteCount - 1];
            isNegative = (mostSignificantByte & 0x80) != 0 && !isUnsigned;

            if (mostSignificantByte == 0)
            {
                // Try to conserve space as much as possible by checking for wasted leading byte[] entries
                if (isBigEndian)
                {
                    int offset = 1;

                    while (offset < byteCount && value[offset] == 0)
                    {
                        offset++;
                    }

                    value = value.Slice(offset);
                    byteCount = value.Length;
                }
                else
                {
                    byteCount -= 2;

                    while (byteCount >= 0 && value[byteCount] == 0)
                    {
                        byteCount--;
                    }

                    byteCount++;
                }
            }
        }
        else
        {
            isNegative = false;
        }

        if (byteCount == 0)
        {
            // BigInteger.Zero
            return BigInteger.Zero;
        }

        if (byteCount <= 4)
        {
            _sign = isNegative ? unchecked((int)0xffffffff) : 0;

            if (isBigEndian)
            {
                for (int i = 0; i < byteCount; i++)
                {
                    _sign = (_sign << 8) | value[i];
                }
            }
            else
            {
                for (int i = byteCount - 1; i >= 0; i--)
                {
                    _sign = (_sign << 8) | value[i];
                }
            }

            _bits = null;
            if (_sign < 0 && !isNegative)
            {
                // Int32 overflow
                // Example: Int64 value 2362232011 (0xCB, 0xCC, 0xCC, 0x8C, 0x0)
                // can be naively packed into 4 bytes (due to the leading 0x0)
                // it overflows into the int32 sign bit
                _bits = new uint[1] { unchecked((uint)_sign) };
                _sign = +1;
            }
            if (_sign == int.MinValue)
            {
                this = s_bnMinInt;
            }
        }
        else
        {
            int wholeUInt32Count = Math.DivRem(byteCount, 4, out int unalignedBytes);
            uint[] val = new uint[wholeUInt32Count + (unalignedBytes == 0 ? 0 : 1)];

            // Copy the bytes to the uint array, apart from those which represent the
            // most significant uint if it's not a full four bytes.
            // The uints are stored in 'least significant first' order.
            if (isBigEndian)
            {
                // The bytes parameter is in big-endian byte order.
                // We need to read the uints out in reverse.

                Span<byte> uintBytes = MemoryMarshal.AsBytes(val.AsSpan(0, wholeUInt32Count));

                // We need to slice off the remainder from the beginning.
                value.Slice(unalignedBytes).CopyTo(uintBytes);

                uintBytes.Reverse();
            }
            else
            {
                // The bytes parameter is in little-endian byte order.
                // We can just copy the bytes directly into the uint array.

                value.Slice(0, wholeUInt32Count * 4).CopyTo(MemoryMarshal.AsBytes<uint>(val.AsSpan()));
            }

            // In both of the above cases on big-endian architecture, we need to perform
            // an endianness swap on the resulting uints.
            if (!BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.ReverseEndianness(val.AsSpan(0, wholeUInt32Count), val);
            }

            // Copy the last uint specially if it's not aligned
            if (unalignedBytes != 0)
            {
                if (isNegative)
                {
                    val[wholeUInt32Count] = 0xffffffff;
                }

                if (isBigEndian)
                {
                    for (int curByte = 0; curByte < unalignedBytes; curByte++)
                    {
                        byte curByteValue = value[curByte];
                        val[wholeUInt32Count] = (val[wholeUInt32Count] << 8) | curByteValue;
                    }
                }
                else
                {
                    for (int curByte = byteCount - 1; curByte >= byteCount - unalignedBytes; curByte--)
                    {
                        byte curByteValue = value[curByte];
                        val[wholeUInt32Count] = (val[wholeUInt32Count] << 8) | curByteValue;
                    }
                }
            }

            if (isNegative)
            {
                NumericsHelpers.DangerousMakeTwosComplement(val); // Mutates val

                // Pack _bits to remove any wasted space after the twos complement
                int len = val.Length - 1;
                while (len >= 0 && val[len] == 0) len--;
                len++;

                if (len == 1)
                {
                    switch (val[0])
                    {
                        case 1: // abs(-1)
                            this = s_bnMinusOneInt;
                            return;

                        case kuMaskHighBit: // abs(Int32.MinValue)
                            this = s_bnMinInt;
                            return;

                        default:
                            if (unchecked((int)val[0]) > 0)
                            {
                                _sign = (-1) * ((int)val[0]);
                                _bits = null;
                                AssertValid();
                                return;
                            }

                            break;
                    }
                }

                if (len != val.Length)
                {
                    _sign = -1;
                    _bits = new uint[len];
                    Array.Copy(val, _bits, len);
                }
                else
                {
                    _sign = -1;
                    _bits = val;
                }
            }
            else
            {
                _sign = +1;
                _bits = val;
            }
        }
        AssertValid();
    }
#endif
}
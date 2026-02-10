using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nemonuri.NetStandards.Runtime.CompilerServices;

namespace Nemonuri.NetStandards;

public static partial class MemoryTheory
{
    internal static bool Contains<T>(ref T searchSpace, T value, int length) where T : IEquatable<T>?
    {
/**
- Reference: https://github.com/dotnet/runtime/blob/v10.0.2/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L227
*/
        Debug.Assert(length >= 0);

        nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations

        if (default(T) != null || (object?)value != null)
        {
            Debug.Assert(value is not null);

            while (length >= 8)
            {
                length -= 8;

                if (value.Equals(Unsafe.Add(ref searchSpace, index + 0)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 1)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 2)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 3)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 4)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 5)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 6)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 7)))
                {
                    goto Found;
                }

                index += 8;
            }

            if (length >= 4)
            {
                length -= 4;

                if (value.Equals(Unsafe.Add(ref searchSpace, index + 0)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 1)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 2)) ||
                    value.Equals(Unsafe.Add(ref searchSpace, index + 3)))
                {
                    goto Found;
                }

                index += 4;
            }

            while (length > 0)
            {
                length--;

                if (value.Equals(Unsafe.Add(ref searchSpace, index)))
                    goto Found;

                index += 1;
            }
        }
        else
        {
            nint len = length;
            for (index = 0; index < len; index++)
            {
                if ((object?)Unsafe.Add(ref searchSpace, index) is null)
                {
                    goto Found;
                }
            }
        }

        return false;

    Found:
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ContainsValueType<T>(ref T searchSpace, T value, int length) where T : struct, IEquatable<T>
    {
        return NonPackedContainsValueType(ref searchSpace, value, length);
    }

    internal static bool NonPackedContainsValueType<T>(ref T searchSpace, T value, int length) where T : struct, IEquatable<T>
    {
/**
- Reference: https://github.com/dotnet/runtime/blob/9ffface2f3fa6fbbb427793c3230b1626a1fdd84/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1316
*/
        Debug.Assert(length >= 0, "Expected non-negative length");
        Debug.Assert(value is byte or short or int or long or nint, "Expected caller to normalize to one of these types");

        nuint offset = 0;

        while (length >= 8)
        {
            length -= 8;

            if (Unsafe.Add(ref searchSpace, offset).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 1).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 2).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 3).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 4).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 5).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 6).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 7).Equals(value))
            {
                return true;
            }

            offset += 8;
        }

        if (length >= 4)
        {
            length -= 4;

            if (Unsafe.Add(ref searchSpace, offset).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 1).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 2).Equals(value)
                || Unsafe.Add(ref searchSpace, offset + 3).Equals(value))
            {
                return true;
            }

            offset += 4;
        }

        while (length > 0)
        {
            length -= 1;

            if (Unsafe.Add(ref searchSpace, offset).Equals(value)) return true;

            offset += 1;
        }

        return false;
    }


    /// <summary>
    /// <see cref="System.MemoryExtensions.Contains{T}(System.ReadOnlySpan{T},T)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.contains?view=net-10.0#system-memoryextensions-contains-1(system-readonlyspan((-0))-0)">
    /// (doc)
    /// </a>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
    {
/** 
- Reference: https://github.com/dotnet/runtime/blob/9ffface2f3fa6fbbb427793c3230b1626a1fdd84/src/libraries/System.Private.CoreLib/src/System/MemoryExtensions.cs#L321
*/
        {
            if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            {
                return ContainsValueType(
                    ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)),
                    UnsafeTheory.BitCast<T, byte>(value),
                    span.Length);
            }
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            {
                return ContainsValueType(
                    ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)),
                    UnsafeTheory.BitCast<T, short>(value),
                    span.Length);
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            {
                return ContainsValueType(
                    ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)),
                    UnsafeTheory.BitCast<T, int>(value),
                    span.Length);
            }
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            {
                return ContainsValueType(
                    ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)),
                    UnsafeTheory.BitCast<T, long>(value),
                    span.Length);
            }
            else if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
            {
                return ContainsValueType(
                    ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)),
                    UnsafeTheory.BitCast<T, long>(value),
                    span.Length);
            }
        }

        return Contains(ref MemoryMarshal.GetReference(span), value, span.Length);
    }
}
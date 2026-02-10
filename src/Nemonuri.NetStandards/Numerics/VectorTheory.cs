using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nemonuri.NetStandards.Numerics;

public static class VectorTheory
{
    /// <summary>
    /// <see cref="System.Numerics.Vector{T}.IsSupported" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.issupported?view=net-10.0">
    /// (doc)
    /// </a>
    public static bool IsSupported<T>() where T : struct
    {
/**
## Remarks

The type `T` can be any of the following numeric types:

| C# keywords |	Framework Type |
| ----------- | -------------- |
| sbyte | SByte |
| byte | Byte |
| short | Int16 |
| ushort | UInt16 |
| int | Int32 |
| uint | UInt32 |
| long | Int64 |
| ulong | UInt64 |
| float | Single |
| double | Double |

- https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.-ctor
*/
        return
            (typeof(T) == typeof(sbyte)) ||
            (typeof(T) == typeof(byte)) ||
            (typeof(T) == typeof(short)) ||
            (typeof(T) == typeof(ushort)) ||
            (typeof(T) == typeof(int)) ||
            (typeof(T) == typeof(uint)) ||
            (typeof(T) == typeof(long)) ||
            (typeof(T) == typeof(ulong)) ||
            (typeof(T) == typeof(float)) ||
            (typeof(T) == typeof(double)) ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GuardTypeIsSupported<T>() where T : struct
    {
        if (!IsSupported<T>()) { throw new NotSupportedException(string.Format("Type {0} not supported.", typeof(T))); }
    }

#if NETSTANDARD2_0
    /// <summary>
    /// <see cref="System.Numerics.Vector{T}(System.Span{T})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector-1.-ctor?view=net-10.0#system-numerics-vector-1-ctor(system-span((-0)))">
    /// (doc)
    /// </a>
    /// </summary>
    public static Vector<T> Create<T>(Span<T> values) where T : struct
    {
/**
- Reference: https://github.com/dotnet/maintenance-packages/blob/main/src/System.Numerics.Vectors/src/System/Numerics/Vector.cs#L771
*/
        GuardTypeIsSupported<T>();

        if (values.Length < Vector<T>.Count)
        {
            throw new IndexOutOfRangeException(string.Format("{0} did not contain at least {1} elements.", nameof(values), Vector<T>.Count));
        }

        return UncheckedLoadUnsafe(ref MemoryMarshal.GetReference(values));
    }
#endif

    /// <summary>
    /// No type check, No ref 'readonly' check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector<T> UncheckedLoadUnsafe<T>(ref T source) where T : struct
    {
/**
- Reference: https://github.com/dotnet/runtime/blob/500d83e287aea93e8361e1fbbb8fe17c6ea5cf07/src/libraries/System.Private.CoreLib/src/System/Numerics/Vector.cs#L1986
*/
        ref byte address = ref Unsafe.As<T, byte>(ref source);
        return Unsafe.ReadUnaligned<Vector<T>>(ref address);
    }

    /// <summary>
    /// <see cref="System.Numerics.Vector.LoadUnsafe{T}(ref readonly T)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector.loadunsafe?view=net-10.0#system-numerics-vector-loadunsafe-1(-0@)">
    /// (doc)
    /// </a>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector<T> LoadUnsafe<T>(ref readonly T source) where T : struct
    {
        GuardTypeIsSupported<T>();

        return UncheckedLoadUnsafe(ref Unsafe.AsRef(in source));
    }

    /// <summary>
    /// No type check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void UncheckedStoreUnsafe<T>(Vector<T> source, ref T destination) where T : struct
    {
/**
- Reference: https://github.com/dotnet/runtime/blob/500d83e287aea93e8361e1fbbb8fe17c6ea5cf07/src/libraries/System.Private.CoreLib/src/System/Numerics/Vector.cs#L3029
*/
        ref byte address = ref Unsafe.As<T, byte>(ref destination);
        Unsafe.WriteUnaligned(ref address, source);
    }

    /// <summary>
    /// <see cref="System.Numerics.Vector.StoreUnsafe{T}(System.Numerics.Vector{T}, ref T)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.numerics.vector.storeunsafe?view=net-10.0#system-numerics-vector-storeunsafe-1(system-numerics-vector((-0))-0@)">
    /// (doc)
    /// </a>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void StoreUnsafe<T>(Vector<T> source, ref T destination) where T : struct
    {
        GuardTypeIsSupported<T>();

        UncheckedStoreUnsafe(source, ref destination);
    }

}

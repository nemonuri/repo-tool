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
        if (!IsSupported<T>()) { throw new NotSupportedException(string.Format("Type {0} not supported.", typeof(T))); }

        if (values.Length < Vector<T>.Count)
        {
            throw new IndexOutOfRangeException(string.Format("{0} did not contain at least {1} elements.", nameof(values), Vector<T>.Count));
        }

        return Unsafe.ReadUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }
#endif
}

using System.Runtime.CompilerServices;

namespace Nemonuri.NetStandards.Runtime.CompilerServices;

public static unsafe class UnsafeTheory
{
    /// <summary>
    /// <see cref="System.Runtime.CompilerServices.Unsafe.BitCast{TFrom,TTo}(TFrom)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafe.bitcast?view=net-10.0#system-runtime-compilerservices-unsafe-bitcast-2(-0)">
    /// (doc)
    /// </a>
    /// </summary>
#pragma warning disable CS8500
    public static TTo BitCast<TFrom, TTo>(TFrom source)
    {
/** 
- Reference: https://github.com/dotnet/runtime/blob/9ffface2f3fa6fbbb427793c3230b1626a1fdd84/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/Unsafe.cs#L254C1-L263C10
*/
        if (sizeof(TFrom) != sizeof(TTo) || !typeof(TFrom).IsValueType || !typeof(TTo).IsValueType)
        {
            throw new NotSupportedException(string.Format("The size of {0} and {1} are not the same.", typeof(TFrom), typeof(TTo)));
        }
        return Unsafe.ReadUnaligned<TTo>(ref Unsafe.As<TFrom, byte>(ref source));
    }
#pragma warning restore CS8500
}

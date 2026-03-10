using System.Text;
using System.Runtime.CompilerServices;
using M = System.Runtime.InteropServices.MemoryMarshal;

namespace Nemonuri.NetStandards.Text;

public static class EncodingTheory
{
#if !NETSTANDARD2_1_OR_GREATER
    internal static class MemoryMarshal
    {
        // Copy from: https://github.com/dotnet/runtime/blob/dc5fd7a8dce8309e4add8fd4bd5d8718f221b15a/src/libraries/System.Private.CoreLib/src/System/Runtime/InteropServices/MemoryMarshal.cs#L88-L100

        /// <summary>
        /// Returns a reference to the 0th element of the Span. If the Span is empty, returns a reference to fake non-null pointer. Such a reference can be used
        /// for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe ref T GetNonNullPinnableReference<T>(Span<T> span) => ref (span.Length != 0) ? ref M.GetReference(span) : ref Unsafe.AsRef<T>((void*)1);

        /// <summary>
        /// Returns a reference to the 0th element of the ReadOnlySpan. If the ReadOnlySpan is empty, returns a reference to fake non-null pointer. Such a reference
        /// can be used for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe ref T GetNonNullPinnableReference<T>(ReadOnlySpan<T> span) => ref (span.Length != 0) ? ref M.GetReference(span) : ref Unsafe.AsRef<T>((void*)1);
    }

    /// <summary>
    /// <see cref="System.Text.Encoding.GetChars(System.ReadOnlySpan{byte}, System.Span{char})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getchars?view=net-10.0#system-text-encoding-getchars(system-readonlyspan((system-byte))-system-span((system-char)))">
    /// (doc)
    /// </a>
    /// </summary>
    public static unsafe int GetChars(Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        // Copy from: https://github.com/dotnet/runtime/blob/dc5fd7a8dce8309e4add8fd4bd5d8718f221b15a/src/libraries/System.Private.CoreLib/src/System/Text/Encoding.cs#L876

        fixed (byte* bytesPtr = &MemoryMarshal.GetNonNullPinnableReference(bytes))
        fixed (char* charsPtr = &MemoryMarshal.GetNonNullPinnableReference(chars))
        {
            return encoding.GetChars(bytesPtr, bytes.Length, charsPtr, chars.Length);
        }
    }

    /// <summary>
    /// <see cref="System.Text.Encoding.GetBytes(System.ReadOnlySpan{char}, System.Span{byte})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getbytes?view=net-10.0#system-text-encoding-getbytes(system-readonlyspan((system-char))-system-span((system-byte)))">
    /// (doc)
    /// </a>
    /// </summary>
    public static unsafe int GetBytes(Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        // Copy from: https://github.com/dotnet/runtime/blob/dc5fd7a8dce8309e4add8fd4bd5d8718f221b15a/src/libraries/System.Private.CoreLib/src/System/Text/Encoding.cs#L727-L734

        fixed (char* charsPtr = &MemoryMarshal.GetNonNullPinnableReference(chars))
        fixed (byte* bytesPtr = &MemoryMarshal.GetNonNullPinnableReference(bytes))
        {
            return encoding.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
        }
    }
#endif
}

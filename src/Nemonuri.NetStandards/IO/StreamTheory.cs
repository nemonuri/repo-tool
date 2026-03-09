using System.Buffers;

namespace Nemonuri.NetStandards.IO;

public static class StreamTheory
{
#if !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// <see cref="System.IO.Stream.Write(System.ReadOnlySpan{System.Byte})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.write?view=net-10.0#system-io-stream-write(system-readonlyspan((system-byte)))">
    /// (doc)
    /// </a>
    /// </summary>
    public static void Write(Stream stream, ReadOnlySpan<byte> buffer)
    {
        // Copy from: https://github.com/dotnet/runtime/blob/dc5fd7a8dce8309e4add8fd4bd5d8718f221b15a/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs#L912

        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(sharedBuffer);
            stream.Write(sharedBuffer, 0, buffer.Length);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }
    }
#endif
}

public static class BinaryWriterTheory
{
#if !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// <see cref="System.IO.BinaryWriter.Write(System.ReadOnlySpan{System.Byte})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.io.binarywriter.write?view=net-10.0#system-io-binarywriter-write(system-readonlyspan((system-byte)))">
    /// (doc)
    /// </a>
    /// </summary>
    public static void Write(BinaryWriter writer, ReadOnlySpan<byte> buffer)
    {
        StreamTheory.Write(writer.BaseStream, buffer);
    }
#endif
}
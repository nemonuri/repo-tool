namespace Nemonuri.ByteChars.IO;

public static class StreamTheory
{
    public static void WriteByteSpan(Stream stream, ReadOnlySpan<byte> byteSpan)
    {
        Guard.IsNotNull(stream);
#if NETSTANDARD2_1_OR_GREATER
        stream.Write(byteSpan);
#else
        Nemonuri.NetStandards.IO.StreamTheory.Write(stream, byteSpan);
#endif
    }
}

public static class BinaryWriterTheory
{
    public static void WriteByteSpan(BinaryWriter writer, ReadOnlySpan<byte> byteSpan)
    {
        Guard.IsNotNull(writer);
#if NETSTANDARD2_1_OR_GREATER
        writer.Write(byteSpan);
#else
        Nemonuri.NetStandards.IO.BinaryWriterTheory.Write(writer, byteSpan);
#endif
    }
}

public static class StandardIOTheory
{
//--- Output field ---
    private static volatile BinaryWriter? s_output = null;

    public static BinaryWriter Output
    {
        get
        {
            if (s_output == null)
            {
                BinaryWriter newItem = new (Console.OpenStandardOutput());
                _ = Interlocked.CompareExchange(ref s_output, newItem, null);
                if (!Object.ReferenceEquals(s_output, newItem)) { newItem.Dispose(); }
            }
            return s_output;
        }
    }

    public static void RefreshOutput()
    {
        var prevItem = Interlocked.Exchange(ref s_output, null);
        prevItem?.Dispose();
    }
//---|

//--- Error field ---
    private static volatile BinaryWriter? s_error = null;

    public static BinaryWriter Error
    {
        get
        {
            if (s_error == null)
            {
                BinaryWriter newItem = new (Console.OpenStandardError());
                _ = Interlocked.CompareExchange(ref s_error, newItem, null);
                if (!Object.ReferenceEquals(s_error, newItem)) { newItem.Dispose(); }
            }
            return s_error;
        }
    }

    public static void RefreshError()
    {
        var prevItem = Interlocked.Exchange(ref s_error, null);
        prevItem?.Dispose();
    }
//---|

//--- Input field ---
    private static volatile BinaryReader? s_Input = null;

    public static BinaryReader Input
    {
        get
        {
            if (s_Input == null)
            {
                BinaryReader newItem;
                {
                    try
                    {
                        newItem = new (Console.OpenStandardInput());
                    }
                    catch
                    {
                        newItem = new (Stream.Null);
                    }
                }
                _ = Interlocked.CompareExchange(ref s_Input, newItem, null);
                if (!Object.ReferenceEquals(s_Input, newItem)) { newItem.Dispose(); }
            }
            return s_Input;
        }
    }

    public static void RefreshInput()
    {
        var prevItem = Interlocked.Exchange(ref s_Input, null);
        prevItem?.Dispose();
    }
//---|
}

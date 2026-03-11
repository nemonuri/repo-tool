using Nemonuri.Buffers;
using Nemonuri.ByteChars.Internal;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;

namespace Nemonuri.ByteChars.Numerics;

public static class Int32Theory
{
    public static ArraySegment<byte> ToMutableByteString(int source)
    {
        Span<byte> dest = stackalloc byte[InternalConstants.MaxDecimalInt32ByteStringLength];
        var ok = Utf8Formatter.TryFormat(source, dest, out int bytesWritten, new StandardFormat('D'));
        Debug.Assert(ok);
        return new(dest[..bytesWritten].ToArray());
    }
}

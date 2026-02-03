namespace Nemonuri.NetStandards.Text;

using System.Text;

public static class EncodingExtensions
{
#if !NETSTANDARD2_1_OR_GREATER
    extension(Encoding encoding)
    {
        public int GetCharCount(ReadOnlySpan<byte> bytes)
        {
            unsafe
            {
                fixed (byte* pBytes = bytes)
                {
                    return encoding.GetCharCount(pBytes, bytes.Length);
                }
            }
        }
    }
#endif
}

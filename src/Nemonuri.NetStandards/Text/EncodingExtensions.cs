using System.Text;

namespace Nemonuri.NetStandards.Text;

#if false

public static class EncodingExtensions
{

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

}

#endif
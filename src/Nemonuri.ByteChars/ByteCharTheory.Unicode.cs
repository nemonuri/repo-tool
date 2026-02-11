namespace Nemonuri.ByteChars;

public static partial class ByteCharTheory
{

    public static bool TryUtf8ByteCharToUtf16DotNetChar(byte byteChar, out char utf16DotNetChar)
    {
        if (!IsValid(byteChar))
        {
            utf16DotNetChar = default;
            return false;
        }

        utf16DotNetChar = (char)byteChar;
        return true;
    }
}
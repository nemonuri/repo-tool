namespace Nemonuri.OCamlDotNet;

public static class ByteCharConstants
{
    // Reference: https://en.wikipedia.org/wiki/ASCII#Character_set

    /// <summary>'\'(0x5c)</summary>
    public const byte AsciiBackslash = (byte)'\\';

    /// <summary>'"'(0x22)</summary>
    public const byte AsciiDoubleQuote = (byte)'\"';

    /// <summary>"'"(0x27)</summary>
    public const byte AsciiSingleQuote = (byte)'\'';

    /// <summary>'\n'(0x0a)</summary>
    public const byte AsciiLineFeed = (byte)'\n';

    /// <summary>'\r'(0x0d)</summary>
    public const byte AsciiCarriageReturn = (byte)'\r';

    /// <summary>'\t'(0x09)</summary>
    public const byte AsciiHorizontalTabulation = (byte)'\t';

    /// <summary>'\b'(0x08)</summary>
    public const byte AsciiBackspace = (byte)'\b';

    /// <summary>' '(0x20)</summary>
    public const byte AsciiSpace = (byte)' ';

    /// <summary>0x20. Inclusive</summary>
    public const byte AsciiPrintableMinimum = AsciiSpace;

    /// <summary>0x7e. Inclusive</summary>
    public const byte AsciiPrintableMaximum = (byte)'~';

    /// <summary>vertical tab (0x0b)</summary>
    public const byte AsciiVerticalTabulation = 0x0b;

    /// <summary>form feed (0x0c)</summary>
    public const byte AsciiFormFeed = 0x0c;

    /// <summary>0x21. Inclusive</summary>
    public const byte AsciiGraphicCharacterMinimum = 0x21;

    /// <summary>0x7e. Inclusive</summary>
    public const byte AsciiGraphicCharacterMaximum = AsciiPrintableMaximum;

    /// <summary>'\0'(0x00)</summary>
    public const byte AsciiNull = (byte)'\0';

    /// <summary>Delete character. (0x7f)</summary>
    public const byte AsciiDelete = 0x7f;

    /// <summary>0x00. Inclusive</summary>
    public const byte AsciiMinimum = AsciiNull;

    /// <summary>0x7f. Inclusive</summary>
    public const byte AsciiMaximum = AsciiDelete;

    /// <summary>'A'</summary>
    public const byte AsciiUpperA = (byte)'A';

    /// <summary>'a'</summary>
    public const byte AsciiLowerA = (byte)'a';

    /// <summary>'F'</summary>
    public const byte AsciiUpperF = (byte)'F';

    /// <summary>'f'</summary>
    public const byte AsciiLowerF = (byte)'f';

    /// <summary>'Z'</summary>
    public const byte AsciiUpperZ = (byte)'Z';

    /// <summary>'z'</summary>
    public const byte AsciiLowerZ = (byte)'z';

    /// <summary>'0'</summary>
    public const byte AsciiDigit0 = (byte)'0';

    /// <summary>'9'</summary>
    public const byte AsciiDigit9 = (byte)'9';

    public const byte AsciiUpperToLowerDistance = AsciiLowerA - AsciiUpperA;
}

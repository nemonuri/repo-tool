namespace Nemonuri.OCamlDotNet;

public static class LexicalTheory
{
    // Reference: https://en.wikipedia.org/wiki/ASCII#Character_set

    /// <summary>' \ '</summary>
    public const byte AsciiBackslash = 0x5c;

    /// <summary>' " '</summary>
    public const byte AsciiDoubleQuote = 0x22;

    /// <summary>' ' '</summary>
    public const byte AsciiSingleQuote = 0x27;

    /// <summary>' \n '</summary>
    public const byte AsciiLineFeed = 0x0a;

    /// <summary>' \r '</summary>
    public const byte AsciiCarriageReturn = 0x0d;

    /// <summary>' \t '</summary>
    public const byte AsciiHorizontalTabulation = 0x09;

    /// <summary>' \b '</summary>
    public const byte AsciiBackspace = 0x08;

    /// <summary>A white space character.</summary>
    public const byte AsciiSpace = 0x20;

    /// <summary>Inclusive</summary>
    public const byte AsciiPrintableMinimum = 0x20;

    /// <summary>Inclusive</summary>
    public const byte AsciiPrintableMaximum = 0x7e;

    /// <summary>vertical tab (0x0B)</summary>
    public const byte AsciiVerticalTabulation = 0x0b;

    /// <summary>form feed (0x0C)</summary>
    public const byte AsciiFormFeed = 0x0c;

    /// <summary>Inclusive</summary>
    public const byte AsciiGraphicCharacterMinimum = 0x21;

    /// <summary>Inclusive</summary>
    public const byte AsciiGraphicCharacterMaximum = AsciiPrintableMaximum;
}


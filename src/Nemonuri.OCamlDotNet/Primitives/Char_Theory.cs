namespace Nemonuri.OCamlDotNet;

public readonly partial struct Char
{
    public static readonly Char UpperA = new((byte)'A');

    public static readonly Char LowerA = new((byte)'a');

    public static readonly Char UpperF = new((byte)'F');

    public static readonly Char LowerF = new((byte)'f');

    public static readonly Char UpperZ = new((byte)'Z');

    public static readonly Char LowerZ = new((byte)'z');

    public static readonly Char MinValue = new(byte.MinValue);

    public static readonly Char MaxValue = new(byte.MaxValue);

    public static readonly Char Digit0 = new((byte)'0');

    public static readonly Char Digit9 = new((byte)'9');

    public static readonly Char PrintableMinValue = new(LexicalTheory.AsciiPrintableMinimum);

    public static readonly Char PrintableMaxValue = new(LexicalTheory.AsciiPrintableMaximum);

    public static readonly Char GraphicMinValue = new(LexicalTheory.AsciiGraphicCharacterMinimum);

    public static readonly Char GraphicMaxValue = new(LexicalTheory.AsciiGraphicCharacterMaximum);

    public static readonly Char Space = new(LexicalTheory.AsciiSpace);
}
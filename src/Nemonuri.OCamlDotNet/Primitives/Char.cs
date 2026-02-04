namespace Nemonuri.OCamlDotNet;

using System.Diagnostics;
using L = Nemonuri.OCamlDotNet.LexicalTheory;

/// <remarks>
/// <list type="bullet">
///   <item>
///     <term>Reference</term>
///     <description>https://ocaml.org/manual/5.4/api/Char.html</description>
///   </item>
/// </list>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Char : IComparable<Char>, IEquatable<Char>
{
    private readonly byte _value;

    public Char(byte value)
    {
        _value = value;
    }

    public static implicit operator Char(byte value) => new(value);

    public byte Value => _value;


    public int CompareTo(Char other) => _value.CompareTo(other._value);

    public static bool operator < (Char l, Char r) => l.CompareTo(r) < 0;

    public static bool operator > (Char l, Char r) => l.CompareTo(r) > 0;

    public static bool operator <= (Char l, Char r) => l.CompareTo(r) <= 0;

    public static bool operator >= (Char l, Char r) => l.CompareTo(r) >= 0;


    public bool Equals(Char other) => _value.Equals(other._value);

    public static bool operator == (Char l, Char r) => l.Equals(r);

    public static bool operator != (Char l, Char r) => !(l == r);

    public override bool Equals(object? obj) => obj is Char other && Equals(other);
    
    public override int GetHashCode() => _value;
    

    public char ToDotNetChar() => Convert.ToChar(_value);

    public static implicit operator char(Char self) => self.ToDotNetChar();


    /// <exception cref="System.OverflowException">
    /// <c>value</c> represents a number that is greater than <see cref="System.Byte.MaxValue">Byte.MaxValue</see>.
    /// </exception>
    public static Char FromDotNetChar(char dotnetChar) => new(Convert.ToByte(dotnetChar));

    /// <inheritdoc cref="FromDotNetChar" />
    public static explicit operator Char(char dotnetChar) => FromDotNetChar(dotnetChar);


    public override string ToString() => ToDotNetChar().ToString();

    /// <summary>
    /// Return a string representing the given character, with special characters escaped following the lexical conventions of OCaml. 
    /// All characters outside the ASCII printable range [<c>0x20</c>;<c>0x7E</c>] are escaped, as well as backslash, double-quote, and single-quote.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <term>Reference</term>
    ///     <description>https://ocaml.org/manual/5.4/lex.html#escape-sequence</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public String ToEscaped()
    {
        static ReadOnlySpan<byte> ByteToDecimalAscii(byte value, Span<byte> dest)
        {
            const int asciiZero = 0x30;

            Debug.Assert(dest.Length == 3);

            int curValue = value;
            Debug.Assert(curValue >= byte.MinValue);
            Debug.Assert(curValue <= byte.MaxValue);

            for (int i = 0; i < 3; i++)
            {
                curValue = Math.DivRem(curValue, 10, out int rem);
                dest[(3-1) - i] = (byte)(asciiZero + rem);
            }

            return dest;
        }

        return _value switch
        {
            L.AsciiBackslash => [L.AsciiBackslash, L.AsciiBackslash],
            L.AsciiDoubleQuote => [L.AsciiBackslash, L.AsciiDoubleQuote],
            L.AsciiSingleQuote => [L.AsciiBackslash, L.AsciiSingleQuote],
            L.AsciiLineFeed => [L.AsciiBackslash, .."n"u8],
            L.AsciiCarriageReturn => [L.AsciiBackslash, .."r"u8],
            L.AsciiHorizontalTabulation => [L.AsciiBackslash, .."t"u8],
            L.AsciiBackspace => [L.AsciiBackslash, .."b"u8],
            L.AsciiSpace => [L.AsciiBackslash, .." "u8],
            >= L.AsciiPrintableMinimum and <= L.AsciiPrintableMaximum => [this],
            _ => [L.AsciiBackslash, ..ByteToDecimalAscii(_value, stackalloc byte[3])]
        };
    }
}


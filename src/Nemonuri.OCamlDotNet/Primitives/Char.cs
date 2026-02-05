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
public readonly partial struct Char : IComparable<Char>, IEquatable<Char>, IComparable
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

    public int CompareTo(object? obj)
    {
        if (obj == null) {return 1;}

        if (obj is Char c) { return CompareTo(c); }
        
        throw new ArgumentException("Argument must be Nemonuri.OCamlDotNet.Char");
    }
     


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
        static void ByteToDecimalAscii(byte value, Span<Char> dest)
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
        }

        static String CreateDefault(byte value, Span<Char> dest)
        {
            Debug.Assert(dest.Length == 4);

            dest[0] = L.AsciiBackslash;
            ByteToDecimalAscii(value, dest.Slice(1));
            return new(dest);
        }

        return _value switch
        {
            L.AsciiBackslash => [L.AsciiBackslash, L.AsciiBackslash],
            L.AsciiDoubleQuote => [L.AsciiBackslash, L.AsciiDoubleQuote],
            L.AsciiSingleQuote => [L.AsciiBackslash, L.AsciiSingleQuote],
            L.AsciiLineFeed => [L.AsciiBackslash, (byte)'n'],
            L.AsciiCarriageReturn => [L.AsciiBackslash, (byte)'r'],
            L.AsciiHorizontalTabulation => [L.AsciiBackslash, (byte)'t'],
            L.AsciiBackspace => [L.AsciiBackslash, (byte)'b'],
            L.AsciiSpace => [L.AsciiBackslash, (byte)' '],
            >= L.AsciiPrintableMinimum and <= L.AsciiPrintableMaximum => [this],
            _ => CreateDefault(_value, stackalloc Char[4])
        };
    }
}

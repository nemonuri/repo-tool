using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;

namespace Nemonuri.OCamlDotNet;

[CollectionBuilder(typeof(Nemonuri.OCamlDotNet.Extensions.StringExtensions), nameof(Nemonuri.OCamlDotNet.Extensions.StringExtensions.ToOCamlString))]
public readonly struct String : IEquatable<String>, IComparable<String>, IReadOnlyList<Char>
{
    private readonly ImmutableArray<Char> _value;

    public String(ImmutableArray<Char> value)
    {
        _value = value;
    }

    public String(IEnumerable<Char>? chars)
    {
        _value = chars is null ? [] : ImmutableArray.Create(chars.ToArray());
    }

    public String(ReadOnlySpan<Char> chars) : this(ImmutableArray.Create(chars))
    {}

    public String(ReadOnlySpan<byte> bytes) : this(Extensions.StringExtensions.ToOCamlChars(bytes))
    {}

    public ImmutableArray<Char> Value => _value;

    public ReadOnlySpan<Char> AsSpan() => _value.AsSpan();

    public int Length => _value.Length;

    public bool Equals(String other)
    {
        return AsSpan().SequenceEqual(other.AsSpan());
    }

    public static bool operator == (String l, String r) => l.Equals(r);

    public static bool operator != (String l, String r) => !(l == r);

    public override bool Equals(object? obj) => obj is String other && Equals(other);
    
    public override int GetHashCode()
    {
        return AsSpan().GetDjb2HashCode();
    }


    public int CompareTo(String other)
    {
        return AsSpan().SequenceCompareTo(other.AsSpan());
    }

    public Char this[int index] => _value[index];

    int IReadOnlyCollection<Char>.Count => Length;

    public ImmutableArray<Char>.Enumerator GetEnumerator() => _value.GetEnumerator();

    IEnumerator<Char> IEnumerable<Char>.GetEnumerator() => ((IEnumerable<Char>)_value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();


    public String Slice(int start, int length) => new(_value.Slice(start, length));


    public string ToDotNetString()
    {
        ReadOnlySpan<Char> thisAsSpan = AsSpan();
        Span<char> dotnetChars = stackalloc char[thisAsSpan.Length];
        for (int i = 0; i < dotnetChars.Length; i++)
        {
            dotnetChars[i] = thisAsSpan[i].ToDotNetChar();
        }
        return dotnetChars.ToString();
    }

    public static implicit operator string(String ocamlString) => ocamlString.ToDotNetString();

    public override string ToString() => ToDotNetString();


    /// <exception cref="System.OverflowException" />
    public static String FromDotNetString(string dotnetString)
    {
        Guard.IsNotNull(dotnetString);

        ReadOnlySpan<char> dotnetStringAsSpan = dotnetString.AsSpan();
        Span<Char> ocamlChars = stackalloc Char[dotnetStringAsSpan.Length];
        for (int i = 0; i < ocamlChars.Length; i++)
        {
            ocamlChars[i] = (Char)dotnetStringAsSpan[i];
        }
        return new(ocamlChars);
    }

    /// <inheritdoc cref="FromDotNetString" />
    public static explicit operator String(string dotnetString) => FromDotNetString(dotnetString);

    /// <exception cref="ArgumentNullException">Thrown if <paramref name="encoding"/> is <see langword="null"/>.</exception>
    /// <exception cref="EncoderFallbackException" />
    public static String FromDotNetStringAndEncoding(string dotnetString, Encoding encoding)
    {
        Guard.IsNotNull(dotnetString);
        Guard.IsNotNull(encoding);

        var bytes = encoding.GetBytes(dotnetString);
        return new(bytes.AsSpan());
    }
}

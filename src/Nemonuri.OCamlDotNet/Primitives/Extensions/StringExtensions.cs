namespace Nemonuri.OCamlDotNet.Extensions;

public static class StringExtensions
{
    extension(ReadOnlySpan<byte> bytes)
    {
        public ReadOnlySpan<Char> ToOCamlChars() => MemoryMarshal.Cast<byte, Char>(bytes);

        public String ToOCamlString() => new(bytes);
    }

    extension(Span<byte> bytes)
    {
        public Span<Char> ToOCamlChars() => MemoryMarshal.Cast<byte, Char>(bytes);

        public String ToOCamlString() => new(bytes);
    }

    extension(ReadOnlySpan<Char> ocamlChars)
    {
        public ReadOnlySpan<byte> ToBytes() => MemoryMarshal.Cast<Char, byte>(ocamlChars);

        public String ToOCamlString() => new(ocamlChars);
    }

    extension(Span<Char> ocamlChars)
    {
        public Span<byte> ToBytes() => MemoryMarshal.Cast<Char, byte>(ocamlChars);

        public String ToOCamlString() => new(ocamlChars);
    }
}

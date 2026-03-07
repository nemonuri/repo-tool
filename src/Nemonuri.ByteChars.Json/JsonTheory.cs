using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.ByteChars.Json;

public static class JsonDocumentTheory
{
    public static bool TryParseUtf8Span(ReadOnlySpan<byte> source, [NotNullWhen(true)] out JsonDocument? document)
    {
        Utf8JsonReader reader = new(source);
        return JsonDocument.TryParseValue(ref reader, out document);
    }
}

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.OCamlDotNet.Xunit;

public class StringSerializer() : IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue)
    {
        Debug.Assert(type == typeof(String));
        return String.FromDotNetString(serializedValue);
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = default;
        return type == typeof(String);
    }

    public string Serialize(object value)
    {
        return ((String)value).ToDotNetString();
    }
}
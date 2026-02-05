using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.OCamlDotNet.Primitives.UnitTests;

public class CharSerializer() : IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue)
    {
        Debug.Assert(type == typeof(Char));
        Debug.Assert(serializedValue.Length == 1);
        return new Char(Convert.ToByte(serializedValue[0]));
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = default;
        return type == typeof(Char);
    }

    public string Serialize(object value)
    {
        return ((Char)value).ToDotNetChar().ToString();
    }
}

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
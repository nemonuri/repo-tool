using System.Text;

namespace Nemonuri.NetStandards.Text;

public static class StringBuilderTheory
{
#if !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// <see cref="System.Text.StringBuilder.Append(System.ReadOnlySpan{char})" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.text.stringbuilder.append?view=net-10.0#system-text-stringbuilder-append(system-readonlyspan((system-char)))">
    /// (doc)
    /// </a>
    /// </summary>
    public unsafe static StringBuilder AppendSpan(StringBuilder builder, ReadOnlySpan<char> chars)
    {
        fixed (char* ptr = chars)
        {
            builder.Append(ptr, chars.Length);
        }
        return builder;
    }
#endif
}
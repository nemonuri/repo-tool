namespace Nemonuri.ByteChars.Extensions;

public static class GuardExtensions
{
    extension(CommunityToolkit.Diagnostics.Guard)
    {
        public static void HasSizeEqualTo<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, [CallerArgumentExpression(nameof(left))] string name = "")
        {
            CommunityToolkit.Diagnostics.Guard.HasSizeEqualTo(left, right.Length, name);
        }
    }
}

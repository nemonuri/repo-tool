namespace Nemonuri.ByteChars.Extensions;

public static class UnsafePinnedSpanPointerExtensions
{
    extension<T>(IUnsafePinnedSpanPointer<T> ptr) where T : unmanaged
    {
        public Span<T> LoadSpan() => UnsafePinnedSpanPointerTheory.LoadSpan(ptr);

        public ReadOnlySpan<T> LoadReadOnlySpan() => UnsafePinnedSpanPointerTheory.LoadReadOnlySpan(ptr);
    }
}

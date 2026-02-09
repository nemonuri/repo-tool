namespace Nemonuri.ByteChars.Extensions;

public static class ImmutableArrayBuilderExtensions
{
    extension<T>(ImmutableArray<T>.Builder array) where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetPinnableReference()
        {
            return ref Unsafe.AsRef(in array.ItemRef(0));
        }
    }
}
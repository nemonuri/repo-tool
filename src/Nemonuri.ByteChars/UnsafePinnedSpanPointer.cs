using D = Nemonuri.ByteChars.Diagnostics;

namespace Nemonuri.ByteChars;

public unsafe readonly struct UnsafePinnedSpanPointer<T> : IUnsafePinnedSpanPointer<T>
    where T : unmanaged
{
    public T* PinnedPointer {get;}

    public int SpanLength {get;}

    public UnsafePinnedSpanPointer(T* pinnedPointer, int spanLength)
    {
        PinnedPointer = pinnedPointer;
        SpanLength = spanLength;
    }

    public UnsafePinnedSpanPointer<T> Slice(int offset, int length)
    {
        D.Guard.GuardSliceArgumentsAreInValidRange(SpanLength, offset, length);
        int sizeT = Unsafe.SizeOf<T>();
        T* newPointer = (T*)Unsafe.AsPointer(ref Unsafe.AddByteOffset(ref Unsafe.AsRef<T>(PinnedPointer), (nint)(offset * sizeT)));
        return new (newPointer, length);
    }
}
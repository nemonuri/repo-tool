using Nemonuri.FixedSizes;

namespace Nemonuri.Transcodings.Utf8Encodings;

public interface IMaxLengthPremise<T, TFormat>
{
    int GetMaxLength(in T source, in TFormat format);
}

public readonly struct FixedMaxLength<T, TFormat, TFixedSize> : IMaxLengthPremise<T, TFormat>
    where TFixedSize : unmanaged, IFixedSizePremise
{
    public int GetMaxLength(in T source, in TFormat format) => (new TFixedSize()).FixedSize;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MaxLengthHandle<T, TFormat>(delegate*<in T, in TFormat, int> fp)
{
    private readonly delegate*<in T, in TFormat, int> _fp = fp;

    public bool HasValue => _fp != null;

    public int GetMaxLength(in T source, in TFormat format) => _fp(in source, in format);
}

public static class MaxLengthTheory
{
    extension<T, TFormat, TMaxLength>(TMaxLength)
        where TMaxLength : unmanaged, IMaxLengthPremise<T, TFormat>
    {
        public unsafe static MaxLengthHandle<T, TFormat> ToHandle()
        {
            static int Impl(in T source, in TFormat format) => (new TMaxLength()).GetMaxLength(in source, in format);

            return new(&Impl);
        }
    }

    extension<T, TFormat, TFixedSize>(TFixedSize)
        where TFixedSize : unmanaged, IFixedSizePremise
    {
        public static MaxLengthHandle<T, TFormat> FixedSizeToHandle() => MaxLengthTheory.ToHandle<T, TFormat, FixedMaxLength<T, TFormat, TFixedSize>>();
    }
}
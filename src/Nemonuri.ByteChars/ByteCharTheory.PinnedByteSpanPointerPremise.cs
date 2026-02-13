using CommunityToolkit.HighPerformance;
using Nemonuri.ByteChars.Extensions;
using Nemonuri.ByteChars.Internal;

namespace Nemonuri.ByteChars;

public static partial class ByteCharTheory
{
    public readonly struct PinnedByteSpanPointerPremise : IByteCharPremise<PinnedByteSpanPointerPremise, UnsafePinnedSpanPointer<byte>>
    {
        public bool LessThanOrEqualAll(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right) =>
            ByteCharSpanTheory.LessThanOrEqualAll(left.LoadSpan(), right.LoadSpan());

        public bool EqualsAll(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right) =>
            ByteCharSpanTheory.EqualsAll(left.LoadSpan(), right.LoadSpan());

        public UnsafePinnedSpanPointer<byte> Add(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            ByteCharSpanTheory.Add(left.LoadSpan(), right.LoadSpan());
            return left;
        }

        public UnsafePinnedSpanPointer<byte> Subtract(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            ByteCharSpanTheory.Subtract(left.LoadSpan(), right.LoadSpan());
            return left;
        }

        public UnsafePinnedSpanPointer<byte> Modulus(UnsafePinnedSpanPointer<byte> left, UnsafePinnedSpanPointer<byte> right)
        {
            ByteCharSpanTheory.Modulus(left.LoadSpan(), right.LoadSpan());
            return left;
        }

        public unsafe UnsafePinnedSpanPointer<byte> GetTemporaryConstant(byte value)
        {
            var (b, l) = ByteCharSpanTheory.GetTemporaryConstantLocation(value);
            return new((byte*)Unsafe.AsPointer(ref b.DangerousGetReferenceAt(l)), 1);
        }

        public bool TryDecomposeToReadOnlyByteSpan(UnsafePinnedSpanPointer<byte> source, out ReadOnlySpan<byte> readOnlyByteSpan)
        {
            readOnlyByteSpan = source.LoadSpan();
            return true;
        }

        public bool TryDecomposeToByteSpan(UnsafePinnedSpanPointer<byte> source, out Span<byte> byteSpan, [MaybeNullWhen(false)] out object? aux)
        {
            byteSpan = source.LoadSpan();
            aux = default;
            return true;
        }

        public unsafe delegate*<ReadOnlySpan<byte>, object?, UnsafePinnedSpanPointer<byte>> ComposeFromByteSpan => null;
    }
}

using CommunityToolkit.HighPerformance;
using Nemonuri.OCamlDotNet.Extensions;
using Nemonuri.OCamlDotNet.Internal;

namespace Nemonuri.OCamlDotNet;

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

        public bool TryUnsafeDecomposeToByteSpan(UnsafePinnedSpanPointer<byte> composed, out Span<byte> unsafeBytes)
        {
            unsafeBytes = composed.LoadSpan();
            return true;
        }

        public unsafe UnsafePinnedSpanPointer<byte> GetTemporaryConstant(byte value)
        {
            var (b, l) = ByteCharSpanTheory.GetTemporaryConstantLocation(value);
            return new((byte*)Unsafe.AsPointer(ref b.DangerousGetReferenceAt(l)), 1);
        }
    }
}

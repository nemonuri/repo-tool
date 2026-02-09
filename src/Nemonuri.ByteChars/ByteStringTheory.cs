using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;
using static Nemonuri.ByteChars.Extensions.ImmutableArrayBuilderExtensions;

namespace Nemonuri.ByteChars;

public static unsafe class ByteStringTheory
{
    internal const int ByteCharStackLimitSize = 512; // .NET 은 Vector512 를 지원한다.

#if NETSTANDARD2_1_OR_GREATER
    private static Span<byte> BuilderToSpan(ImmutableArray<byte>.Builder builder)
    {
        return MemoryMarshal.CreateSpan(ref builder.GetPinnableReference(), builder.Capacity);
    }
#endif

    private static void UnsafeUpdateBuilderWithAux<TAux>
    (
        ImmutableArray<byte>.Builder builder,
        TAux aux,
        delegate*<Span<byte>, TAux, void> updater
    )
    {
#if NETSTANDARD2_1_OR_GREATER
        updater(BuilderToSpan(builder), aux);
#else
        fixed (void* p = builder)
        {
            updater(new (p, builder.Capacity), aux);
        }
#endif
    }

    public static ImmutableArray<byte> CreateConstantInitialized(int length, byte initialValue)
    {
        Guard.IsGreaterThanOrEqualTo(length, 0);
        if (length <= Sls.GetFixedSize())
        {
            Span<byte> bytes = stackalloc byte[length];
            bytes.Fill(initialValue);
            return bytes.ToImmutableArray();
        }
        else
        {
            static void Updater(Span<byte> bytes, byte initialValue0)
            {
                bytes.Fill(initialValue0);
            }
            
            var builder = ImmutableArray.CreateBuilder<byte>(initialCapacity: length);
            UnsafeUpdateBuilderWithAux(builder, initialValue, &Updater);
            return builder.DrainToImmutable();
        }
    }

    public static ImmutableArray<byte> CreateInitialized(int length, Func<int, byte> initializer)
    {
        Guard.IsGreaterThanOrEqualTo(length, 0);
        Guard.IsNotNull(initializer);

        static void Updater(Span<byte> bytes, Func<int, byte> initializer0)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = initializer0.Invoke(i);
            }
        }

        var builder = ImmutableArray.CreateBuilder<byte>(initialCapacity: length);
        UnsafeUpdateBuilderWithAux(builder, initializer, &Updater);
        return builder.DrainToImmutable();
    }

    public static ImmutableArray<byte> Empty => [];
}

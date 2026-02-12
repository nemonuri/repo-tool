using Nemonuri.ByteChars.Internal;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;
using static Nemonuri.ByteChars.Extensions.ImmutableArrayBuilderExtensions;

namespace Nemonuri.ByteChars;

public static unsafe partial class ByteStringTheory
{
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

    /// <exception cref="System.ArgumentOutOfRangeException" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GuardLengthIsInValidRange(int length)
    {
        Guard.IsInRange(length, 0, ByteStringConstants.MaxLength);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    private static ImmutableArray<byte> UnsafeCreateFixedLength<TAux>(int length, TAux aux, delegate*<Span<byte>, TAux, void> updater)
    {
        GuardLengthIsInValidRange(length);
        if (length <= Sls.GetFixedSize())
        {
            Span<byte> bytes = stackalloc byte[length];
            updater(bytes, aux);
            return bytes.ToImmutableArray();
        }
        else
        {
            var builder = ImmutableArray.CreateBuilder<byte>(initialCapacity: length);
            UnsafeUpdateBuilderWithAux(builder, aux, updater);
            return builder.DrainToImmutable();
        }
    }

    private static void UnsafeUpdateBuilderWithAuxReadOnlySpan<TAux>
    (
        ImmutableArray<byte>.Builder builder,
        ReadOnlySpan<TAux> auxSpan,
        delegate*<Span<byte>, ReadOnlySpan<TAux>, void> updater
    )
    {
#if NETSTANDARD2_1_OR_GREATER
        updater(BuilderToSpan(builder), auxSpan);
#else
        fixed (void* p = builder)
        {
            updater(new (p, builder.Capacity), auxSpan);
        }
#endif
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    private static ImmutableArray<byte> UnsafeCreateFixedLengthByReadOnlySpan<TAux>
    (
        ReadOnlySpan<TAux> auxSpan, 
        delegate*<Span<byte>, ReadOnlySpan<TAux>, void> updater
    )
    {
        int length = auxSpan.Length;
        GuardLengthIsInValidRange(length);
        
        if (length <= Sls.GetFixedSize())
        {
            Span<byte> bytes = stackalloc byte[length];
            updater(bytes, auxSpan);
            return bytes.ToImmutableArray();
        }
        else
        {
            var builder = ImmutableArray.CreateBuilder<byte>(initialCapacity: length);
            UnsafeUpdateBuilderWithAuxReadOnlySpan(builder, auxSpan, updater);
            return builder.DrainToImmutable();
        }
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> FromInitialValue(int length, byte initialValue)
    {
        static void Updater(Span<byte> bytes0, byte initialValue0)
        {
            bytes0.Fill(initialValue0);
        }

        return UnsafeCreateFixedLength(length, initialValue, &Updater);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> FromInitializer(int length, Func<int, byte> initializer)
    {
        static void Updater(Span<byte> bytes, Func<int, byte> initializer0)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = initializer0.Invoke(i);
            }
        }

        Guard.IsNotNull(initializer);
        return UnsafeCreateFixedLength(length, initializer, &Updater);
    }

    public static ImmutableArray<byte> Empty => [];

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> DotNetCharSpanToByteString(params ReadOnlySpan<char> dotNetChars)
    {
        static void Updater(Span<byte> bytes, ReadOnlySpan<char> dotNetChars0)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = ByteCharTheory.DotNetCharToByteChar(dotNetChars0[i]);
            }
        }

        return UnsafeCreateFixedLengthByReadOnlySpan(dotNetChars, &Updater);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> CheckedDotNetCharSpanToByteString(params ReadOnlySpan<char> dotNetChars)
    {
        static void Updater(Span<byte> bytes, ReadOnlySpan<char> dotNetChars0)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = ByteCharTheory.CheckedDotNetCharToByteChar(dotNetChars0[i]);
            }
        }

        return UnsafeCreateFixedLengthByReadOnlySpan(dotNetChars, &Updater);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> DotNetStringToByteString(string? dotNetString)
    {
        static void Updater(Span<byte> bytes, string dotNetString0)
        {
            for (int i = 0; i < dotNetString0.Length; i++)
            {
                bytes[i] = ByteCharTheory.DotNetCharToByteChar(dotNetString0[i]);
            }
        }

        if (dotNetString == null) { return Empty; }
        return UnsafeCreateFixedLength(dotNetString.Length, dotNetString, &Updater);
    }

    public static ImmutableArray<byte> DotNetStringToByteStringByEncoding(string? dotNetString, Encoding? encoding = null)
    {
        if (dotNetString == null) { return Empty; }
        Encoding ensuredEncoding = encoding ?? Encoding.Default;
        byte[] bytes = ensuredEncoding.GetBytes(dotNetString); // TODO: 최적화 - 현재는 같은 Array 가 두 번 생성됨
        return bytes.ToImmutableArray();
    }

    /// <exception cref="System.OverflowException" />
    public static ImmutableArray<byte> CheckedDotNetStringToByteString(string? dotNetString)
    {
        static void Updater(Span<byte> bytes, string dotNetString0)
        {
            for (int i = 0; i < dotNetString0.Length; i++)
            {
                bytes[i] = ByteCharTheory.CheckedDotNetCharToByteChar(dotNetString0[i]);
            }
        }

        if (dotNetString == null) { return Empty; }
        return UnsafeCreateFixedLength(dotNetString.Length, dotNetString, &Updater);
    }

    public static string ByteStringToDotNetString(ImmutableArray<byte> byteString)
    {
        var sb = StringBuilderPoolTheroy.Shared.Get();
        foreach (byte byteChar in byteString)
        {
            sb.Append(ByteCharTheory.ByteCharToDotNetChar(byteChar));
        }
        string result = sb.ToString();
        StringBuilderPoolTheroy.Shared.Return(sb);
        return result;
    }

    public static ImmutableArray<byte> FromByteSpan(params ReadOnlySpan<byte> bytes) => bytes.ToImmutableArray();

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ImmutableArray<byte> Concat(ImmutableArray<byte> left, ImmutableArray<byte> right)
    {
        GuardLengthIsInValidRange(left.Length + right.Length);
        return left.AddRange(right);
    }
     
}

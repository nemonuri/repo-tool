using Nemonuri.FixedSizes;
using Sls = Nemonuri.ByteChars.Internal.StackLimitSizePremise;

namespace Nemonuri.ByteChars;

public static unsafe partial class MutableByteStringTheory
{
    private static void UnsafeUpdateBuilderWithAux<TAux>
    (
        ArrayBuilder<byte> builder,
        TAux aux,
        delegate*<Span<byte>, TAux, void> updater
    )
    {
        updater(builder.AsSpan(), aux);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GuardLengthIsInValidRange(int length)
    {
        Guard.IsInRange(length, 0, ByteStringConstants.MaxLength);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    private static ArraySegment<byte> UnsafeCreateFixedLength<TAux>(int length, TAux aux, delegate*<Span<byte>, TAux, void> updater)
    {
        GuardLengthIsInValidRange(length);
        if (length <= Sls.GetFixedSize())
        {
            Span<byte> bytes = stackalloc byte[length];
            updater(bytes, aux);
            return new(bytes.ToArray());
        }
        else
        {
            var builder = new ArrayBuilder<byte>(length);
            UnsafeUpdateBuilderWithAux(builder, aux, updater);
            return builder.DrainToArraySemgent();
        }
    }

    private static void UnsafeUpdateBuilderWithAuxReadOnlySpan<TAux>
    (
        ArrayBuilder<byte> builder,
        ReadOnlySpan<TAux> auxSpan,
        delegate*<Span<byte>, ReadOnlySpan<TAux>, void> updater
    )
    {
        updater(builder.AsSpan(), auxSpan);
    }

    /// <exception cref="System.ArgumentOutOfRangeException" />
    private static ArraySegment<byte> UnsafeCreateFixedLengthByReadOnlySpan<TAux>
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
            return new(bytes.ToArray());
        }
        else
        {
            var builder = new ArrayBuilder<byte>(length);
            UnsafeUpdateBuilderWithAuxReadOnlySpan(builder, auxSpan, updater);
            return builder.DrainToArraySemgent();
        }
    }

    public static ArraySegment<byte> Empty => [];

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ArraySegment<byte> FromDotNetCharSpan(params ReadOnlySpan<char> dotNetChars)
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
    public static  ArraySegment<byte> CheckedFromDotNetCharSpan(params ReadOnlySpan<char> dotNetChars)
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
    public static ArraySegment<byte> FromDotNetString(string? dotNetString) => FromDotNetCharSpan(dotNetString ?? ReadOnlySpan<char>.Empty);

    /// <exception cref="System.ArgumentOutOfRangeException" />
    public static ArraySegment<byte> CheckedFromDotNetString(string? dotNetString) => CheckedFromDotNetCharSpan(dotNetString ?? ReadOnlySpan<char>.Empty);

    public static ArraySegment<byte> FromDotNetStringWithEncoding(string? dotNetString, Encoding? encoding = null)
    {
        if (dotNetString == null) { return Empty; }
        Encoding ensuredEncoding = encoding ?? Encoding.Default;
        return new(ensuredEncoding.GetBytes(dotNetString));
    }

    public static ArraySegment<byte> FromDotNetStringWithUtf8Encoding(string? dotNetString) => FromDotNetStringWithEncoding(dotNetString, Encoding.UTF8);
}


using Nemonuri.ByteChars.Internal;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;
using Nemonuri.ByteChars.Extensions;

namespace Nemonuri.ByteChars;

public class ByteStringBuilderPool
{
    public static ByteStringBuilderPool Shared {get;} = new(ImmutableByteArrayBuilderPoolTheory.Shared);

    private readonly ObjectPool<ImmutableArray<byte>.Builder> _innerPool;

    internal ByteStringBuilderPool(ObjectPool<ImmutableArray<byte>.Builder> innerPool)
    {
        Debug.Assert(innerPool is not null);
        _innerPool = innerPool;
    }

    public ByteStringBuilderPool() : this(ImmutableByteArrayBuilderPoolTheory.Create())
    {}

    public RentedBuilder Rent() => new(this);

    public struct RentedBuilder : IDisposable
    {
        private readonly ByteStringBuilderPool _parent;
        private ImmutableArray<byte>.Builder? _innerBuilder;

        internal RentedBuilder(ByteStringBuilderPool parent)
        {
            Debug.Assert(parent is not null);
            _parent = parent;
            _innerBuilder = _parent._innerPool.Get();
        }

        public readonly ImmutableArray<byte>.Builder? InnerBuilder 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _innerBuilder; 
        }

        [MemberNotNullWhen(false, nameof(InnerBuilder))]
        public readonly bool Disposed => _innerBuilder == null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MemberNotNull(nameof(InnerBuilder))]
        public readonly void ThrowIfDisposed()
        {
            Guard.IsFalse(Disposed);
        }

        public readonly ref byte GetPinnableReference()
        {
            ThrowIfDisposed();
            return ref InnerBuilder.GetPinnableReference();
        }

        public readonly RentedBuilder AddByteSpan(ReadOnlySpan<byte> byteSpan)
        {
            ThrowIfDisposed();
            if (byteSpan.Length > 0)
            {
                InnerBuilder.AddRange(byteSpan);
            }
            return this;
        }

        public readonly ImmutableArray<byte> DrainToByteString()
        {
            ThrowIfDisposed();
            return InnerBuilder.DrainToImmutable();
        }

        public void Dispose()
        {
            if (Disposed) { return; }

            var builder = Interlocked.Exchange(ref _innerBuilder, null);
            if (builder == null) { return; }

            _parent._innerPool.Return(builder);
        }
    }
}

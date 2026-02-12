using Microsoft.Extensions.ObjectPool;

namespace Nemonuri.ByteChars.Internal;

internal static class StringBuilderPoolTheory
{
    public static ObjectPool<StringBuilder> Shared {get;} = ObjectPool.Create(new StringBuilderPooledObjectPolicy());
}

internal static class ImmutableByteArrayBuilderPoolTheory
{
    public static ObjectPool<ImmutableArray<byte>.Builder> Shared {get;} = new DefaultObjectPool<ImmutableArray<byte>.Builder>(new ImmutableByteArrayBuilderPoolPolicy());

    internal class ImmutableByteArrayBuilderPoolPolicy() : IPooledObjectPolicy<ImmutableArray<byte>.Builder>
    {
/**
- Reference: https://github.com/dotnet/aspnetcore/blob/v10.0.3/src/ObjectPool/src/StringBuilderPooledObjectPolicy.cs
*/
        private const int InitialCapacity = 100;

        private const int MaximumRetainedCapacity = 4 * 1024;

        public ImmutableArray<byte>.Builder Create()
        {
            return ImmutableArray.CreateBuilder<byte>(InitialCapacity);
        }

        public bool Return(ImmutableArray<byte>.Builder obj)
        {
            if (obj is not { Capacity: <= MaximumRetainedCapacity })
            {
                // Too big. Discard this one.
                return false;
            }

            obj.Clear();
            return true;
        }
    }
}
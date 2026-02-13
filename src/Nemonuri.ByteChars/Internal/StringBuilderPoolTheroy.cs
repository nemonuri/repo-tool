using Microsoft.Extensions.ObjectPool;

namespace Nemonuri.ByteChars.Internal;

internal static class StringBuilderPoolTheory
{
    public static ObjectPool<StringBuilder> Shared {get;} = ObjectPool.Create(new StringBuilderPooledObjectPolicy());
}

namespace Nemonuri.ByteChars.Diagnostics;

public static class Guard
{
    internal static bool CheckSliceArgumentsAreInValidRangeCore(int sourceLength, int offset, int sliceLength, bool throwIfInvalid)
    {
        // Referece: https://github.com/dotnet/maintenance-packages/blob/6b84308c9ad012f53240d72c1d716d7e42546483/src/System.Memory/src/System/Span.Portable.cs#L382C13-L382C87
        if ((uint)offset > (uint)sourceLength || (uint)sliceLength > (uint)(sourceLength - offset))
        {
            return throwIfInvalid ?
                throw new ArgumentOutOfRangeException(string.Format("{0} or {0} + {1} is less than zero or greater than {2}.", nameof(offset), nameof(sliceLength), nameof(sourceLength))) : false;
        }
        return true;
    }

    public static bool CheckSliceArgumentsAreInValidRange(int sourceLength, int offset, int sliceLength) => CheckSliceArgumentsAreInValidRangeCore(sourceLength, offset, sliceLength, throwIfInvalid: false);

    public static void GuardSliceArgumentsAreInValidRange(int sourceLength, int offset, int sliceLength) => CheckSliceArgumentsAreInValidRangeCore(sourceLength, offset, sliceLength, throwIfInvalid: true);
}
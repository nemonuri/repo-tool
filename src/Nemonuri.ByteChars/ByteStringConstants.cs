namespace Nemonuri.ByteChars;

public static class ByteStringConstants
{
    internal const int ByteCharStackLimitSize = 512; // .NET 은 Vector512 를 지원한다.

/**
https://learn.microsoft.com/en-us/dotnet/api/system.array?view=netstandard-2.0

The array size is limited to a total of 4 billion elements, and to a maximum index of 0X7FEFFFFF in any given dimension 
(__0X7FFFFFC7 for byte arrays__ and arrays of single-byte structures).
*/
    public const int MaxLength = 0x7FFFFFC7;
}
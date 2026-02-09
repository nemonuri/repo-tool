namespace Nemonuri.NetStandards;

public static class MathTheory
{
    /// <summary>
    /// <see cref="System.Math.DivRem(int, int)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.math.divrem?view=net-10.0#system-math-divrem(system-int32-system-int32)">
    /// (doc)
    /// </a>
    /// </summary>
    public static (int Quotient, int Remainder) DivRem(int left, int right)
    {
        int q = System.Math.DivRem(left, right, out int r);
        return (q,r);
    }

    /// <summary>
    /// <see cref="System.Math.DivRem(long, long)" />
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.math.divrem?view=net-10.0#system-math-divrem(system-int64-system-int64)">
    /// (doc)
    /// </a>
    /// </summary>
    public static (long Quotient, long Remainder) DivRem(long left, long right)
    {
        long q = System.Math.DivRem(left, right, out long r);
        return (q,r);
    }
}
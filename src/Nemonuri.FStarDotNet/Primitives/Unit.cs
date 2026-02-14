namespace Nemonuri.FStarDotNet.Primitives;

public class Unit : IEquatable<Unit>
{
    public static Unit Singleton {get;} = new();

    private Unit() {}

    public bool Equals(Unit other) => ReferenceEquals(this, other);

    public override bool Equals(object obj) => obj is Unit v && Equals(v);

    public override int GetHashCode() => 0;
}

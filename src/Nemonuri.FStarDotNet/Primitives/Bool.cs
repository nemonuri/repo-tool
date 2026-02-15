namespace Nemonuri.FStarDotNet.Primitives;

public abstract class Bool : IEquatable<Bool>, ILogical
{
    public sealed class True : Bool
    {
        public static True Singleton {get;} = new();

        private True() {}
    }

    public sealed class False : Bool
    {
        public static False Singleton {get;} = new();

        private False() {}
    }

    public bool Equals(Bool other) => ReferenceEquals(this, other);
}

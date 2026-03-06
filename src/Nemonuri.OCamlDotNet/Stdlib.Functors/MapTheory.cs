using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace Nemonuri.OCamlDotNet.Primitives.Functors;

public interface IMap<TKey>
{
    public readonly struct t<a>(FSharpMap<TKey,a> value) : IComparable, IEquatable<t<a>>
    {
        public FSharpMap<TKey,a> Value {get;} = value;

        public int CompareTo(object? obj)
        {
            return ((IComparable)this.Value).CompareTo(obj is t<a> other ? other.Value : obj);
        }

        public bool Equals(t<a> other)
        {
            return this.Value.Equals(other.Value);
        }

        public override bool Equals(object? obj) => obj is t<a> other ? this.Equals(other) : false;

        public override int GetHashCode() => Value.GetHashCode();
    }
}

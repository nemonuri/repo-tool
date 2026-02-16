using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.FStarDotNet.Primitives;

public interface ILogical
{}

public interface IDependentTypePremise<T>
{
    bool TryGetType(T arg, [NotNullWhen(true)] out Type? type);
}

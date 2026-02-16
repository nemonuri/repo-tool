namespace Nemonuri.FStarDotNet.Primitives.TypeConstraints;

[AttributeUsage(
    AttributeTargets.GenericParameter,
    AllowMultiple = false,
    Inherited = true
)]
public sealed class AnyAttribute : Attribute
{
    public Type[]? Constraints {get;}

    public AnyAttribute(params Type[]? constraints)
    {
        Constraints = constraints;
    }
}

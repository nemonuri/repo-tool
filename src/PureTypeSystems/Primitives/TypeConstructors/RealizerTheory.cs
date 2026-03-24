using System.Reflection;

namespace Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

public static class RealizerTheory
{
    private const DynamicallyAccessedMemberTypes RealizerAccess = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicMethods;

    public static string FallbackRealizerName => "Create";

    private static bool TryGetStaticCreate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>([NotNullWhen(true)] out MethodInfo? methodInfo)
    {
        if 
        (
            typeof(T).GetMethod(FallbackRealizerName) is { IsStatic: true } mi &&
            mi.ReturnType == typeof(T)
        )
        {
            methodInfo = mi; 
            return true;
        }
        else
        {
            methodInfo = default;
            return false;            
        }
    }

    private static bool HasParameterlessConstructor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>()
    {
        if (typeof(T).IsValueType)
        {
            return true;
        }
        else if (typeof(T).GetConstructor(Type.EmptyTypes) is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static T Realize<[DynamicallyAccessedMembers(RealizerAccess)] T>()
    {
        if (HasParameterlessConstructor<T>()) 
        { 
            return Activator.CreateInstance<T>(); 
        }
        else if (TryGetStaticCreate<T>(out var mi))
        {
            return (T)mi.Invoke(null, null)!;
        }
        else
        {
            throw new MissingMethodException("Cannot create an instance of an abstract class, or the type that is specified for T does not have a parameterless constructor.");
        }
    }

    public static bool HasRealizer<[DynamicallyAccessedMembers(RealizerAccess)] T>()
    {
        if (HasParameterlessConstructor<T>())
        {
            return true;
        }
        else if (TryGetStaticCreate<T>(out _))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool TryRealize<T>([NotNullWhen(true)] out T? realized)
    {
        try
        {
            realized = Realize<T>()!;
            return true;
        }
        catch (MissingMethodException)
        {
            realized = default;
            return false;
        }
    }
}

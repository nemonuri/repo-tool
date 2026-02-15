using System.Globalization;
using System.Reflection;

namespace Nemonuri.FStarDotNet.Primitives;

// type prop = a: Type0{a `subtype_of` unit}
public class Unit : System.Type, IEquatable<Unit>, ILogical
{
    public static Unit Singleton {get;} = new();

    private Unit() {}

    public bool Equals(Unit other) => ReferenceEquals(this, other);

    private TypeInfo _thisTypeInfo = typeof(Unit).GetTypeInfo();

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetConstructors(bindingAttr);
    }

    public override Type GetElementType()
    {
        return _thisTypeInfo.GetElementType();
    }

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetEvent(name, bindingAttr);
    }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetEvents(bindingAttr);
    }

    public override FieldInfo GetField(string name, BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetField(name, bindingAttr);
    }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetFields(bindingAttr);
    }

    public override Type GetInterface(string name, bool ignoreCase)
    {
        return _thisTypeInfo.GetInterface(name, ignoreCase);
    }

    public override Type[] GetInterfaces()
    {
        return _thisTypeInfo.GetInterfaces();
    }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetMembers(bindingAttr);
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetMethods(bindingAttr);
    }

    public override Type GetNestedType(string name, BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetNestedType(name, bindingAttr);
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetNestedTypes(bindingAttr);
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
        return _thisTypeInfo.GetProperties(bindingAttr);
    }

    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
    {
        return _thisTypeInfo.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
    }

    public override Assembly Assembly => _thisTypeInfo.Assembly;

    public override string AssemblyQualifiedName => _thisTypeInfo.AssemblyQualifiedName;

    public override Type BaseType => _thisTypeInfo.BaseType;

    public override string FullName => _thisTypeInfo.FullName;

    public override Guid GUID => _thisTypeInfo.GUID;

    public override Module Module => _thisTypeInfo.Module;

    public override string Namespace => _thisTypeInfo.Namespace;

    public override Type UnderlyingSystemType => _thisTypeInfo.UnderlyingSystemType;

    public override object[] GetCustomAttributes(bool inherit)
    {
        return _thisTypeInfo.GetCustomAttributes(inherit);
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        return _thisTypeInfo.GetCustomAttributes(attributeType, inherit);
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        return _thisTypeInfo.IsDefined(attributeType, inherit);
    }

    public override string Name => _thisTypeInfo.Name;

    protected override TypeAttributes GetAttributeFlagsImpl() => _thisTypeInfo.Attributes;

    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) =>
        _thisTypeInfo.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);

    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) =>
        _thisTypeInfo.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);

    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) =>
        _thisTypeInfo.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

    protected override bool HasElementTypeImpl() => _thisTypeInfo.HasElementType;

    protected override bool IsArrayImpl() => _thisTypeInfo.IsArray;

    protected override bool IsByRefImpl() => _thisTypeInfo.IsByRef;

    protected override bool IsCOMObjectImpl() => _thisTypeInfo.IsCOMObject;

    protected override bool IsPointerImpl() => _thisTypeInfo.IsPointer;

    protected override bool IsPrimitiveImpl() => _thisTypeInfo.IsPrimitive;
}

using System.Runtime.InteropServices;

namespace Nemonuri.FStarDotNet.Primitives.Experimental;

[StructLayout(LayoutKind.Sequential)]
public readonly struct FStarFunctionTable
{
    public readonly nint Value;

    public FStarFunctionTable(nint value)
    {
        Value = value;
    }
}

public interface IFStarFunctionTable : IFStarInstance<FStarFunctionTable>
{
}

public interface IFStarDelayedInstance : ISolvedFStarType
{
    void GetValue(out object? delayed);
}

public interface IFStarDelayedInstance<T> : IFStarDelayedInstance
{
    void GetValue(out T? delayed);
}

public readonly struct FStarDelayedInstance<T> : IFStarDelayedInstance<T>
{
    private readonly T? _value;

    public FStarDelayedInstance(T? value)
    {
        _value = value;
    }

    public void GetValue(out T? delayed)
    {
        throw new NotImplementedException();
    }

    public void GetValue(out object? delayed)
    {
        throw new NotImplementedException();
    }

    public Type GetSolvedDotNetType()
    {
        throw new NotImplementedException();
    }

    public ITypeList? GetDotNetTypes()
    {
        throw new NotImplementedException();
    }
}

public readonly struct AAA<T>
{
    public static implicit operator int(AAA<T> sdfsdf) => default;
}


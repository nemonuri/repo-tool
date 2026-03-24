using Hth = Nemonuri.PureTypeSystems.Primitives.HandleTheory;
using System.Runtime.CompilerServices;
using Nemonuri.PureTypeSystems.Primitives.TypeConstructors;

namespace Nemonuri.PureTypeSystems.Primitives;

public readonly record struct TestResult
{
    /** ￢E ∨ ￢￢E */

    public bool IsCounterExample {get;}
    public bool IsExample => !IsCounterExample;

    internal TestResult(bool isCounterExample)
    {
        IsCounterExample = isCounterExample;
    }   

    public static TestResult CounterExample => new(true);

    public static TestResult Example => new(false);
}

public interface ITesterPremise
{
    TestResult Test<T>(in T subject);
}

public interface ITesterPremise<T> : ITesterPremise, IArrowPremise<T, TestResult>
{
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct TesterHandle<T> : IHandle, IEquatable<TesterHandle<T>>
{
    private readonly ArrowHandle<T, TestResult> _arrowHandle;

    internal TesterHandle(ArrowHandle<T, TestResult> arrowHandle)
    {
        _arrowHandle = arrowHandle;
    }

    public nint ToIntPtr() => _arrowHandle.ToIntPtr();

    public ArrowHandle<T, TestResult> ArrowHandle => _arrowHandle;

    public bool IsFailure => ArrowHandle.IsFailure;

    public TestResult TestForce(in T subject) => ArrowHandle.Apply(in subject);

    public bool Equals(TesterHandle<T> other) => Hth.Equals(this, other);

    public override bool Equals(object? obj) => obj is TesterHandle<T> o && Equals(o);

    public override int GetHashCode() => Hth.GetHashCode(this);
}


public static class TesterTheory
{
    public static TesterHandle<T> GetFailureTestHandle<T>() => default;

    public static bool TryFreeTest<T1, T2, TTester>(in TTester boundTester, in T2 expr, [NotNullWhen(true)] out TestResult? testResult)
        where TTester : ITesterPremise<T1>
    {
        if (typeof(T1) != typeof(T2)) { testResult = null; return false; }
        testResult = boundTester.Apply(in Unsafe.As<T2,T1>(ref Unsafe.AsRef(in expr)));
        return true;
    }

    extension<TTester>(TTester) where TTester : ITesterPremise
    {
        public unsafe static TesterHandle<T> ToHandle<T>()
        {
            static TestResult Impl(in T item) => RealizerTheory.Realize<TTester>().Test(in item);

            ArrowHandle<T, TestResult> arrowHandle = new(&Impl);
            return new(arrowHandle);
        }
    }

    public static bool TryIntroduceTester<T>(IIntroducer<TestResult> introducer, out TesterHandle<T> testerHandle)
    {
        var handle = introducer.Introduce<T>(default);
        if (handle.IsFailure) 
        { 
            testerHandle = default; 
            return false; 
        }
        else
        {
            testerHandle = new(handle);
            return true;
        }
    }
        
}
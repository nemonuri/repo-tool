using System.Runtime.InteropServices;

namespace Nemonuri.PureTypeSystems.Primitives;

public interface IConstant<T>
{
    T Value {get;}
}

public interface IRefinerPremise<TPre, TPost>
{
    Judgement Judge(in TPre? pre, out TPost? post);
}

public interface IRefinerPremise<T>
{
    Judgement Judge(in T? pre, out T? post);
}


[StructLayout(LayoutKind.Sequential)]
public readonly record struct Judgement(bool Determinate, bool Truthy)
{
    public static Judgement Unknown => new(false,false);

    public static Judgement Thunk => new(false,true);

    public static Judgement False => new(true,false);

    public static Judgement True => new(true,true);
}

public readonly unsafe struct RefinerHandle<TPre, TPost>(delegate*<in TPre?, out TPost?, Judgement> fp)
{
    private readonly delegate*<in TPre?, out TPost?, Judgement> _fp = fp;

    public bool HasValue => _fp != null;

    public Judgement Judge(in TPre? pre, out TPost? post) => _fp(in pre, out post);
}


[StructLayout(LayoutKind.Sequential)]
public readonly struct Refined<T, TRefiner>(T value) where TRefiner : IRefinerPremise<T>
{
    private readonly T _value = value;

    public T Value => _value;
}

public readonly struct Bracket<T> {}

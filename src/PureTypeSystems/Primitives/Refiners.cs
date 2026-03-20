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
    Judgement Judge(in T? pre);
}




public readonly unsafe struct RefinerHandle<TPre, TPost>(delegate*<in TPre?, out TPost?, Judgement> fp)
{
    private readonly delegate*<in TPre?, out TPost?, Judgement> _fp = fp;

    public bool HasValue => _fp != null;

    public Judgement Judge(in TPre? pre, out TPost? post) => _fp(in pre, out post);
}

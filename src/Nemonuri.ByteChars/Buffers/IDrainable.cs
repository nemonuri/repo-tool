namespace Nemonuri.Buffers;

public interface IFlushable
{
    void Flush();
}

public interface IDrainable<T>
{
    T? Drain();
}
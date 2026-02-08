namespace Nemonuri.OCamlDotNet;

public interface IFixedSizePremise<TPremise>
    where TPremise : unmanaged, IFixedSizePremise<TPremise>
{
    int FixedSize {get;}
}


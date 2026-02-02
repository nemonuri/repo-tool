module Nemonuri.FStarDotNet.Z

//--- Reference: https://ocaml.org/p/zarith/1.10/doc/zarith/Z/index.html ---

/// Implmentaion of [System.Numerics.BigInteger.Explicit\(BigInteger to IntPtr\)](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.op_explicit?view=net-10.0#system-numerics-biginteger-op-explicit(system-numerics-biginteger)-system-intptr)
/// - [Reference](https://github.com/dotnet/runtime/blob/12d635004345678d5245e77dd9e4e9daeb6a543f/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L2040)
let to_nint (i: bigint) : nativeint = 
    if System.Environment.Is64BitProcess then
        let r : int64 = bigint.op_Explicit i
        Operators.nativeint r
    else
        let r : int32 = bigint.op_Explicit i
        Operators.nativeint r

let to_int (i: bigint) : FSharp.Core.int = bigint.op_Explicit i

//---|
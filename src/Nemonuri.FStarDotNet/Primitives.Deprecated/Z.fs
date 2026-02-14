// Reference: https://ocaml.org/p/zarith/1.10/doc/zarith/Z/index.html

module Nemonuri.OCamlDotNet.zarith.Z

/// Type of integers of arbitrary length.
type t = bigint

/// Raised by conversion functions when the value cannot be represented in the destination type.
exception Overflow = System.OverflowException

let to_int (i: bigint) : FSharp.Core.int = bigint.op_Explicit i

let zero = t.Zero

let one = t.One

let minus_one = t.MinusOne

let of_int (i: FSharp.Core.int) : t = t i

let of_string (s: string) : t = t.Parse s

let to_string (i: t) : string = i.ToString()

let of_nativeint (i: nativeint) : t = t (Operators.int64 i)

/// Implmentaion of [System.Numerics.BigInteger.Explicit\(BigInteger to IntPtr\)](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.op_explicit?view=net-10.0#system-numerics-biginteger-op-explicit(system-numerics-biginteger)-system-intptr)
/// - [Reference](https://github.com/dotnet/runtime/blob/12d635004345678d5245e77dd9e4e9daeb6a543f/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L2040)
let to_nativeint (i: t) : nativeint = 
    if System.Environment.Is64BitProcess then
        let r : int64 = t.op_Explicit i
        Operators.nativeint r
    else
        let r : int32 = t.op_Explicit i
        Operators.nativeint r

(**
### .NET DivRem operation

- Copy from [mathnet-numerics/docs/Euclid.md](https://github.com/mathnet/mathnet-numerics/blob/v5.0.0/docs/Euclid.md)

#### Remainder

The **remainder** is the amount left over after performing the division of a dividend
by a divisor, $\frac{dividend}{divisor}$, which do not divide evenly, that is,
where the result of the division cannot be expressed as an integer. It is thus natural
that the **remainder has the sign of the dividend**.

In C# and F#, the remainder is available as `%` operator, in VB as `Mod`.
Alternatively you can use the Reminder function:

    [lang=csharp]
    Euclid.Remainder( 5,  3); // =  2, such that 5 = 1*3 + 2
    Euclid.Remainder(-5,  3); // = -2, such that -5 = -1*3 - 2
    Euclid.Remainder( 5, -3); // =  2, such that 5 = -1*-3 + 2
    Euclid.Remainder(-5, -3); // = -2, such that -5 = 1*-3 - 2
*)

(**
### ZArith ediv_rem

- Copy From [zarith doc](https://ocaml.org/p/zarith/1.10/doc/zarith/Z/index.html#val-ediv_rem)

```ocaml
val ediv_rem : t -> t -> t * t
```

Euclidean division and remainder. 
**ediv_rem a b** returns a pair **(q, r)** 
such that **a = b * q + r** and **0 <= r < |b|**. 
Raises **Division_by_zero** if **b = 0**.
*)

(**
### FSharpPlus

- [FSharpPlus.Math.Generic.divRemE](https://github.com/fsprojects/FSharpPlus/blob/v1.9.1/src/FSharpPlus/Math/Generic.fs)
*)

let ediv_rem (n: t) (d: t) : t * t =
    if n < 0I then
        let q, r = t.DivRem(n, d)
        assert (r <= 0I)
        if r = 0I then
            q, r
        elif d > 0I then
            q-1I, r+d
        else
            q+1I, r-d
    else 
        t.DivRem(n, d)

//---|
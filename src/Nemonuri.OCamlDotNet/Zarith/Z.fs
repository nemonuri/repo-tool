/// - Reference: https://ocaml.org/p/zarith/1.10/doc/zarith/Z/index.html
module Nemonuri.OCamlDotNet.Zarith.Z

open System.Numerics
open Nemonuri.OCamlDotNet
open type Nemonuri.ByteChars.Numerics.BigIntegerTheory

/// Type of integers of arbitrary length.
type t = BigInteger

/// Raised by conversion functions when the value cannot be represented in the destination type.
exception Overflow = System.OverflowException


/// The number 0.
let zero: t = t.Zero

/// The number 1.
let one: t = t.One

/// The number -1.
let minus_one: t = t.MinusOne

/// Converts from a base integer.
let of_int (n: int) = t n

/// Converts from a 32-bit integer.
let of_int32 (n: int32) = t n

/// Converts from a 64-bit integer.
let of_int64 (n: int32) = t n

/// Converts from a native integer.
let of_nativeint (i: nativeint) : t = t (Operators.int64 i)

(**
- https://learn.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.op_explicit?view=netstandard-2.0#system-numerics-biginteger-op-explicit(system-double)-system-numerics-biginteger
*)
/// Converts from a floating-point value. The value is truncated (rounded towards zero). 
/// Raises Overflow on infinity and NaN arguments.
let of_float (n: float) = t n 

(**
- 현재, '10진법 문자열'에 대해서만 구현
*)
/// Converts a string to an integer. 
/// An optional - prefix indicates a negative number, while a + prefix is ignored. 
/// An optional prefix 0x, 0o, or 0b (following the optional - or + prefix) indicates that the number is, represented, in hexadecimal, octal, or binary, respectively. 
/// Otherwise, base 10 is assumed. (Unlike C, a lone 0 prefix does not denote octal.) 
/// Raises an Invalid_argument exception if the string is not a syntactically correct representation of an integer.
let of_string (s: string) : t = 
    let success, n = TryParseAsciiByteSpanToBigInteger(s.AsSpan())
    if not success then Stdlib.invalid_arg "invalid argument" else
    n

/// Gives a human-readable, decimal string representation of the argument.
let to_string (n: t) : string = FormatBigIntegerToAsciiDecimalByteString n

//// <category name="Basic arithmetic operations">

/// Returns its argument plus one.
let succ (n: t) : t = BigInteger.op_Increment(n)

/// Returns its argument minus one.
let pred (n: t) : t = BigInteger.op_Decrement(n)

/// Absolute value.
let abs (n: t) : t = BigInteger.Abs(n)

/// Unary negation.
let neg (n: t) : t = BigInteger.op_UnaryNegation(n)

/// Addition.
let add (left: t) (right: t) : t = BigInteger.Add(left, right)

/// Subtraction.
let sub (left: t) (right: t) : t = BigInteger.Subtract(left, right)

/// Multiplication.
let mul (left: t) (right: t) : t = BigInteger.Multiply(left, right)

/// Integer division. The result is truncated towards zero and obeys the rule of signs. Raises Division_by_zero if the divisor (second argument) is 0.
let div (left: t) (right: t) : t = BigInteger.Divide(left, right)

/// Integer remainder. Can raise a Division_by_zero. 
/// The result of rem a b has the sign of a, and its absolute value is strictly smaller than the absolute value of b. 
/// The result satisfies the equality a = b * div a b + rem a b.
let rem (left: t) (right: t) : t = BigInteger.Remainder(left, right)

/// Computes both the integer quotient and the remainder. 
/// div_rem a b is equal to (div a b, rem a b). Raises Division_by_zero if b = 0.
let div_rem (left: t) (right: t) : t * t = BigInteger.DivRem(left, right)

//// </category">
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



//// <category name="Conversions">

/// Converts to a base integer. May raise Overflow.
let to_int (n: t) : int = BigInteger.op_Explicit n


/// Gives a human-readable, decimal string representation of the argument.
let to_string (n: t) : string = FormatBigIntegerToAsciiDecimalByteString n



//// </category> 


//// <category name="Ordering">

/// Comparison. compare x y returns 0 if x equals y, -1 if x is smaller than y, and 1 if x is greater than y.
///
/// Note that Pervasive.compare can be used to compare reliably two integers only on OCaml 3.12.1 and later versions.
let inline compare (left: t) (right: t) = Microsoft.FSharp.Core.LanguagePrimitives.GenericComparison left right

/// Equality test.
let equal (left: t) (right: t) = BigInteger.op_Equality(left, right)

/// Less than or equal.
let leq (left: t) (right: t) = BigInteger.op_LessThanOrEqual(left, right)

/// Greater than or equal.
let geq (left: t) (right: t) = BigInteger.op_GreaterThanOrEqual(left, right)

/// Less than (and not equal).
let lt (left: t) (right: t) = BigInteger.op_LessThan(left, right)

/// Greater than (and not equal).
let gt (left: t) (right: t) = BigInteger.op_GreaterThan(left, right)

/// Returns -1, 0, or 1 when the argument is respectively negative, null, or positive.
let sign (n: t) = n.Sign

/// Returns the minimum of its arguments.
let min (left: t) (right: t) : t = BigInteger.Min(left, right)

/// Returns the maximum of its arguments.
let max (left: t) (right: t) : t = BigInteger.Max(left, right)

let is_even (n: t) : bool = n.IsEven

let is_odd (n: t) : bool = is_even n |> not

let hash (n: t) : int = n.GetHashCode()

//// </category>



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

let private is_zero n = equal n zero

let private is_positive n = gt n zero

/// Integer division with rounding towards +oo (ceiling). Can raise a Division_by_zero.
let cdiv (left: t) (right: t) : t =
    let q, r = div_rem left right
    if is_zero r then q else
    if is_positive left = is_positive right then succ q else
    q

/// Integer division with rounding towards -oo (floor). Can raise a Division_by_zero.
let fdiv (left: t) (right: t) : t =
    let q, r = div_rem left right
    if is_zero r then q else
    if is_positive left <> is_positive right then pred q else
    q

/// Euclidean division and remainder. ediv_rem a b returns a pair (q, r) such that a = b * q + r and 0 <= r < |b|. Raises Division_by_zero if b = 0.
let ediv_rem (left: t) (right: t) : t * t =
    let q, r = div_rem left right
    if is_zero r then q, r else
    if is_positive left then q, r else
    if is_positive right then
        pred q, r + right
    else
        succ q, r - right

/// Euclidean division. ediv a b is equal to fst (ediv_rem a b). The result satisfies 0 <= a - b * ediv a b < |b|. Raises Division_by_zero if b = 0.
let ediv (left: t) (right: t) : t = ediv_rem left right |> Microsoft.FSharp.Core.Operators.fst

/// Euclidean remainder. erem a b is equal to snd (ediv_rem a b). The result satisfies 0 <= erem a b < |b| and a = b * ediv a b + erem a b. Raises Division_by_zero if b = 0.
let erem (left: t) (right: t) : t = ediv_rem left right |> Microsoft.FSharp.Core.Operators.snd

//// </category>

//// <category name="Bit-level operations">

/// Bitwise logical and.
let logand (left: t) (right: t) : t = left &&& right

/// Bitwise logical or.
let logor (left: t) (right: t) : t = left ||| right

/// Bitwise logical exclusive or.
let logxor (left: t) (right: t) : t = left ^^^ right

/// Bitwise logical negation. The identity lognot a=-a-1 always hold.
let lognot (n: t) : t = BigInteger.op_OnesComplement n

/// Shifts to the left. Equivalent to a multiplication by a power of 2. The second argument must be nonnegative.
let shift_left (left: t) (right: int) : t = left <<< right

/// Shifts to the right. This is an arithmetic shift, equivalent to a division by a power of 2 with rounding towards -oo. 
/// The second argument must be nonnegative.
let shift_right (left: t) (right: int) : t = left >>> right

//// </category>
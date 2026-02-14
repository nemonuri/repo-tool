// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/fsharp/base/FStar_UInt8.fs

module Nemonuri.FStarDotNet.FStar.UInt8
open Nemonuri.FStarDotNet

// TODO: Would it make sense to use .net byte here?
type uint8 = FSharp.Core.byte
type byte = uint8
type t = uint8
type t' = t

let n = Prims.of_int 8
let v (x:t) : Prims.int = Prims.int x

let zero = 0uy
let one = 1uy
let ones = 255uy
                                              
let add (a:uint8) (b:uint8) : uint8 = a + b
let add_underspec a b = (add a b) &&& 255uy
let add_mod = add_underspec

let sub (a:uint8) (b:uint8) : uint8 = a - b
let sub_underspec a b = (sub a b) &&& 255uy
let sub_mod = sub_underspec

let mul (a:uint8) (b:uint8) : uint8 = a * b
let mul_underspec a b = (mul a b) &&& 255uy
let mul_mod = mul_underspec

let div (a:uint8) (b:uint8) : uint8 = a / b

let rem (a:uint8) (b:uint8) : uint8 = a % b

let logand (a:uint8) (b:uint8) : uint8 = a &&& b
let logxor (a:uint8) (b:uint8) : uint8 = a ^^^ b
let logor  (a:uint8) (b:uint8) : uint8 = a ||| b
let lognot (a:uint8) : uint8 = ~~~a
       
let int_to_uint8 (x:Prims.int) : uint8 = (x % 256I) |> bigint.op_Explicit

let shift_right (a:uint8) (b:System.UInt32) : uint8 = a >>> (int32 b)
let shift_left  (a:uint8) (b:System.UInt32) : uint8 = (a <<< (int32 b)) &&& 255uy

(* Comparison operators *)
let eq (a:uint8) (b:uint8) : bool = a = b
let gt (a:uint8) (b:uint8) : bool = a > b
let gte (a:uint8) (b:uint8) : bool = a >= b
let lt (a:uint8) (b:uint8) : bool = a < b
let lte (a:uint8) (b:uint8) : bool =  a <= b

(* NOT Constant time comparison operators *)
let gte_mask (a:uint8) (b:uint8) : uint8 = if a >= b then 255uy else 0uy
let eq_mask (a:uint8) (b:uint8) : uint8 = if a = b then 255uy else 0uy

let of_string s = System.Byte.Parse s
let to_string (s: uint8) = s.ToString()
// The hex printing for BigInteger in .NET is a bit non-standard as it 
// prints an extra leading '0' for positive numbers
let to_string_hex (s : t) = "0x" + (s.ToString("X").TrimStart([| '0' |]))
let to_string_hex_pad (s : t) = s.ToString("X").TrimStart([| '0' |]).PadLeft(2, '0')
let uint_to_t s = int_to_uint8 s
let to_int (s: byte) = bigint s
let __uint_to_t = uint_to_t
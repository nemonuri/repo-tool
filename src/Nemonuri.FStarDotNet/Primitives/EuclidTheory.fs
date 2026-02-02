namespace Nemonuri.FStarDotNet

open System
open System.Numerics

module EuclidTheory =

    type System.Int32 with
        static member DivRem (dividend: int32, divisor: int32) = Math.DivRem(dividend, divisor)

    type System.Int64 with
        static member DivRem (dividend: int64, divisor: int64) = Math.DivRem(dividend, divisor)
    
    type System.Numerics.BigInteger with
        static member DivRem (dividend: bigint, divisor: bigint) = bigint.DivRem(dividend, divisor)


    let inline divRem'<^T when ^T : (static member DivRem : ^T * ^T -> ^T * ^T)>
        (dividend: ^T) (divisor: ^T) = 
        (^T : (static member DivRem : ^T * ^T -> ^T * ^T) (dividend, divisor))


(*
    static member DivRem (dividend: int32, divisor: int32) = Math.DivRem(dividend, divisor)
    static member DivRem (dividend: int64, divisor: int64) = Math.DivRem(dividend, divisor)
    static member DivRem (dividend: bigint, divisor: bigint) = bigint.DivRem(dividend, divisor)

    static member inline DivRem'<^Theory, ^T when ^Theory : (static member DivRem : ^T * ^T -> ^T * ^T)>
        (dividend: ^T) (divisor: ^T) = 
        (^Theory : (static member DivRem : ^T * ^T -> ^T * ^T) (dividend, divisor))
*)
(*
    static member inline EdivRem<^T 
        when ^T : comparison 
        and ^T : equality
        and ^T : (static member (+) ) > 
        (dividend: ^T) (divisor: ^T) : ^T * ^T
*)

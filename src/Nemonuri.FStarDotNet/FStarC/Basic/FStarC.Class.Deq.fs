#nowarn "1173"
#nowarn "1174"

// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Deq.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Deq.fst

namespace Nemonuri.FStarDotNet.FStarC.Class

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStar.Pervasives
open Nemonuri.FStarDotNet.FStarOperators
open Microsoft.FSharp.Core.Operators.Unchecked

module Deq =

    type deq<'a> =
        interface
            abstract member (=?) : 'a -> 'a -> bool
        end
    
    let (<>?) (_0: #deq<'a>) (x: 'a) (y: 'a) = not (_0.(=?) x y)

    type deq_int =
        struct
            interface deq<Prims.int> with
                member this.(=?) (x: Prims.int) (y: Prims.int): bool = x =. y
        end

    type deq_bool =
        struct
            interface deq<bool> with
                member this.(=?) x y: bool = x =. y
        end

    type deq_unit =
        struct
            interface deq<unit> with
                member this.(=?) x y: bool = x =. y
        end

    type deq_string =
        struct
            interface deq<Prims.string> with
                member this.(=?) x y: bool = x =. y
        end
    
    type deq_option<'a, '_1 when '_1 :> deq<'a> and '_1 : unmanaged> =
        struct
            interface deq<option<'a>> with
                member this.(=?) x y = 
                    match x, y with
                    | None, None -> true
                    | Some x, Some y -> x </defaultof<'_1>.(=?)/> y
                    | _, _ -> false
        end
    
    let rec private eqList (eq : #deq<'a>) (xs : list<'a>) (ys : list<'a>) : bool =
        match xs, ys with
        | [], [] -> true
        | x::xs, y::ys -> x </eq.(=?)/> y && eqList eq xs ys
        | _, _ -> false
    
    type deq_list<'a, 'd when 'd :> deq<'a> and 'd : unmanaged> =
        struct
            interface deq<list<'a>> with
                member this.(=?) x y = eqList defaultof<'d> x y
        end
    
    type deq_either<'a, 'b, 'd1, 'd2 
                        when 'd1 :> deq<'a> and 'd1 : unmanaged
                        and 'd2 :> deq<'b> and 'd2 : unmanaged> =
        struct
            interface deq<either<'a,'b>> with
                member this.(=?) x y = 
                    match x, y with
                    | Inl x, Inl y -> x </defaultof<'d1>.(=?)/> y
                    | Inr x, Inr y -> x </defaultof<'d2>.(=?)/> y
                    | _, _ -> false
        end
namespace Nemonuri.FStarDotNet

open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.Forwarded
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

[<AbstractClass; Sealed>]
type FStarLiteralTheory =

    static member Int(s: int) : Prims.int = (Z.of_int >> Fu.pur) s

    static member Unit : Prims.unit = Fu.pur ()

    //static member String(s: byte array)

module FStarOperators =

    let Unit = FStarLiteralTheory.Unit

    let Int s = FStarLiteralTheory.Int s

    let Nil<'a> : Prims.list<'a> = Prims.Nil<'a>

    let Cons<'a> (hd: 'a) (tl: Prims.list<'a>) : Prims.list<'a> = Prims.Cons hd tl

    let (>::) hd tl = Cons hd tl

    let (|Nil|Cons|) l = Prims.(|Nil|Cons|) l

    let (|Singleton|_|) l = Prims.(|Singleton|_|) l



    let BTrue = Prims.BTrue

    let BFalse = Prims.BFalse

    let (|BTrue|BFalse|) b = Prims.(|BTrue|BFalse|) b

    let (&&.) x y = Prims.op_AmpAmp x y

    let (||.) x y = Prims.op_BarBar x y

    let bnot x = Prims.op_Negation x

    let ( *. ) x y = Prims.op_Multiply x y

    let ( -. ) x y = Prims.op_Subtraction x y

    let ( +. ) x y = Prims.op_Addition x y

    let ( ~-. ) x  = Prims.op_Minus x 

    let ( <=. ) x y = Prims.(<=) x y

    let ( >. ) x y = Prims.(>) x y

    let ( >=. ) x y = Prims.(>=) x y

    let ( <. ) x y = Prims.(<) x y

    let ( =. ) x y = Prims.(=) x y

    let ( <>. ) x y = Prims.op_disEquality x y

    let ( ^. ) s1 s2 = Prims.op_Hat s1 s2

    let ( @. ) x y = FStar_List_Tot_Base.append x y


    
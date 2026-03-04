namespace Nemonuri.FStarDotNet

open Nemonuri.OCamlDotNet.Primitives.Operations
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.FStarDotNet.Forwarded
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

[<AbstractClass; Sealed>]
type FStarOperatorTheory =

    static member Int(s: int) : Prims.int = (Z.of_int >> Fu.pur) s

    static member Unit : Prims.unit = Fu.pur ()

    static member String(s: byte array) : Prims.string = (OCamlByteSpanSources.Unsafe.stringOfArray >> Fu.pur) s

    



module FStarOperators =

    module Tu = Nemonuri.FStarDotNet.Primitives.Operations.Tuples

    



#if false
    let inline ( &. ) l r =
        let inline call (p': ^p) (l': ^l) (r': ^r) = ((^p or ^l) : (static member AddTupleItem: _*_ -> _) l', r')
        call Unchecked.defaultof<TuplePremise> l r
#endif

    
        




    let unitO = FStarOperatorTheory.Unit

    let intO s = FStarOperatorTheory.Int s

    let Nil<'a> : Prims.list<'a> = Prims.Nil<'a>

    let Cons<'a> (hd: 'a) (tl: Prims.list<'a>) : Prims.list<'a> = Prims.Cons hd tl

    let stringO(s: byte array) = FStarOperatorTheory.String s

    let (>::) hd tl = Cons hd tl

    let (|Nil|Cons|) l = Prims.(|Nil|Cons|) l

    let (|Singleton|_|) l = Prims.(|Singleton|_|) l




    let BTrue = Prims.BTrue

    let BFalse = Prims.BFalse

    let (|BTrue|BFalse|) b = Prims.(|BTrue|BFalse|) b

    let (&&.) x y = Prims.op_AmpAmp x y

    let (||.) x y = Prims.op_BarBar x y

    let notO x = Prims.op_Negation x

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


    let None<'a> = FStar_Pervasives_Native.None<'a>

    let Some v = FStar_Pervasives_Native.Some v

    let (|None|Some|) x = FStar_Pervasives_Native.(|None|Some|)



    let inline ( &. ) l r = FStarTupleOperators.( &. ) l r

    let inline ( .&. ) l r = FStarTupleOperators.( .&. ) l r


    let type0O s = Fu.pur s

    let (|Type0|) s = Fu.extract s

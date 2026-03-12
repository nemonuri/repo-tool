namespace Nemonuri.FStarDotNet

open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.OCamlDotNet.Forwarded

module Operations =

    let inline flip f x y = f y x

module TypeShadowing =

    type char = FStar.Char.char

    type int = Prims.int

    type string = Prims.string

    type float = FStar.Float.float

    type double = FStar.Float.double


module Operators =

    module Literals =

        module T = TypeShadowing
        module O = Nemonuri.OCamlDotNet.Primitives.Operators.Literals

        type Premise =
            struct
                static member ( ~% ) (s: Core.int) : T.int = Z.of_int s
                static member ( ~% ) (s: byte array) : T.string = O.Premise.( ~% ) s
                static member ( ~% ) (s: Core.string) : T.string = O.Premise.( ~% ) s
                static member ( ~% ) (s: Core.char) : T.char = System.Text.Rune(s).Value
                static member ( ~% ) (s: byte) : T.char = Core.Operators.int s
            end

        let inline ( ~% ) s =
            let inline call (p: ^p) (s': ^s) = ((^p or ^s) : (static member ( ~% ): ^s -> ^fs ) s')
            call Unchecked.defaultof<Premise> s

        /// Constant pattern matching
        let inline (|C|_|) l r =
            let inline call (l': ^s) (r': ^fs) = Prims.(=) (%l') r' in
            match call l r with
            | true -> ValueSome r
            | false -> ValueNone

    let inline (</) x = (|>) x

    let inline (/>) x = Operations.flip x

    let toInt s : Prims.int = Z.of_int s

    let toString s: Prims.string = OCamlByteSpanSources.Unsafe.stringOfArray s

    let toChar (s: byte) = Core.Operators.int s

    let ( * ) x y = Prims.op_Multiply x y

    let ( - ) x y = Prims.op_Subtraction x y

    let ( + ) x y = Prims.op_Addition x y

    let ( % ) x y = Prims.op_Modulus x y

    let ( / ) x y = Prims.op_Division x y

    let ( ~- ) x  = Prims.op_Minus x 

    let ( <=. ) x y = Prims.(<=) x y

    let ( >. ) x y = Prims.(>) x y

    let ( >=. ) x y = Prims.(>=) x y

    let ( <. ) x y = Prims.(<) x y

    let ( =. ) x y = Prims.(=) x y

    let ( <>. ) x y = Prims.op_disEquality x y

    let ( ^ ) s1 s2 = Prims.op_Hat s1 s2

    let (|ToString|_|) (b: byte array) (s: Prims.string) = toString b </String.equal/> s


#if false
    

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



    let ( @. ) x y = FStar_List_Tot_Base.append x y


    let None<'a> = FStar_Pervasives_Native.None<'a>

    let Some v = FStar_Pervasives_Native.Some v

    let (|None|Some|) x = FStar_Pervasives_Native.(|None|Some|) x



    let inline ( &. ) l r = FStarTupleOperators.( &. ) l r

    let inline ( .&. ) l r = FStarTupleOperators.( .&. ) l r



    let (|Type0|) s = Fu.extract s

    let inline ( >: ) (_: Prims.unit) (r: 'a) = r

#endif
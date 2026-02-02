
// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/fsharp/base/Prims.fs

#nowarn "86" //The '<=' operator should not normally be redefined. To define overloaded comparison semantics for a particular type, implement the 'System.IComparable' interface in the definition of that type.
#nowarn "62" //This construct is for ML compatibility. Consider using the '+' operator instead. This may require a type annotation to indicate it acts on strings. This message can be disabled using '--nowarn:62' or '#nowarn "62"'

module Nemonuri.FStarDotNet.Prims
open Nemonuri.FStarDotNet
open System.Numerics

type int       = bigint
type nonzero = int
let ( + )  (x:bigint) (y:int) = x + y
let ( - )  (x:int) (y:int) = x - y
let ( * )  (x:int) (y:int) = x * y
let ( / )  (x:int) (y:int) = fst (Z.ediv_rem x y)
let ( <= ) (x:int) (y:int)  = x <= y
let ( >= ) (x:int) (y:int)  = x >= y
let ( < )  (x:int) (y:int) = x < y
let ( > )  (x:int) (y:int) = x > y
let (mod) (x:int) (y:int)  = snd (Z.ediv_rem x y)
let mod_f x y = x mod y
let ( ~- ) (x:int) = -x
let abs (x:int) = BigInteger.Abs x
let of_int (x:FSharp.Core.int) = BigInteger x
let int_zero = of_int 0
let int_one = of_int 1
let parse_int = BigInteger.Parse
let to_string (x:int) = x.ToString()

type unit      = Microsoft.FSharp.Core.unit
type bool      = Microsoft.FSharp.Core.bool
type string    = Microsoft.FSharp.Core.string
type 'a array  = 'a Microsoft.FSharp.Core.array
type exn       = Microsoft.FSharp.Core.exn
type 'a list'  = 'a list
type 'a list   = 'a Microsoft.FSharp.Collections.list

type nat       = int
type pos       = int
type 'd b2t    = B2t of unit

type 'a squash = Squash of unit

type (' p, ' q) sum =
  | Left of ' p
  | Right of ' q

type (' p, ' q) l_or = sum<'p, 'q> squash

let uu___is_Left = function Left _ -> true | Right _ -> false

let uu___is_Right = function Left _ -> false | Right _ -> true

type (' p, ' q) pair =
| Pair of ' p * ' q

type (' p, ' q) l_and = pair<'p, 'q> squash

let uu___is_Pair _ = true


type trivial =
  | T

type l_True = trivial squash

let uu___is_T _ = true

type empty = unit
(*This is how Coq extracts Inductive void := . Our extraction needs to be fixed to recognize when there
       are no constructors and generate this type abbreviation*)
type l_False = empty squash

type (' p, ' q) l_imp = ('p -> 'q) squash

type (' p, ' q) l_iff = l_and<l_imp<'p, 'q>, l_imp<'q, 'p>>

type ' p l_not = l_imp<' p, l_False>

type (' a, ' p) l_Forall = L_forall of unit

type (' a, ' p) l_Exists = L_exists of unit


type (' p, ' q, 'dummyP) eq2 = Eq2 of unit
type (' p, ' q, 'dummyP, 'dummyQ) op_Equals_Equals_Equals = Eq3 of unit

type prop     = obj

let cut = ()
let admit () = failwith "no admits"
let _assume () = ()
let _assert x = ()
let magic () = failwith "no magic"
let unsafe_coerce x = unbox (box x)
let op_Negation x = not x

let op_Equality x y = x = y
let op_disEquality x y = x<>y
let op_AmpAmp x y = x && y
let op_BarBar x y  = x || y
let uu___is_Nil l = List.isEmpty l
let uu___is_Cons l = not (uu___is_Nil l)
let strcat x y = x ^ y

let string_of_bool (b:bool) = b.ToString()
let string_of_int (i:int) = i.ToString()

type ('a, 'b) dtuple2 =
  | Mkdtuple2 of 'a * 'b

let __proj__Mkdtuple2__item___1 x = 
    match x with
    | Mkdtuple2 (x, _) -> x
let __proj__Mkdtuple2__item___2 x = 
    match x with
    | Mkdtuple2 (_, x) -> x

let rec pow2 (n:int) = 
    if n = bigint 0 then
        bigint 1
    else
        (bigint 2) * pow2 (n - (bigint 1))

let __proj__Cons__item__tl = function
  | _::tl -> tl
  | _     -> failwith "Impossible"

let min = min
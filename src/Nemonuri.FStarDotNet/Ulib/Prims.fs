(**
- Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/ml/app/Prims.ml
*)

#nowarn "25" // Incomplete pattern matches
#nowarn "86" // Comparison perator should not normally be redefined

module Nemonuri.FStarDotNet.Prims

open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.OCamlDotNet.String
open Nemonuri.OCamlDotNet.Stdlib

type int = Z.t
let of_int = Z.of_int
let int_zero = Z.zero
let int_one = Z.one
let parse_int = Z.of_string

let to_string = Z.to_string

type attribute = Unit.t
let (cps : attribute) = ()
type [<RequireQualifiedAccess>] 'Auu____5 hasEq = | hasEq of unit
type eqtype = Unit.t
//type bool = 

type empty = Unit.t
(*This is how Coq extracts Inductive void := . Our extraction needs to be fixed to recognize when there
       are no constructors and generate this type abbreviation*)
type trivial =
  | T
let (uu___is_T : trivial -> bool) = fun projectee  -> true
type unit = Unit.t
type [<RequireQualifiedAccess>] 'Ap squash = | squash of unit
type [<RequireQualifiedAccess>] 'Ap auto_squash = | auto_squash of unit
type l_True = unit
type l_False = unit
type ('Aa,'Ax,'dummyV0) equals =
  | Refl
(**
ocaml 의 'explicit polymorphic type' 에 해당하는 것은, F#의 'explicit generic type' 인 것 같다.
*)
let inline uu___is_Refl<'Aa> : 'Aa -> 'Aa -> equals<'Aa,unit,unit> -> bool =
  fun x -> fun uu____65 -> fun projectee -> true
type [<RequireQualifiedAccess>] ('Aa,'Ax,'Ay) eq2 = | eq2 of unit
type [<RequireQualifiedAccess>] ('Aa,'Ab,'Ax,'Ay) op_Equals_Equals_Equals = | op_Equals_Equals_Equals of unit
type [<RequireQualifiedAccess>] 'Ab b2t = | b2t of unit
type ('Ap,'Aq) pair =
  | Pair of 'Ap * 'Aq
let uu___is_Pair<'Ap, 'Aq> : pair<'Ap, 'Aq> -> bool =
  fun projectee  -> true
let __proj__Pair__item___1<'Ap, 'Aq> : pair<'Ap, 'Aq> -> 'Ap =
  fun projectee  -> match projectee with | Pair (_0,_1) -> _0
let __proj__Pair__item___2<'Ap, 'Aq> : pair<'Ap, 'Aq> -> 'Aq =
  fun projectee  -> match projectee with | Pair (_0,_1) -> _1
type [<RequireQualifiedAccess>] ('Ap,'Aq) l_and = | l_and of unit
type ('Ap,'Aq) sum =
  | Left of 'Ap
  | Right of 'Aq
let uu___is_Left<'Ap, 'Aq> : sum<'Ap, 'Aq> -> bool =
  fun projectee  ->
    match projectee with | Left _0 -> true | uu____344 -> false

let __proj__Left__item___0<'Ap, 'Aq> : sum<'Ap, 'Aq> -> 'Ap =
  fun projectee  -> match projectee with | Left _0 -> _0
let uu___is_Right<'Ap, 'Aq> : sum<'Ap, 'Aq> -> bool =
  fun projectee  ->
    match projectee with | Right _0 -> true | uu____404 -> false

let __proj__Right__item___0<'Ap, 'Aq> : sum<'Ap, 'Aq> -> 'Aq =
  fun projectee  -> match projectee with | Right _0 -> _0
type [<RequireQualifiedAccess>] ('Ap,'Aq) l_or = | l_or of unit
type [<RequireQualifiedAccess>] ('Ap,'Aq) l_imp = | l_imp of unit
type [<RequireQualifiedAccess>] ('Ap,'Aq) l_iff = | l_iff of unit
type [<RequireQualifiedAccess>] 'Ap l_not = | l_not of unit
type [<RequireQualifiedAccess>] ('Ap,'Aq,'Ar) l_ITE = | l_ITE of unit
type [<RequireQualifiedAccess>] ('Aa,'Ab,'Auu____484,'Auu____485) precedes = | precedes of unit
type [<RequireQualifiedAccess>] ('Aa,'Auu____490,'Auu____491) has_type = | has_type of unit
type [<RequireQualifiedAccess>] ('Aa,'Ap) l_Forall = | l_Forall of unit
type prop = unit
let id x = x
type ('Aa,'Ab) dtuple2 =
  | Mkdtuple2 of 'Aa * 'Ab
let uu___is_Mkdtuple2<'Aa, 'Ab> : dtuple2<'Aa, 'Ab> -> bool =
  fun projectee  -> true
let __proj__Mkdtuple2__item___1<'Aa, 'Ab> :  dtuple2<'Aa, 'Ab> -> 'Aa =
  fun projectee  -> match projectee with | Mkdtuple2 (_1,_2) -> _1
let __proj__Mkdtuple2__item___2<'Aa, 'Ab> : dtuple2<'Aa, 'Ab> -> 'Ab =
  fun projectee  -> match projectee with | Mkdtuple2 (_1,_2) -> _2
type [<RequireQualifiedAccess>] ('Aa,'Ap) l_Exists = | l_Exists of unit
type string = String.t
type pure_pre = unit
type [<RequireQualifiedAccess>] ('Aa,'Apre) pure_post' = | pure_post' of unit
type [<RequireQualifiedAccess>] 'Aa pure_post = | pure_post of unit
type [<RequireQualifiedAccess>] 'Aa pure_wp = | pure_wp of unit
type [<RequireQualifiedAccess>] 'Auu____655 guard_free = | guard_free of unit
type [<RequireQualifiedAccess>] ('Aa,'Ax,'Ap) pure_return = | pure_return of unit
type [<RequireQualifiedAccess>] ('Ar1,'Aa,'Ab,'Awp1,'Awp2,'Ap) pure_bind_wp = | pure_bind_wp of 'Awp1
type [<RequireQualifiedAccess>] ('Aa,'Ap,'Awp_then,'Awp_else,'Apost) pure_if_then_else = | pure_if_then_else of unit
type [<RequireQualifiedAccess>] ('Aa,'Awp,'Apost) pure_ite_wp = | pure_ite_wp of unit
type [<RequireQualifiedAccess>] ('Aa,'Awp1,'Awp2) pure_stronger = | pure_stronger of unit
type [<RequireQualifiedAccess>] ('Aa,'Ab,'Awp,'Ap) pure_close_wp = | pure_close_wp of unit
type [<RequireQualifiedAccess>] ('Aa,'Aq,'Awp,'Ap) pure_assert_p = | pure_assert_p of unit
type [<RequireQualifiedAccess>] ('Aa,'Aq,'Awp,'Ap) pure_assume_p = | pure_assume_p of unit
type [<RequireQualifiedAccess>] ('Aa,'Ap) pure_null_wp = | pure_null_wp of unit
type [<RequireQualifiedAccess>] ('Aa,'Awp) pure_trivial = | pure_trivial of 'Awp
type [<RequireQualifiedAccess>] ('Ap, 'Apost) pure_assert_wp = | pure_assert_wp of unit
type [<RequireQualifiedAccess>] ('Aa,'Awp,'Auu____878) purewp_id = | purewp_id of 'Awp


let op_AmpAmp x y = x && y
let op_BarBar x y  = x || y
let op_Negation x = not x

let ( + )     = Z.add
let ( - )     = Z.sub
let ( * )     = Z.mul
let ( / )     = Z.ediv
let ( <= )    = Z.leq
let ( >= )    = Z.geq
let ( < )     = Z.lt
let ( > )     = Z.gt
let ( mod )   = Z.erem
let ( ~- )    = Z.neg
let abs       = Z.abs

(*
let op_Multiply x y = x * y
let op_Subtraction x y = x - y
let op_Addition x y = x + y
*)
let op_Minus x = -x
(*
let op_LessThan x y = x < y
let op_LessThanOrEqual x y = x <= y
let op_GreaterThan x y = x > y
let op_GreaterThanOrEqual x y = x >= y
*)
let op_Equality x y = x = y
let op_disEquality x y = x <> y

type exn = Microsoft.FSharp.Core.exn
type 'a array = Microsoft.FSharp.Core.array<'a>
let strcat x y = x ^ y
let op_Hat x y = x ^ y

type 'a list = Microsoft.FSharp.Collections.list<'a>
let uu___is_Nil<'Aa> : 'Aa list -> bool =
  fun projectee  -> match projectee with | []  -> true | uu____1190 -> false
let uu___is_Cons<'Aa> : 'Aa list -> bool =
  fun projectee  ->
    match projectee with | hd::tl -> true | uu____1216 -> false

let __proj__Cons__item__hd<'Aa> : 'Aa list -> 'Aa =
  fun projectee  -> match projectee with | hd::tl -> hd
let __proj__Cons__item__tl<'Aa> : 'Aa list -> 'Aa list =
  fun projectee  -> match projectee with | hd::tl -> tl
type pattern = unit


type [<RequireQualifiedAccess>] ('Aa,'Auu____1278) decreases = | decreases of unit
let returnM<'Aa> : 'Aa -> 'Aa = fun x  -> x

type [<RequireQualifiedAccess>] ('Aa,'Awp) as_requires = | as_requires of 'Awp
type [<RequireQualifiedAccess>] ('Aa,'Awp,'Ax) as_ensures = | as_ensures of unit
let admit () = failwith "Prims.admit: cannot be executed"
let magic () = failwith "Prims.magic: cannot be executed"
let unsafe_coerce<'Aa, 'Ab> : 'Aa -> 'Ab =
  fun x -> Obj.magic x

type 'Ap spinoff = 'Ap


type nat = int
type pos = int
type nonzero = int
let op_Modulus x y = x mod y
(*
let op_Division x y = x / y
*)
let rec pow2 : nat -> pos =
  fun x  ->
    Z.shift_left Z.one (Z.to_int x)

let (min : int -> int -> int) =
  fun x  -> fun y  -> if x <= y then x else y
(*
let (abs : int -> int) =
  fun x  -> if x >= (parse_int !"0"B) then x else op_Minus x
*)
let string_of_bool = string_of_bool
let string_of_int = to_string

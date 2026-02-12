// Reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/ml/app/FStar_Char.ml

module Nemonuri.FStarDotNet.FStar.Char

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet
open Nemonuri.OCamlDotNet.Batteries


module UChar = BatUChar

module U32 = FStar.UInt32

type char = Char.t
type char_code = U32.t

(* FIXME(adl) UChar.lowercase/uppercase removed from recent Batteries. Use Camomile? *)
let lowercase (x:char) : char =
  try Char.code (Char.lowercase_ascii (Char.chr x))
  with _ -> x

let uppercase (x:char) : char =
  try Char.code (Char.uppercase_ascii (Char.chr x))
  with _ -> x

let int_of_char (x:char) : Z.t= Z.of_int x
let char_of_int (i:Z.t) : char = Z.to_int i

let u32_of_char (x:char) : char_code = U32.of_native_int x
let char_of_u32 (x:char_code) : char = U32.to_native_int x
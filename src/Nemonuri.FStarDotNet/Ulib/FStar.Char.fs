// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/FStar.Char.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/ml/app/FStar_Char.ml

namespace Nemonuri.FStarDotNet.FStar

open Nemonuri.FStarDotNet
open Nemonuri.OCamlDotNet.Zarith
open Nemonuri.OCamlDotNet.Primitives
open System.Text
module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses

[<RequireQualifiedAccess>]
module Char =

    /// This module provides the [char] type, an abstract type
    /// representing UTF-8 characters.
    ///
    /// UTF-8 characters are representing in a variable-length encoding of
    /// between 1 and 4 bytes, with a maximum of 21 bits used to represent
    /// a code.
    ///
    /// See https://en.wikipedia.org/wiki/UTF-8 and
    /// https://erratique.ch/software/uucp/doc/unicode.html

#if false
    module U32 = FStar.UInt32
#endif

    (** [char] is a new primitive type with decidable equality *)
    type char = OCamlInt

    (** A [char_code] is the representation of a UTF-8 char code in
        an unsigned 32-bit integer whose value is at most 0x110000,
        and not between 0xd800 and 0xe000 *)
    
    /// type char_code = n: U32.t{U32.v n < 0xd7ff \/ (U32.v n >= 0xe000 /\ U32.v n <= 0x10ffff)}
    type char_code = Core.uint32

    (** A primitive to extract the [char_code] of a [char] *)

    /// val u32_of_char: char -> Tot char_code
    let u32_of_char (x: char) : char_code = Core.Operators.uint32 x


    (** A primitive to promote a [char_code] to a [char] *)
    /// val char_of_u32: char_code -> Tot char
    let char_of_u32 (x: char_code) : char = Core.Operators.int x

#if false
    (** Encoding and decoding from [char] to [char_code] is the identity *)
    val char_of_u32_of_char (c: char)
        : Lemma (ensures (char_of_u32 (u32_of_char c) == c)) [SMTPat (u32_of_char c)]

    (** Encoding and decoding from [char] to [char_code] is the identity *)
    val u32_of_char_of_u32 (c: char_code)
        : Lemma (ensures (u32_of_char (char_of_u32 c) == c)) [SMTPat (char_of_u32 c)]
#endif

    (** A couple of utilities to use mathematical integers rather than [U32.t]
        to represent a [char_code] *)
    /// let int_of_char (c: char) : nat = U32.v (u32_of_char c)
    let int_of_char (c: char) : Prims.int = Z.of_int c

    /// let char_of_int (i: nat{i < 0xd7ff \/ (i >= 0xe000 /\ i <= 0x10ffff)}) : char = char_of_u32 (U32.uint_to_t i)
    let char_of_int (i: Prims.int) : char = Z.to_int i
    

    (** Case conversion *)
    /// val lowercase: char -> Tot char
    let lowercase (x: char) : char = Rune.ToLowerInvariant(Rune(x)).Value

    /// val uppercase: char -> Tot char
    let uppercase (x: char) : char = Rune.ToUpperInvariant(Rune(x)).Value

#if false
    #set-options "--admit_smt_queries true"

    (** This private primitive is used internally by the compiler to
        translate character literals with a desugaring-time check of the
        size of the number, rather than an expensive verification check.
        Since it is marked private, client programs cannot call it
        directly Since it is marked unfold, it eagerly reduces,
        eliminating the verification overhead of the wrapper *)

    private unfold
    let __char_of_int (x: int) : char = char_of_int x
    #reset-options
#endif
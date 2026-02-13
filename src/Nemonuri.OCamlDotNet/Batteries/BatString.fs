/// String operations.
/// 
/// Given a string s of length l, we call character number in s the index of a character in s. 
/// Indexes start at 0, and we will call a character number valid in s if it falls within the range \[0...l-1\]. 
/// A position is the point between two characters or at the beginning or end of the string. We call a position valid in s if it falls within the range \[0...l\]. 
/// Note that character number n is between positions n and n+1.
/// 
/// Two parameters start and len are said to designate a valid substring of s if len >= 0 and start and start+len are valid positions in s.
/// 
/// This module replaces Stdlib's String module.
/// 
/// If you're going to do a lot of string slicing, BatSubstring might be a useful module to represent slices of strings, as it doesn't allocate new strings on every operation.
/// 
/// - Author(s): Xavier Leroy (base library), Nicolas Cannasse, David Teller, Edgar Friendly
/// - Reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatString.html
module Nemonuri.OCamlDotNet.Batteries.BatString
open Nemonuri.OCamlDotNet
open type Nemonuri.ByteChars.ByteStringTheory

type t = string

/// split_on_string sep s splits the string s into a list of strings which are separated by sep (excluded). 
/// split_on_string _ "" returns a single empty string. Note: split_on_string sep s is identical to nsplit s sep but for empty strings.
///
/// Example: String.split_on_string "bc" "abcabcabc" = ["a"; "a"; "a"; ""]
let split_on_string (sep: string) (s: string) : string list = 
    let sepLength = sep.Length
    let mutable e = SplitByteSpan(s.AsSpan(), sep.AsSpan())
    let mutable curList : string list = []
    while e.MoveNext() do
        let rrs = e.Current
        let newString = FromByteSpan(rrs.GetSliced().Slice(sepLength))
        curList <- curList @ [newString]

    curList

/// The comparison function for strings, with the same specification as Pervasives.compare. Along with the type t, this function compare allows the module String to be passed as argument to the functors Set.Make and Map.Make.
let inline compare (left: t) (right: t) = String.compare left right

/// `String.concat sep sl` concatenates the list of strings sl, inserting the separator string sep between each.
let inline concat (sep: string) (sl: string list) : string = String.concat sep sl

/// Return a copy of the argument, with all lowercase letters translated to uppercase, using the US-ASCII character set.
let uppercase_ascii = String.uppercase_ascii

/// Return a copy of the argument, with all uppercase letters translated to lowercase, using the US-ASCII character set.
let lowercase_ascii = String.lowercase_ascii

/// Return a copy of the argument, with special characters represented by escape sequences, following the lexical conventions of OCaml. 
/// If there is no special character in the argument, return the original string itself, not a copy. Its inverse function is Scanf.unescaped.
let escaped = String.escaped

/// Returns a string containing one given character.
///
/// Example:  String.of_char 's' = "s" 
let of_char (c: char) : string = 
    let charArray : char array = [|c|]
    !>charArray

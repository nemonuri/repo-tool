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
open Nemonuri.OCamlDotNet.Batteries

type t = string

/// split_on_string sep s splits the string s into a list of strings which are separated by sep (excluded). 
/// split_on_string _ "" returns a single empty string. Note: split_on_string sep s is identical to nsplit s sep but for empty strings.
///
/// Example: String.split_on_string "bc" "abcabcabc" = ["a"; "a"; "a"; ""]
let split_on_string (sep: string) (s: string) : string list = 
    let splited = s.Split([|sep|], System.StringSplitOptions.None) |> List.ofArray
    if s.EndsWith sep then splited @ [""] else splited

let inline compare (left: t) (right: t) = left.CompareTo right

/// `String.concat sep sl` concatenates the list of strings sl, inserting the separator string sep between each.
let inline concat (sep: string) (sl: string list) : string = t.Join(sep, values = sl)

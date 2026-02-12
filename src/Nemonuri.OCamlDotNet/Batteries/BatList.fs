/// Additional and modified functions for lists.
///
/// The OCaml standard library provides a module for list functions. 
/// This BatList module can be used to extend the List module or as a standalone module. 
/// It provides new functions and modify the behavior of some other ones (in particular all functions are now tail-recursive).
///
/// The following functions have the same behavior as the List module ones but are tail-recursive: 
/// map, append, concat, flatten, fold_right, remove_assoc, remove_assq, split. That means they will not cause a Stack_overflow when used on very long list.
///
/// The implementation might be a little more slow in bytecode, but compiling in native code will not affect performances.
///
/// This module extends Stdlib's List module, go there for documentation on the rest of the functions and types.
module Nemonuri.OCamlDotNet.Batteries.BatList
open Nemonuri.OCamlDotNet

module L = Nemonuri.OCamlDotNet.List

type 'a t = L.t<'a>

/// Concatenate a list of lists. The elements of the argument are all concatenated together (in the same order) to give the result. Tail-recursive.
let concat = L.concat

/// Same as concat.
let flatten  = L.flatten

/// map f [a0; a1; ...; an] applies function f to a0, a1, ..., an, and builds the list [f a0; f a1; ...; f an] with the results returned by f. Tail-recursive.
let map = L.map
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

/// Returns the first element of the list, or
/// 
/// - Raises Failure if the list is empty.
let hd = L.hd

/// Return the given list without its first element.
/// 
/// - Raises Failure if the list is empty.
let tl = L.tl

/// Concatenate a list of lists. The elements of the argument are all concatenated together (in the same order) to give the result. Tail-recursive.
let concat = L.concat

/// Same as concat.
let flatten  = L.flatten

/// map f [a0; a1; ...; an] applies function f to a0, a1, ..., an, and builds the list [f a0; f a1; ...; f an] with the results returned by f. Tail-recursive.
let map = L.map

/// Similar to Array.init, init n f returns the list containing the results of (f 0),(f 1).... (f (n-1)).
let init = L.init

/// Return the length (number of elements) of the given list.
let length = L.length

/// at l n returns the n-th element of the list l or
///
/// Raises Invalid_argument if the index is outside of l bounds. O(l)
let at (l : 'a list) n = 
    try l[n] with | :? System.ArgumentException as e -> Stdlib.invalid_arg !>e.Message

/// Obsolete. As at.
let nth = at

/// List reversal.
let rev = L.rev

/// append l1 l2 is a concatenation of l1 and l2. Same function as the infix operator @. Tail-recursive. This function takes O(length l1) time.
let append = L.append

/// List.rev_append l1 l2 reverses l1 and concatenates it to l2.
let rev_append = L.rev_append

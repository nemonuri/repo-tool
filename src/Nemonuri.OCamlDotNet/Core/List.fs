/// List operations.
///
/// Some functions are flagged as not tail-recursive. A tail-recursive function uses constant stack space, 
/// while a non-tail-recursive function uses stack space proportional to the length of its list argument, 
/// which can be a problem with very long lists. When the function takes several list arguments, 
/// an approximate formula giving stack usage (in some unspecified constant unit) is shown in parentheses.
///
/// The above considerations can usually be ignored if your lists are not longer than about 10000 elements.
module Nemonuri.OCamlDotNet.List
open Nemonuri.OCamlDotNet

module L = Microsoft.FSharp.Collections.List

/// An alias for the type of lists.
type 'a t = 'a list

/// Concatenate a list of lists. 
/// The elements of the argument are all concatenated together (in the same order) to give the result. 
/// Not tail-recursive (length of the argument + length of the longest sub-list).
let concat (ls: 'a list list) : 'a list = L.concat ls

/// Same as List.concat. Not tail-recursive (length of the argument + length of the longest sub-list).
let flatten = concat

//// <category name="Iterators">

/// map f [a1; ...; an] applies function f to a1, ..., an, and builds the list [f a1; ...; f an] with the results returned by f.
let map = L.map


//// </category>
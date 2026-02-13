/// List operations.
///
/// Some functions are flagged as not tail-recursive. A tail-recursive function uses constant stack space, 
/// while a non-tail-recursive function uses stack space proportional to the length of its list argument, 
/// which can be a problem with very long lists. When the function takes several list arguments, 
/// an approximate formula giving stack usage (in some unspecified constant unit) is shown in parentheses.
///
/// The above considerations can usually be ignored if your lists are not longer than about 10000 elements.
/// - Reference: https://ocaml.org/manual/5.4/api/List.html
module Nemonuri.OCamlDotNet.List
open Nemonuri.OCamlDotNet

module L = Microsoft.FSharp.Collections.List

/// An alias for the type of lists.
type 'a t = 'a list

/// Return the length (number of elements) of the given list.
let length = L.length

/// Compare the lengths of two lists. 
/// compare_lengths l1 l2 is equivalent to compare (length l1) (length l2), 
/// except that the computation stops after reaching the end of the shortest list.
[<TailCall>]
let rec compare_lengths (l1: 'a list) (l2: 'a list) : int = 
    match l1, l2 with
    | [], [] -> 0
    | [], _::_ -> 1
    | _::_, [] -> -1
    | _::tl1, _::tl2 -> compare_lengths tl1 tl2

/// Return the first element of the given list.
///
/// Raises Failure if the list is empty.
let hd l = try L.head l with | :? System.ArgumentException as e -> Forward.failwith e.Message

/// Return the given list without its first element.
/// 
/// Raises Failure if the list is empty.
let tl l = try L.tail l with | :? System.ArgumentException as e -> Forward.failwith e.Message




/// init len f is [f 0; f 1; ...; f (len-1)], evaluated left to right.
///
/// Since 4.06
/// Raises Invalid_argument if len < 0.
let init len f = 
    try
        L.init len f
    with
        | :? System.ArgumentException as e -> // Thrown when the input length is negative.
            Forward.invalid_arg e.Message

/// Return the n-th element of the given list. The first element (head of the list) is at position 0.
/// 
/// Raises
/// Failure if the list is too short.
/// Invalid_argument if n is negative.
let nth (l: 'a list) (n: int) : 'a = 
    if n < 0 then Forward.invalid_arg "n is negative"B else
    try
        l[n]
    with
        | :? System.ArgumentException as e -> Forward.failwith e.Message
    
/// List reversal.
let rev = L.rev

/// append l0 l1 appends l1 to l0. Same function as the infix operator @.
///
/// Since 5.1 this function is tail-recursive.
let append = L.append

/// rev_append l1 l2 reverses l1 and concatenates it with l2. This is equivalent to (List.rev l1) @ l2.
let inline rev_append l1 l2 = append (rev l1) l2

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
namespace Nemonuri.OCamlDotNet.Forwarded

module Fl = Microsoft.FSharp.Collections.List

/// reference: https://ocaml.org/manual/5.4/api/List.html
module List =

    type 'a t = Microsoft.FSharp.Collections.list<'a>

    let length l = Fl.length l

    [<TailCall>]
    let rec compare_lengths (l1: 'a list) (l2: 'a list) : int = 
        match l1, l2 with
        | [], [] -> 0
        | [], _::_ -> 1
        | _::_, [] -> -1
        | _::tl1, _::tl2 -> compare_lengths tl1 tl2
    
    let hd l = Fl.head l

    let tl l = Fl.tail l

    let init len f = Fl.init len f

    let nth (l: 'a list) (n: int) : 'a = l[n]

    let rev l = Fl.rev l

    let append l0 l1 = Fl.append l0 l1

    let rev_append l1 l2 = append (rev l1) l2

    let concat (ls: 'a list list) : 'a list = Fl.concat ls

    let flatten ls = concat ls

    let map f l = Fl.map f l

    let fold_right (f: 'a -> 'acc -> 'acc) (l: 'a list) (init: 'acc) : 'acc = Fl.foldBack f l init
    
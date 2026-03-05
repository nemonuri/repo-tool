namespace Nemonuri.PureTypeSystems.Primitives.Operations

open Nemonuri.PureTypeSystems.Primitives
open SingletonValueTuples

module Tuples =

    let inline push l r =
        let inline call (p': ^p) (l': ^l) (r': ^r) = ((^p or ^l) : (static member Push: _*_ -> _) l', r') in
        call Unchecked.defaultof<TuplePremise> l r

    let inline pop l =
        let inline call (p': ^p) (u': ^u) (l': ^l) = ((^p or ^l) : (static member Pop: _*_ -> _) u', l') in
        call Unchecked.defaultof<TuplePremise> () l

    let inline dequeue l =
        let inline call (p': ^p) (l': ^l) = ((^p or ^l) : (static member Dequeue: _ -> _) l') in
        call Unchecked.defaultof<TuplePremise> l

    
    
    module Operators =

        let inline ( &. ) l r = push l r

        let inline ( .&. ) l r = () &. l &. r

        
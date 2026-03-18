namespace Nemonuri.OCamlDotNet.Primitives

[<Struct; NoEquality; NoComparison>]
type ByRefOption<'T> = internal { HasValue: bool; mutable Value: 'T }


module ByRefOptions =

    type t<'a> = ByRefOption<'a>

    let none<'a> = { HasValue = false; Value = Unchecked.defaultof<'a> }

    let some (v: 'a) = { HasValue = true; Value = v }

    let isSome (r: byref<t<'a>>) = r.HasValue

    let isNone (r: byref<t<'a>>) = not (isSome &r)

    let ensureSome (r: byref<t<'a>>) (thunk: unit -> 'a) =
        if isSome &r then 
            ()
        else
            let v = thunk() in r <- some v;
        &r.Value
    
    type Tapper<'a> = delegate of byref<'a> -> unit

    let ensureNone (r: byref<t<'a>>) (tap: Tapper<'a>) =
        if isNone &r then
            ()
        else
            tap.Invoke(&r.Value);
            r <- none
    


namespace Nemonuri.OCamlDotNet.Forwarded

module Fm = Microsoft.FSharp.Collections.Map

/// https://ocaml.org/manual/5.4/api/Map.Make.html
module Map =   

    [<AbstractClass; Sealed>]
    type S<'key when 'key : comparison> =

        static member empty<'a>() : t<'key,'a> = Fm.empty<'key,'a>

        static member add<'a> key data m : t<'key,'a> = Fm.add key data m

        static member of_list<'a> bs : t<'key,'a> = Fm.ofList bs

        static member find_opt<'a> (x: 'key) (m: t<'key,'a>)  = Fm.tryFind x m

        static member iter<'a> (f: 'key -> 'a -> unit) (m: t<'key,'a>) = Fm.iter f m

        static member fold<'a, 'acc> (f: 'key -> 'a -> 'acc -> 'acc) (m: t<'key,'a>) (init: 'acc) = Fm.foldBack f m init

        static member update<'a> (key: 'key) (f: 'a option -> 'a option) (m: t<'key,'a>) = Fm.change key f m

        static member remove<'a> (x: 'key) (m: t<'key,'a>) = Fm.remove x m


    and t<'key,'a when 'key : comparison> = Microsoft.FSharp.Collections.Map<'key,'a>

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

    and t<'key,'a when 'key : comparison> = Microsoft.FSharp.Collections.Map<'key,'a>

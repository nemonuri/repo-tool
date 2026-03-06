namespace Nemonuri.FStarDotNet.FStar

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Primitives
open Nemonuri.FStarDotNet.Forwarded

#if false


[<RequireQualifiedAccess>]
module Pervasives =

    module Native =

        module F = FStar_Pervasives_Native

        let None<'a> = F.None<'a>
        let Some = F.Some

        type option<'a> = F.option<'a>

        let (|None|Some|) = F.(|None|Some|)

        let Mktuple2 = F.Mktuple2
        type tuple2<'a,'b> = F.tuple2<'a,'b>
        let (|Mktuple2|) = F.(|Mktuple2|)

        let fst = F.fst
        let snd = F.snd

        let Mktuple3 = F.Mktuple3
        type tuple3<'a,'b,'c> = F.tuple3<'a,'b,'c>
        let (|Mktuple3|) = F.(|Mktuple3|)

#endif
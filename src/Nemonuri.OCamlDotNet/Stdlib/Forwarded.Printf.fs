namespace Nemonuri.OCamlDotNet.Forwarded

open Nemonuri.Buffers
open type Nemonuri.Transcodings.TranscoderTheory
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.OCamlDotNet.Primitives.TypeShadowing
open Nemonuri.OCamlDotNet.Primitives.FormatBasics
open Nemonuri.OCamlDotNet.Primitives.FormatBasics.Finalizers
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources
module S = Nemonuri.OCamlDotNet.Forwarded.String

/// reference: https://ocaml.org/manual/5.4/api/Printf.html
module Printf =

    type private bd = DrainableArrayBuilder<byte>

    let fprintf (outchan: out_channel) (format: format<_, out_channel, unit>) =
        let fin = TapFinalizer<out_channel,unit,FlushTap<out_channel>,NoneTap<unit>>.Instance
        let cfg = WriterFactories.toConfigAuto outchan () in
        Formats.write fin cfg format

    let printf format = fprintf Out_channel.stdout format

    let eprintf format = fprintf Out_channel.stderr format

    type private StringWriter() =
        class
            static member Instance = StringWriter()

            static member Format (str: string) (Segments.FlattenSegment(le,fo,tr,tc)) (s: 'TSource) : string =
                let mutable bd = bd(S.length str) in
                let mutable ph: int = 0 in
                bd.AddRange(Obs.stringToReadOnlySpan str);
                bd.AddRange(Obs.stringToReadOnlySpan le);
                tc.TranscodeSingletonWhileDestinationTooSmall(s,&bd,fo,&ph);
                bd.AddRange(Obs.stringToReadOnlySpan tr);
                bd.DrainToArraySemgent() |> Obs.Unsafe.stringOfArraySegment

            interface IFormatter<string> with
                member _.Format str seg s: string = StringWriter.Format str seg s
        end

    let private sprintfCfg() =
        let cfg: WriterFactoryConfig<unit,string> = { BufferWriter = ValueNone; ResultWriter = ValueSome StringWriter.Instance; InitialBuffer = (); InitialResult = S.empty } in
        cfg

    let sprintf (format: format<_, unit, string>) =
        let fin = TapFinalizer<unit,string,NoneTap<unit>,NoneTap<string>>.Instance in
        let cfg = sprintfCfg() in
        Formats.write fin cfg format
    
    let ksprintf (cont: string -> 'd) (format: format4<_, unit, string, 'd>) =
        let fin = 
            { new IFinalizer<unit, string, 'd> with 
                member x.Finalize (b: byref<unit>, r: byref<string>, e: exn option): 'd = cont r }
        in
        let cfg = sprintfCfg() in
        Formats.write fin cfg format
        

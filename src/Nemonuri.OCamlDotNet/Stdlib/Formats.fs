namespace Nemonuri.OCamlDotNet.Primitives


open Nemonuri.OCamlDotNet.Primitives.FormatBasics
module Bs = Nemonuri.OCamlDotNet.Primitives.ByteSpans
module Obs = Nemonuri.OCamlDotNet.Primitives.OCamlByteSpanSources


[<NoEquality; NoComparison>]
[<Struct>]
type OCamlFormat<'TContext, 'TBuffer, 'TResult, 'TFinal> =
    private | OCamlFormat of LegacyFormat<'TContext, 'TBuffer, 'TResult> //* Fb.IFinalizer<'TBuffer, 'TResult, 'TFinal>

module Formats =

    let write (fin: IFinalizer<_,_,_>) config (OCamlFormat(lfmt)) ctx =
        let env = WriterFactories.legacyFormatToEnv config lfmt in
        let exnOpt =
            try
                Legacies.toWriterFactory lfmt env ctx; None
            with e -> 
                Some e
        in
        let mutable bs = env.BufferState in
        let mutable rs = env.ResultState in
        fin.Finalize(&bs, &rs, exnOpt)

    let empty = OCamlFormat(Legacies.empty)

    let cons hd (tl: OCamlFormat<_,_,_,_>) = 
        let (OCamlFormat(ltl)) = tl in
        OCamlFormat(Legacies.cons hd ltl)

    let head (OCamlFormat(lf)) = Legacies.head lf

    let replaceHead hd (OCamlFormat(lf)) = Legacies.replaceHead hd lf |> OCamlFormat
        
    let ofSegment (s: Segment<'s>) = empty |> cons s

    let toUnit b = Segments.toUnit b |> ofSegment
    
    /// reference: https://ocaml.org/manual/5.4/api/Printf.html
    module Literals = begin

        module L = Nemonuri.OCamlDotNet.Primitives.FormatBasics.Segments.Literals

        /// convert a boolean argument to the string `true` or `false`
        let B() = L.B |> ofSegment

        /// convert an integer argument to signed decimal.
        let d() = L.d |> ofSegment

        /// convert an int32 argument to the format specified by the second letter. (decimal)
        let ld() = L.ld |> ofSegment

        /// convert an int64 argument to the format specified by the second letter.
        let Ld() = L.Ld |> ofSegment

        /// convert an int64 argument to the format specified by the second letter.
        let nd() = L.nd |> ofSegment

        /// insert a character argument.
        let c() = L.c |> ofSegment

        /// convert a character argument to OCaml syntax (single quotes, escapes).
        let C() = L.C |> ofSegment

        let ``()``() = toUnit "()"B

        /// insert a string argument.
        let s() = L.s |> ofSegment

        /// convert a string argument to OCaml syntax (double quotes, escapes).
        let S() = L.S |> ofSegment

    end

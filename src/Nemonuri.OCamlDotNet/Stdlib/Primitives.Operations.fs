namespace Nemonuri.OCamlDotNet.Primitives

open System
open type System.MemoryExtensions

module Operations =

    module DotNetArrays =

        let createUninitialized<'a> (n: OCamlInt) =
#if NET8_0_OR_GREATER
            System.GC.AllocateUninitializedArray<'a>(length = n, pinned = false)
#else
            Array.create n Unchecked.defaultof<'a>
#endif

    module internal Unsafe =

        let sourceToBytes (source: OCamlByteSequenceSource) = { OCamlBytes.Source = source }

        let sourceOfBytes (bytes: OCamlBytes) = bytes.Source

        let sourceToString (source: OCamlByteSequenceSource) = { OCamlString.Source = source }

        let sourceOfString (str: OCamlString) = str.Source

    module OCamlByteSequenceSources =

        let empty = OCamlByteSequenceSource.None

        let ofArray (source: OCamlChar array) = 
            source
            |> ArraySegment<_>
            |> OCamlByteSequenceSource.Array
        
        let inline toSpan (source: OCamlByteSequenceSource) = source.AsReadOnlySpan()

        let inline unsafeToSpan (source: OCamlByteSequenceSource) = source.UnsafeAsSpan()

        let inline clone (source: OCamlByteSequenceSource) = (toSpan source).ToArray() |> ofArray

    module ByteSpans =

        open type Nemonuri.ByteChars.Extensions.UnsafePinnedSpanPointerExtensions
        type private bs = Span<OCamlChar>
        type private rbs = ReadOnlySpan<OCamlChar>
        type private ps = Nemonuri.ByteChars.UnsafePinnedSpanPointer<OCamlChar>
        type private bd = Nemonuri.ByteChars.ArrayBuilder<OCamlChar>

        let inline length (s: rbs) = s.Length

        let inline get (s: rbs) (n: OCamlInt) : OCamlChar = s[n]

        let inline set (s: bs) (n: OCamlInt) (c: OCamlChar) = s[n] <- c

        let inline toSource (s: rbs) = OCamlByteSequenceSources.ofArray (s.ToArray())

        let inline extend (s: rbs) left right =
            let ary = DotNetArrays.createUninitialized<OCamlChar>(left + length s + right)
            s.CopyTo(ary.AsSpan().Slice(left))
            ary
        
        let inline fill (src: bs) pos len c = 
            src.Slice(pos, len).Fill(c)
        
        let inline blit (src: rbs) src_pos (dst: bs) dst_pos len =
            src.Slice(src_pos, len).CopyTo(dst.Slice(dst_pos))
        
        let inline concat (sep: rbs) (sl: #seq<ps>) =
            use _ = fixed sep
            let pp = Nemonuri.ByteChars.UnsafePinnedSpanPointerTheory.FromPinnedSpan(sep)

            ((bd(), false), sl)
            ||> Seq.fold 
                    (fun (builder, looped) elem -> 
                        if looped then builder.Append(pp.LoadReadOnlySpan()) else ()
                        builder.Append(elem.LoadReadOnlySpan())
                        (builder, true))
            |> Core.Operators.fst |> _.DrainToArraySemgent()
        
        let inline cat (s1: ps) (s2: ps) = concat (rbs()) (seq { s1; s2 })
            
        //let inline iter 
            
        



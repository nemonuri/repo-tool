namespace Nemonuri.OCamlDotNet.Primitives

open System

module Operations =

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

        type private bs = Span<OCamlChar>
        type private rbs = ReadOnlySpan<OCamlChar>

        let inline length (s: rbs) = s.Length

        let inline get (s: rbs) (n: OCamlInt) : OCamlChar = s[n]

        let inline set (s: bs) (n: OCamlInt) (c: OCamlChar) = s[n] <- c

        let inline toSource (s: rbs) = OCamlByteSequenceSources.ofArray (s.ToArray())


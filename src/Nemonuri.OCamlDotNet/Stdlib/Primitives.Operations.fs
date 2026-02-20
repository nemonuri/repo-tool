namespace Nemonuri.OCamlDotNet.Primitives

open System
open Nemonuri.ByteChars
open System.Collections.Immutable
open type System.MemoryExtensions
open Microsoft.FSharp.NativeInterop

module Operations =

    module DotNetArrays =

        let createUninitialized<'a> (n: OCamlInt) =
#if NET8_0_OR_GREATER
            System.GC.AllocateUninitializedArray<'a>(length = n, pinned = false)
#else
            Array.create n Unchecked.defaultof<'a>
#endif

    [<Sealed; AbstractClass>]
    type DotNetSpans =
        static member inline WriteAndCount(dest: Span<'T>, s0: 'T) = dest[0] <- s0; 1
        static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T) = dest[1] <- s1; DotNetSpans.WriteAndCount(dest, s0)+1
        static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T, s2: 'T) = dest[2] <- s2; DotNetSpans.WriteAndCount(dest, s0, s1)+1
        static member inline WriteAndCount(dest: Span<'T>, s0: 'T, s1: 'T, s2: 'T, s3: 'T) = dest[3] <- s3; DotNetSpans.WriteAndCount(dest, s0, s1, s2)+1

        static member inline NativePtrToSpan(ptr: nativeptr<'T>, length: int) = Span<'T>(ptr |> NativePtr.toVoidPtr, length)


    module internal Unsafe =

        let sourceToBytes (source: OCamlByteSequenceSource) = { OCamlBytes.Source = source }

        let sourceOfBytes (bytes: OCamlBytes) = bytes.Source

        let sourceToString (source: OCamlByteSequenceSource) = { OCamlString.Source = source }

        let sourceOfString (str: OCamlString) = str.Source

    module internal OCamlByteSequenceSources =

        let empty = OCamlByteSequenceSource.None

        let ofArraySegemnt (source: ArraySegment<OCamlChar>) = source |> OCamlByteSequenceSource.Array

        let ofArray (source: OCamlChar array) = source |> ArraySegment<_> |> ofArraySegemnt
        
        let inline toSpan (source: OCamlByteSequenceSource) = source.AsReadOnlySpan()

        let inline unsafeToSpan (source: OCamlByteSequenceSource) = source.UnsafeAsSpan()

        let inline clone (source: OCamlByteSequenceSource) = (toSpan source).ToArray() |> ofArray

    module ByteSpans =

        open type Nemonuri.ByteChars.Extensions.UnsafePinnedSpanPointerExtensions
        open type Nemonuri.ByteChars.ByteCharTheory
        type private bs = Span<OCamlChar>
        type private rbs = ReadOnlySpan<OCamlChar>
        type private ps = Nemonuri.ByteChars.UnsafePinnedSpanPointer<OCamlChar>
        type private trbs = ITemporaryReadOnlySpanSource<OCamlChar>
        type private bd = Nemonuri.ByteChars.ArrayBuilder<OCamlChar>
        type private Cc = Nemonuri.ByteChars.ByteCharConstants
        type private Cth = Nemonuri.ByteChars.ByteCharTheory
        type private Dns = DotNetSpans

        let [<Literal>] StackAllocThreshold = 256

        let length (s: rbs) = s.Length

        let get (s: rbs) (n: OCamlInt) : OCamlChar = s[n]

        let set (s: bs) (n: OCamlInt) (c: OCamlChar) = s[n] <- c

        //let toSource (s: rbs) = OCamlByteSequenceSources.ofArray (s.ToArray())

        let extend (s: rbs) left right =
            let ary = DotNetArrays.createUninitialized<OCamlChar>(left + length s + right)
            s.CopyTo(ary.AsSpan().Slice(left))
            ary
        
        let fill (src: bs) pos len c = 
            src.Slice(pos, len).Fill(c)
        
        let blit (src: rbs) src_pos (dst: bs) dst_pos len =
            src.Slice(src_pos, len).CopyTo(dst.Slice(dst_pos))
        
        let inline private toPinnedSpan (s: rbs) = Nemonuri.ByteChars.UnsafePinnedSpanPointerTheory.FromPinnedSpan s

        let concat (sep: #trbs) (sl: #seq<#trbs>) =
            ((bd(0), false), sl)
            ||> Seq.fold 
                    (fun (builder, looped) elem -> 
                        if looped then builder.Append(sep.AsTemporarySpan()) else ()
                        builder.Append(elem.AsTemporarySpan())
                        (builder, true))
            |> Core.Operators.fst |> _.DrainToArraySemgent()
        
        let cat<'t1, 't2 when 't1 :> trbs and 't2 :> trbs> (s1: 't1) (s2: 't2) = 
            let builder = bd(2)
            builder.Append(s1.AsTemporarySpan())
            builder.Append(s2.AsTemporarySpan())
            builder.DrainToArraySemgent()

        let inline private bsToRbs (s: bs) : rbs = s
            
        [<Sealed; AbstractClass; AutoOpen>]
        type OverloadTheory =

            static member Cat<'a when 'a :> trbs> (s1: ps) = s1 |> OCamlByteSequenceSource.PinnedPointer |> cat<_, 'a>
            static member Cat<'a when 'a :> trbs> (pinned: rbs) = toPinnedSpan pinned |> Cat<'a>
            static member Cat<'a when 'a :> trbs> (pinned: bs) = Cat<'a> (bsToRbs pinned)
        
        let iter (f: OCamlChar -> unit) (s: rbs) = for c in s do f c

        let iteri (f: OCamlInt -> OCamlChar -> unit) (s: rbs) =
            for i = 0 to s.Length-1 do f i (s[i])
        
        let map (f: OCamlChar -> OCamlChar) (s: rbs) =
            let result = DotNetArrays.createUninitialized s.Length
            for i = 0 to s.Length-1 do result[i] <- f s[i]
            result
        
        let mapi (f: OCamlInt -> OCamlChar -> OCamlChar) (s: rbs) =
            let result = DotNetArrays.createUninitialized s.Length
            for i = 0 to s.Length-1 do result[i] <- f i s[i]
            result

        let fold_left (f: 'acc -> OCamlChar -> 'acc) (x: 'acc) (s: rbs) =
            let mutable result = x
            for c in s do result <- f result c
            result
        
        let fold_right (f: OCamlChar -> 'acc -> 'acc) (s: rbs) (x: 'acc) =
            let mutable result = x
            for i = s.Length-1 downto 0 do result <- f s[i] result
            result
        
        let for_all (p: OCamlChar -> bool) (s: rbs) =
            let mutable result = true
            let mutable e = s.GetEnumerator()
            while result && e.MoveNext() do result <- p e.Current
            result
        
        let exists (p: OCamlChar -> bool) (s: rbs) =
            let mutable result = false
            let mutable e = s.GetEnumerator()
            while not result && e.MoveNext() do result <- p e.Current
            result
        
        let AsciiWhitespaces = ImmutableArray.Create(' 'B, '\012'B, '\n'B, '\r'B, '\t'B)

        let trim (s: rbs) =
            let trimElems = AsciiWhitespaces.AsSpan()
#if NET8_0_OR_GREATER
            s.Trim(trimElems)
#else
            Nemonuri.NetStandards.MemoryTheory.Trim(s, trimElems)
#endif

        let [<Literal>] private ``/`` = Cc.AsciiBackslash
        
        let internal escapeCharAndCount (dest: Span<OCamlChar>) (c: OCamlChar) =
            let inline divRem10 n = let d = 10uy in n / d, Cth.Modulus(n, d) |> Cth.UncheckedIntegerToDecimalDigit
            match c with
            | Cc.AsciiBackslash | Cc.AsciiDoubleQuote | Cc.AsciiSingleQuote -> Dns.WriteAndCount(dest, ``/``, c)
            | Cc.AsciiLineFeed -> Dns.WriteAndCount(dest, ``/``, 'n'B)
            | Cc.AsciiCarriageReturn -> Dns.WriteAndCount(dest, ``/``, 'r'B)
            | Cc.AsciiHorizontalTabulation -> Dns.WriteAndCount(dest, ``/``, 't'B)
            | Cc.AsciiBackspace -> Dns.WriteAndCount(dest, ``/``, 'b'B)
            | Cc.AsciiSpace -> Dns.WriteAndCount(dest, ``/``, ' 'B)
            | c0 when Cc.AsciiPrintableMinimum <= c0 && c0 <= Cc.AsciiPrintableMaximum -> Dns.WriteAndCount(dest, c0)
            | _ -> 
                let q2, r2 = divRem10 c
                let q1, r1 = divRem10 q2
                let _, r0 = divRem10 q1
                Dns.WriteAndCount(dest, ``/``, r0, r1, r2)
        
        let escaped (s: rbs) =
            let builder = bd(s.Length)
            let tempStore = DotNetSpans.NativePtrToSpan(NativePtr.stackalloc<OCamlChar> 4, 4)
            for c in s do
                let stepCount = escapeCharAndCount tempStore c
                builder.Append(tempStore.Slice(0, stepCount))
            builder.DrainToArraySemgent()
        
        let inline internal (|GreaterThanOrEqualToZero|_|) (v: int) = if v >= 0 then ValueSome v else ValueNone

        let index_from (s: rbs) (i: OCamlInt) (c: OCamlChar) = 
            match s.Slice(i).IndexOf(c) with
            | GreaterThanOrEqualToZero v -> v + i
            | _ -> -1

        let checked_index_from s i c =
            match index_from s i c with
            | GreaterThanOrEqualToZero v -> v
            | _ -> raise Exceptions.Not_found

        let index_from_opt s i c = 
            match index_from s i c with
            | GreaterThanOrEqualToZero v -> Some v
            | _ -> None

        let index (s: rbs) (c: OCamlChar) = index_from s 0 c

        let checked_index s c = checked_index_from s 0 c

        let index_opt s c = index_from_opt s 0 c
        
        let inline private lengthMinus1 (s: rbs) = s.Length - 1

        let rindex_from (s: rbs) (i: OCamlInt) (c: OCamlChar) = s.Slice(0,i+1).LastIndexOf(c)

        let checked_rindex_from s i c =
            match rindex_from s i c with
            | GreaterThanOrEqualToZero v -> v
            | _ -> raise Exceptions.Not_found

        let rindex_from_opt s i c = 
            match rindex_from s i c with
            | GreaterThanOrEqualToZero v -> Some v
            | _ -> None

        let rindex (s: rbs) (c: OCamlChar) = rindex_from s (lengthMinus1 s) c

        let checked_rindex s c = checked_rindex_from s (lengthMinus1 s) c

        let rindex_opt s c = rindex_from_opt s (lengthMinus1 s) c
        
        let contains_from (s: rbs) (start: OCamlInt) (c: OCamlChar) = 
            match index_from s start c with
            | GreaterThanOrEqualToZero _ -> true
            | _ -> false
        
        let contains (s: rbs) c = contains_from s 0 c

        let rcontains_from (s: rbs) stop c = 
            match rindex_from s stop c with
            | GreaterThanOrEqualToZero _ -> true
            | _ -> false

        let inline private spanToArraySegement (s: rbs) = s.ToArray() |> ArraySegment<_>

        let uppercase_ascii (s: rbs) = 
            let mutable result = spanToArraySegement s
            UpdateToUpperCase<ByteArraySegmentPremise,_>(&result)
            result

        let lowercase_ascii  (s: rbs) = 
            let mutable result = spanToArraySegement s
            UpdateToLowerCase<ByteArraySegmentPremise,_>(&result)
            result
            
        

            


                


            
        



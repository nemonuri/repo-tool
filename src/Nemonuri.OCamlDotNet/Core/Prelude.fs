namespace Nemonuri.OCamlDotNet

open Nemonuri.ByteChars

[<AutoOpen>]
module Prelude =

    type char = Microsoft.FSharp.Core.byte
    type int = Microsoft.FSharp.Core.int
    type string = System.Collections.Immutable.ImmutableArray<char>
    type bool = Microsoft.FSharp.Core.bool
    type unit = Microsoft.FSharp.Core.unit
    type nativeint = Microsoft.FSharp.Core.nativeint

    [<Struct; RequireQualifiedAccess>]
    type StringDomain =
        | None
        | ByteString of byteString : string
        | CharArray of chars : char array
        | DotNetString of dotNetString : System.String

        member x.ToByteString() : string =
            match x with
            | None -> ByteStringTheory.Empty
            | ByteString b -> b
            | CharArray c -> ByteStringTheory.FromByteSpan c
            | DotNetString d -> ByteStringTheory.DotNetStringToUtf8ByteString d

        static member inline op_Implicit (byteString: string) : StringDomain = ByteString byteString
        static member inline op_Implicit (chars: char array) : StringDomain = CharArray chars
        static member inline op_Implicit (dotNetString: System.String) : StringDomain = DotNetString dotNetString

    let inline ( ! ) (chars: StringDomain) : string = chars.ToByteString()


module internal Forward =

    exception Invalid_argument = System.ArgumentException

    [<CompiledNameAttribute("MatchInvalidArgument")>]
    let (|Invalid_argument|_|) (e: exn) =
        match e with
        | :? Invalid_argument as inv -> Some inv.Message
        | _ -> None

    let invalid_arg message = System.ArgumentException message |> raise

    let inline equal a0 a1 = Microsoft.FSharp.Core.LanguagePrimitives.GenericEquality a0 a1

    let inline compare a0 a1 = Microsoft.FSharp.Core.LanguagePrimitives.GenericComparison a0 a1

    exception Division_by_zero = System.DivideByZeroException
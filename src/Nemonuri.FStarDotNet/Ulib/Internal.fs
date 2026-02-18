#nowarn "1204"

namespace Nemonuri.FStarDotNet

#if false
module internal Internal =

    type has_type<'a, 'Type>() =

        member _.Invoke<'_1 when '_1 :> elem<'a>>(o: '_1) =
            match
                Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicFunctions.TypeTestGeneric<'Type>(o)
            with
            | true -> typeof<FStarBool.True>
            | false -> typeof<FStarBool.False>
        
        static member Singleton = has_type<'a, 'Type>()
#endif
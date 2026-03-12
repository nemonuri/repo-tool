
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Show.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/class/FStarC.Class.Show.fst

namespace Nemonuri.FStarDotNet.FStarC.Class

open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.Operators

module Show =

    type showable<'a> =
        interface
            abstract member show: 'a -> Prims.string
        end
    
    type showable_unit =
        struct
            interface showable<unit> with
                member _.show _ = (toString "()"B)
        end

    type showable_bool =
        struct
            interface showable<bool> with
                member _.show b = Prims.string_of_bool b
        end
    

    type showable_int =
        struct
            interface showable<Prims.int> with
                member _.show n = Prims.string_of_int n
        end

    type showable_nat =
        struct
            interface showable<Prims.int> with
                member _.show n = (Unchecked.defaultof<showable_int> :> showable<Prims.int>).show n
        end

    type showable_string =
        struct
            interface showable<Prims.string> with
                member _.show x = (toString "\""B) ^ x ^ (toString "\""B)
        end

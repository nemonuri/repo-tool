namespace Nemonuri.FStarDotNet


[<AbstractClass; Sealed>]
type CharListTheory =

    static member ToCharList(x: char list) = x
    static member ToCharList(x: int list) = List.map Operators.char x
    static member ToCharList(x: bigint list) = 
        let inline toShort (n: bigint) : int16 = bigint.op_Explicit n
        List.map (toShort >> Operators.char) x
    

    static member inline private ToCharListAux'<^T, ^Theory when (^T or ^Theory) : (static member ToCharList : ^T -> list<char>) > (x: ^T) = 
        ((^T or ^Theory) : (static member ToCharList : ^T -> list<char>) x)

    static member inline ToCharList' (x: ^T) = CharListTheory.ToCharListAux'<^T, CharListTheory> x

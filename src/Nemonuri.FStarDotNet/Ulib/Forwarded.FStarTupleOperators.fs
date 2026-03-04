namespace Nemonuri.FStarDotNet.Forwarded

module Fu = Nemonuri.FStarDotNet.Primitives.FStarTypeUniverses
module Tu = Nemonuri.FStarDotNet.Primitives.Operations.Tuples

module FStarTupleOperators =

    let inline ( &. ) l r = Fu.monad { let! l' = l in return Tu.addItem l' r }

    let inline ( .&. ) l r = (Fu.pur ()) &. l &. r

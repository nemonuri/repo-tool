namespace Nemonuri.FStarDotNet.Primitives

    open System
    open System.Collections
    open System.Collections.Generic

    module Abbreviations =

        type inst<'a> = IFStarInstance<'a>

        type refine<'p when 'p :> IFStarType> = IFStarRefinement<'p>

        /// FStarTypeTheory.GetDotNetTypes
        let dnTypes (self: ISolvedFStarType<'T>) = FStarTypeTheory.GetDotNetTypes self

        /// FStarTypeTheory.GetSolvedDotNetType
        let sdnType (self: ISolvedFStarType<'T>) = FStarTypeTheory.GetSolvedDotNetType self

        type eqInst<'a when 'a :> IEquatable<'a>> = IFStarEquatableInstance<'a, EqualityComparer<'a>>

        type seqInst<'a when 'a :> IStructuralEquatable> = IFStarEquatableInstance<'a, IEqualityComparer>

        let seqComparer = StructuralComparisons.StructuralEqualityComparer
    
    module EmptyValues =

        let typeList = TypeListTheory.Empty

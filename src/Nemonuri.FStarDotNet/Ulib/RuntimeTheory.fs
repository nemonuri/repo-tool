namespace Nemonuri.FStarDotNet

module internal RuntimeTheory =

    let inline typeeq<'T1, 'T2> = LanguagePrimitives.PhysicalEquality typeof<'T1> typeof<'T2>

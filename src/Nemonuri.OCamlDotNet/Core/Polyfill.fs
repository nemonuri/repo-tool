#if NETSTANDARD2_0

namespace System.Runtime.CompilerServices
open System;

/// [System.Runtime.CompilerServices.IsByRefLikeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isbyreflikeattribute?view=net-10.0)
[<AttributeUsage(AttributeTargets.Struct)>]
type internal IsByRefLikeAttribute() =
    inherit Attribute()

#endif
global using System.Text;
global using System.Collections.Immutable;
global using System.Runtime.InteropServices;
global using System.Runtime.CompilerServices;

global using CommunityToolkit.Diagnostics;

#if NET8_0_OR_GREATER
global using System.Numerics.Tensors;
#endif

#if NETSTANDARD2_0 || NETSTANDARD2_1
global using Math = Nemonuri.NetStandards.Math;
#endif
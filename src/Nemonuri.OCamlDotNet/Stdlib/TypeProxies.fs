namespace Nemonuri.OCamlDotNet.Primitives.TypeProxies

#if false
open Nemonuri.Buffers
open Nemonuri.OCamlDotNet.Primitives
open Nemonuri.PureTypeSystems.Primitives

type OCamlFormatProxy6<'a,'b,'c,'d,'e,'f> = internal { BoxedValue: obj }

type OCamlFormatProxy4<'a,'b,'c,'d> = internal { BoxedValue: obj }

type OCamlFormatProxy<'a,'b,'c> = internal { BoxedValue: obj }

type OCamlTypeProxyPremise =
    struct
        static member ToDotNet(format: OCamlFormatProxy<'a,OCamlOutChannel,unit>) : OCamlFormatShim<'a,StreamWithByteArrayPool,'t> = 
            format.BoxedValue |> unbox


    end
#endif
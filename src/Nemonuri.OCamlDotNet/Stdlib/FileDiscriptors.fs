namespace Nemonuri.OCamlDotNet.Primitives

open System.IO
open Nemonuri.Posix
open Nemonuri.PureTypeSystems.Primitives
open Nemonuri.PureTypeSystems.Predicates

module FileDiscriptors =

    type private t = FileDescriptor
    type private Fth = FileDescriptorTheory
    type private Pth = PosixFileInfoTheory

    [<RequireQualifiedAccess>]
    module Predicates = begin

        type CanWrite =
            struct
                static member Judge (arg: inref<t>): bool = Fth.CanWrite(arg)

                interface IPredicatePremise<t> with
                    member _.Judge (arg: inref<t>): bool = CanWrite.Judge(&arg)
            end

        type CanRead =
            struct
                static member Judge (arg: inref<t>): bool = Fth.CanRead(arg)

                interface IPredicatePremise<t> with
                    member _.Judge (arg: inref<t>): bool = CanRead.Judge(&arg)
            end

        type IsClosed =
            struct
                static member Judge (arg: inref<t>): bool = Fth.IsClosed(arg)

                interface IPredicatePremise<t> with
                    member _.Judge (arg: inref<t>): bool = IsClosed.Judge(&arg)
            end
    end

    module P = Predicates

    let tryRefineToCanWrite x = tryRefineV<_,P.CanWrite> x

    let tryRefineToCanRead x = tryRefineV<_,P.CanRead> x

    let tryRefineToIsClosed x = tryRefineV<_,P.IsClosed> x

    let private trustMe x : FileDescriptor = x |> ValueOption.ofNullable |> ValueOption.get

    let stdout = Pth.GetOut().FileDiscriptorOrNull |> trustMe

    let stdin = Pth.GetIn().FileDiscriptorOrNull |> trustMe

    let stderr = Pth.GetError().FileDiscriptorOrNull |> trustMe

    let toStream (x: t) = 
        let ok, fi = FileDescriptorTheory.TryToPosixFileInfo(x) in
        if ok && fi.HasStream then 
            fi.Stream
        else
            Stream.Null

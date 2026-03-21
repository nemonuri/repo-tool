namespace Nemonuri.OCamlDotNet.Primitives

open System.IO
open Nemonuri.Posix
open Nemonuri.PureTypeSystems.Refiners.Bound
open Nemonuri.PureTypeSystems.Primitives
open type Nemonuri.PureTypeSystems.Primitives.JudgementTheory

module FileDiscriptors =

    type private t = FileDescriptor
    type private Fth = FileDescriptorTheory
    type private Pth = PosixFileInfoTheory

    [<RequireQualifiedAccess>]
    module Judges = begin

        type CanWrite =
            struct
                static member Judge (arg: inref<t>) = Fth.CanWrite(arg) |> FromBoolean

                interface IJudgePremise<t> with
                    member _.Judge arg = CanWrite.Judge(&arg)
            end

        type CanRead =
            struct
                static member Judge (arg: inref<t>) = Fth.CanRead(arg) |> FromBoolean

                interface IJudgePremise<t> with
                    member _.Judge arg = CanRead.Judge(&arg)
            end

        type IsClosed =
            struct
                static member Judge (arg: inref<t>) = Fth.IsClosed(arg) |> FromBoolean

                interface IJudgePremise<t> with
                    member _.Judge (arg: inref<t>) = IsClosed.Judge(&arg)
            end
    end

    module J = Judges

    let tryRefineToCanWrite x = tryRefineV<_,J.CanWrite> x

    let tryRefineToCanRead x = tryRefineV<_,J.CanRead> x

    let tryRefineToIsClosed x = tryRefineV<_,J.IsClosed> x

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

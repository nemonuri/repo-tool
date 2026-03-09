
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Platform.Base.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/ml/FStarC_Platform_Base.ml
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Platform.fsti
// reference: https://github.com/FStarLang/FStar/blob/v2025.12.15/src/basic/FStarC.Platform.fst

namespace Nemonuri.FStarDotNet.FStarC

open Nemonuri.OCamlDotNet.Forwarded
open Nemonuri.FStarDotNet
open Nemonuri.FStarDotNet.FStarOperators
open Nemonuri.FStarDotNet.FStarC.Effect
open Nemonuri.FStarDotNet.FStarC.Class.Show

module Platform =

    module Base =

        type sys =
        | Unix
        | Win32
        | Cygwin

        /// val system : sys
        let system =
            match Sys.os_type with
            | (ToString "Unix"B) -> Unix
            | (ToString "Win32"B) -> Win32
            | (ToString "Cygwin"B) -> Cygwin
            | s -> failwith ((toString "Unrecognized system: "B) ^. s)
        
        (* Tries to read the output of the `uname` command. *)
        /// val kernel () : string
        let kernel () : Prims.string = Sys.os_type
    
    open Base

    type showable_sys =
        struct
            interface showable<sys> with
                member _.show x = 
                    match x with
                    | Unix -> (toString "Unix"B)
                    | Win32 -> (toString "Win32"B)
                    | Cygwin -> (toString "Cygwin"B)
        end

    (* Running on Windows (not cygwin) *)
    /// val windows : bool
    let windows = system =. Win32

    (* Running on Cygwin. *)
    /// val cygwin : bool
    let cygwin = system =. Cygwin

    (* Running on a unix-like system *)
    /// val unix : bool
    let unix = system =. Unix

    (* Executable name for this platform, currently
    just appends '.exe' on Windows. *)
    /// val exe : string -> string
    let exe s =
        if windows then s ^. (toString ".exe"B) else s
    
    (* String used to separate paths in the OCAMLPATH environment variable. *)
    /// val ocamlpath_sep : string
    let ocamlpath_sep =
        if windows then (toString ";"B) else (toString ":"B)
    


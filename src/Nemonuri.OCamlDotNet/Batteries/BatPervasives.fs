
namespace Nemonuri.OCamlDotNet.Batteries

// reference: https://ocaml-batteries-team.github.io/batteries-included/hdoc2/BatPervasives.html
module BatPervasives =

    /// finally fend f x calls f x and then fend() even if f x raised an exception.
    let ``finally`` (fend: unit -> unit) (f: 'a -> 'b) (x: 'a) : 'b =
        try
            f x
        finally
            fend()
    
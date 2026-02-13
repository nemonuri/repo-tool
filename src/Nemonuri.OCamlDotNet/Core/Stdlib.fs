
/// - Reference: https://ocaml.org/manual/5.4/api/Stdlib.html
module Nemonuri.OCamlDotNet.Stdlib
open Nemonuri.OCamlDotNet

exception Invalid_argument = Forward.Invalid_argument

exception Division_by_zero = Forward.Division_by_zero

exception Failure = Forward.Failure

let raise = Microsoft.FSharp.Core.Operators.raise

let (|Invalid_argument|_|) = Forward.(|Invalid_argument|_|)

let failwith (message: string) = Forward.failwith message

let invalid_arg (message: string) = Forward.invalid_arg message


//// <category name="String conversion functions">

let string_of_bool (b: bool) : string = Bool.to_string b

//// <category/>


//// <category name="References">

/// The type of references (mutable indirection cells) containing a value of type 'a.
type 'a ref = Microsoft.FSharp.Core.ref<'a>

/// Return a fresh reference containing the given value.
let ref = Microsoft.FSharp.Core.Operators.ref

/// !r returns the current contents of reference r. Equivalent to fun r -> r.contents. Unary operator, see Ocaml_operators for more information.
let (!) (r : 'a ref) = r.Value

/// r := a stores the value of a in reference r. Equivalent to fun r v -> r.contents <- v. Right-associative operator, see Ocaml_operators for more information.
let (:=) (r : 'a ref) a = r.Value <- a

//// <category/>


//// <category name="Program termination">

/// Terminate the process, returning the given status code to the operating system: usually 0 to indicate no errors, and a small positive integer to indicate failure. All open output channels are flushed with flush_all. The callbacks registered with Domain.at_exit are called followed by those registered with at_exit.
/// 
/// An implicit exit 0 is performed each time a program terminates normally. An implicit exit 2 is performed if the program terminates early because of an uncaught exception.
let exit = Microsoft.FSharp.Core.Operators.exit

//// <category/>

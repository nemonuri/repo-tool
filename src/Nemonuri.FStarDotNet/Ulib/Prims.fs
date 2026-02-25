(**
### Reference

- https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/Prims.fst
- https://github.com/FStarLang/FStar/blob/v2025.12.15/ulib/ml/app/Prims.ml
*)

namespace Nemonuri.FStarDotNet

(*
   Copyright 2008-2020 Microsoft Research

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*)

/// This module is implicitly opened in the scope of all other modules.
///
/// It provides the very basic primitives on which F* is
/// built, including the definition of total functions, the basic
/// logical connectives, the PURE and GHOST effects and the like.
///
/// While some of the primitives have logical significance, others are
/// define various conveniences in the language, e.g., type of
/// attributes.
[<RequireQualifiedAccess>]
module Prims =

    open System
    open TypeEquality
    open Nemonuri.FStarDotNet.Primitives
    open Nemonuri.FStarDotNet.Primitives.Abbreviations
    open System.Collections
    open System.Collections.Generic 
    module Fv = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarValues
    module Ftc = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarTypeContexts
    module Ff = Nemonuri.FStarDotNet.Primitives.Abstractions.FStarFunctions
    module Uc = Microsoft.FSharp.Core.Operators.Unchecked
    module Fty = Nemonuri.FStarDotNet.Primitives.FStarTypes
    type private Fot = Nemonuri.FStarDotNet.Primitives.FStarObjectType


    (***** Begin trusted primitives *****)

    (** Primitives up to the definition of the GTot effect are trusted
        Beyond that all definitions are fully verified *)


    (** Type of attributes *)
    type attribute = System.Attribute

    (** An attribute indicating that some definition must be processed by the
        Dijkstra monads for free construction *)
    [<AttributeUsage(AttributeTargets.All)>]
    type cps() = inherit attribute()

    (** This attribute marks definitions for logical connectives that should
        not be unfolded during tactics. *)
    [<AttributeUsage(AttributeTargets.All)>]
    type tac_opaque() = inherit attribute()

    (** This attribute is added to all projectors. *)
    [<AttributeUsage(AttributeTargets.All)>]
    type projector() = inherit attribute()

    (** This attribute is added to all discriminators. *)
    [<AttributeUsage(AttributeTargets.All)>]
    type discriminator() = inherit attribute()

    (** This attribute can be used on type binders to make unifier attempt
        to unrefine them before instantiating them. This is useful in polymorphic
        definitions where the type does not change the result type, for example
        eq2 below. Using the attribute, an equality between two nats will happen
        at type int, which is more canonical.

        This feature is experimental and only enabled with "--ext __unrefine" *)
    [<AttributeUsage(AttributeTargets.All)>]
    type unrefine() = inherit attribute()

    (** This attribute can be attached to a type definition to partly counter the
        behavior of the `unrefine` attribute. It will cause the definition marked
        `do_not_unrefine` to not be unfolded during the unrefining process. *)
    [<AttributeUsage(AttributeTargets.All)>]
    type do_not_unrefine() = inherit attribute()

    type Type = tc

    type Type0 = Abstractions.IFStarObjectType


    (** A predicate to express when a type supports decidable equality
        The type-checker emits axioms for [hasEq] for each inductive type *)
    [<AbstractClass>]
    type hasEq<'Type 
                when 'Type :> eterm<Type0, 'Type>
                and 'Type : equality> =
        class
            interface Type0 with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Fot.Tail
                member this.GetWitness (d: outref<objnull>) = d <- Fot.Witness
        end


    (** A convenient abbreviation, [eqtype] is the type of types in
        universe 0 which support decidable equality *)
    [<FStarTypeProxy(typedefof<eqtype<_>>)>]
    [<Interface>]
    type eqtype = inherit Type0
    
    and [<FStarTypeProxy(typedefof<IFStarEquatableProxy<_>>)>]
        eqtype<'TTerm 
                when 'TTerm :> eqtype<'TTerm> 
                and 'TTerm : equality> =
        eterm<Type0, 'TTerm>

    and IFStarEquatableProxy<'TTerm 
                                when 'TTerm :> IFStarEquatableProxy<'TTerm> 
                                and 'TTerm : equality> =
        interface
            inherit eqtype<'TTerm>
            inherit refine<hasEq<'TTerm>>
        end



    (** [bool] is a two element type with elements [true] and [false]. We
        assume it is primitive, for convenient interop with other
        languages, although it could easily be defined as an inductive type
        with two cases, [BTrue | BFalse] *)
    type bool = EqType<Core.bool>

    type BTrue = 
        struct
            interface eterm<Type0, BTrue, bool> with
                member this.GetValue (d: outref<BTrue>) = d <- Uc.defaultof<BTrue>
                member this.Embed (d: outref<bool>) = d <- bool.create true
                member this.GetTailTypeContext (d: outref<Type0>) = d <- Ftc.tail this
                member this.GetWitness (d: outref<BTrue>) = d <- Fv.value this
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
                member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
            interface Abstractions.IEmbeddable with
                member this.Embed (d: outref<objnull>) = d <- Fv.embed this |> box
        end

    type BFalse = 
        struct
            interface eterm<Type0, BFalse, bool> with
                member this.GetValue (d: outref<BFalse>) = d <- Uc.defaultof<BFalse>
                member this.Embed (d: outref<bool>) = d <- bool.create true
                member this.GetTailTypeContext (d: outref<Type0>) = d <- Ftc.tail this
                member this.GetWitness (d: outref<BFalse>) = d <- Fv.value this
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
                member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
            interface Abstractions.IEmbeddable with
                member this.Embed (d: outref<objnull>) = d <- Fv.embed this |> box
        end

    (** [empty] is the empty inductive type. The type with no
        inhabitants represents logical falsehood. Note, [empty] is
        seldom used directly in F*. We instead use its "squashed" variant,
        [False], see below. *)
    [<AbstractClass>]
    type empty() =
        do Fty.raiseInvalid typeof<empty>

        abstract member GetTailTypeContext: outref<objnull> -> unit
        abstract member GetWitness: outref<objnull> -> unit

        interface Type with
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- this.GetTailTypeContext()
            member this.GetWitness (d: outref<objnull>): unit = d <- this.GetWitness()

    (** [trivial] is the singleton inductive type---it is trivially
        inhabited. Like [empty], [trivial] is seldom used. We instead use
        its "squashed" variants, [True] *)
    [<RequireQualifiedAccess>]
    [<Struct>]
    type trivial = | T with
        interface Type with
            member this.GetTailTypeContext (d: outref<objnull>): unit = d <- Fot.Tail
            member this.GetWitness (d: outref<objnull>): unit = d <- Fot.Witness


    (** [unit]: another singleton type, with its only inhabitant written [()]
        we assume it is primitive, for convenient interop with other languages *)
    type unit = EqType<Core.unit>

    (** [squash p] is a central type in F*---[squash p] is the proof
        irrelevant analog of [p] and is represented as a unit
        refinement. Squashed proofs are typically discharged using an SMT
        solver, without any proof terms explicitly reconstructed. As
        such, one way to think of [squash p] is as the type of properties
        proven using classical axioms without building proof terms.

        Note, [squash p] is just a unit refinement, it resides in universe
        0, lowering the universe of [p]. From this perspective, one may
        also see [squash] as a coercion down to universe 0.

        The type is marked [tac_opaque] to indicate to Meta-F* that
        instances of [squash] should not be unfolded when evaluating
        tactics (since many optimizations in F*'s SMT encoding rely
        specifically on occurrences of [squash].

        See FStar.Squash for various ways of manipulating squashed
        types. *)
    [<tac_opaque>]
    [<Interface>]
    type squash<'p when 'p :> Type> =
        inherit refine<'p>
        inherit Type0

    (** [auto_squash] is equivalent to [squash]. However, F* will
        automatically insert `auto_squash` when simplifying terms,
        converting terms of the form `p /\ True` to `auto_squash p`.

        We distinguish these automatically inserted squashes from explicit,
        user-written squashes.

        A user should not have to manipulate [auto_squash] at all, except
        in rare circumstances when writing tactics to process proofs that
        have already been partially simplified by F*'s simplifier.
    *)
    type auto_squash<'p when 'p :> Type> = squash<'p>

    (** The [logical] type is transitionary. It is just an abbreviation
        for [Type0], but is used to classify uses of the basic squashed
        logical connectives that follow. Some day, we plan to remove the
        [logical] type, replacing it with [prop] (also defined below).

        The type is marked [private] to intentionally prevent user code
        from referencing this type, hopefully easing the removal of
        [logical] in the future. *)
    type private logical = Type0

    (** An attribute indicating that a symbol is an smt theory symbol and
        hence may not be used in smt patterns.  The typechecker warns if
        such symbols are used in patterns *)
    type smt_theory_symbol() = inherit attribute()

    (** [l_True] has a special bit of syntactic sugar. It is written just
        as "True" and rendered in the ide as [True]. It is a squashed version
        of constructive truth, [trivial]. *)
    [<tac_opaque; smt_theory_symbol>]
    type l_True = squash<trivial>

    (** [l_False] has a special bit of syntactic sugar. It is written just
        as "False" and rendered in the ide as [False]. It is a squashed version
        of constructive falsehood, the empty type. *)
    [<tac_opaque; smt_theory_symbol>]
    type l_False = squash<empty>


    (** The type of provable equalities, defined as the usual inductive
        type with a single constructor for reflexivity.  As with the other
        connectives, we often work instead with the squashed version of
        equality, below. *)

    [<Struct>]
    [<FStarTypeProxy(typedefof<FStarEqualsProxy<_,_>>)>]
    type equals<'a, 'x, '_0 when 'a :> Type and 'x :> thunk<'a> and '_0 :> thunk<'a>> = | Refl 
        with
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Fot.Tail
                member this.GetWitness (d: outref<objnull>) = d <- Fot.Witness
        end

    and FStarEqualsProxy<'a, 'x when 'a :> Type and 'x :> thunk<'a>> = equals<'a, 'x, 'x>

    (** [eq2] is the squashed version of [equals]. It's a proof
        irrelevant, homogeneous equality in Type#0 and is written with
        an infix binary [==].

        TODO: instead of hard-wiring the == syntax,
            we should just rename eq2 to op_Equals_Equals
    *)
    [<tac_opaque; smt_theory_symbol>]
    type eq2<[<unrefine>] 'a, 'x, 'y when 'a :> Type and 'x :> thunk<'a> and 'y :> thunk<'a>> = squash<equals<'a, 'x, 'y>>

    (** bool-to-type coercion: This is often automatically inserted type,
        when using a boolean in context expecting a type. But,
        occasionally, one may have to write [b2t] explicitly *)
    [<FStarTypeProxy(typedefof<FStarEqualsProxy<_,_>>)>]
    type b2t<'b when 'b :> eterm<Type0, 'b, bool> and 'b : unmanaged> = squash<eq2<Type0, 'b, BTrue>>

    and [<AbstractClass>]
        FStarB2tProxy<'b when 'b :> eterm<Type0, 'b, bool> and 'b : unmanaged>() =
            class
                do
                    match
                        let e1 = Fv.embed Uc.defaultof<'b> in
                        let e2 = Fv.embed Uc.defaultof<BTrue> in
                        e1 = e2
                    with
                    | true -> ()
                    | false -> Fty.raiseInvalid typeof<b2t<'b>>
            end

    (** constructive conjunction *)
    [<Struct>]
    type pair<'p, 'q> = | Pair of _1: 'p * _2: 'q 
        with
            interface eterm<Type0, pair<'p, 'q>, ('p * 'q)> with
                member this.Embed (d: outref<'p * 'q>) = d <- match this with | Pair(_1, _2) -> _1, _2
                member this.GetTailTypeContext (d: outref<Type0>) = d <- Fot()
                member this.GetValue (d: outref<pair<'p,'q>>) = d <- this
                member this.GetWitness (d: outref<pair<'p,'q>>) = d <- Fv.value this
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
                member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
            interface Abstractions.IEmbeddable with
                member this.Embed (d: outref<objnull>) = d <- Fv.embed this |> box
        end
            
    (** squashed conjunction, specialized to [Type0], written with an
        infix binary [/\] *)
    [<tac_opaque; smt_theory_symbol>]
    type l_and<'p, 'q when 'p :> logical and 'q :> logical> = squash<pair<'p, 'q>>

    (** constructive disjunction *)
    [<Struct>]
    type sum<'p,'q> =
        | Left of vl: 'p
        | Right of vr: 'q
        with
            interface eterm<Type0, sum<'p,'q>, sum<'p,'q>> with
                member this.Embed (d: outref<sum<'p,'q>>) = d <- this
                member this.GetTailTypeContext (d: outref<Type0>) = d <- Fot()
                member this.GetValue (d: outref<sum<'p,'q>>) = d <- this
                member this.GetWitness (d: outref<sum<'p,'q>>) = d <- Fv.value this
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
                member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
            interface Abstractions.IEmbeddable with
                member this.Embed (d: outref<objnull>) = d <- Fv.embed this |> box
        end

    (** squashed disjunction, specialized to [Type0], written with an
        infix binary [\/] *)
    [<tac_opaque; smt_theory_symbol>]
    type l_or<'p, 'q when 'p :> logical and 'q :> logical> = squash<sum<'p, 'q>>


    type ``->``<'p, 'q when 'p :> Type and 'q :> Type> = imp<'p, thunk<'p>, 'q>
    //pureFuncType<'p, 'q>
    //type ``>=>``<'p, 'q, 'x when 'p :> Type and 'q :> Type and 'x :> pureTerm<'p>> = pureFuncType<'x, 'q>

    [<RequireQualifiedAccess>]
    type ``->0``<'a, 'p when 'a :> Type and 'p :> ``->``<'a, Type0>> = { Value: 'p }
        with
            interface ``->``<'a, Type> with
                member this.Invoke (s: thunk<'a>, d: outref<Type>): Core.unit = d <- let ty0: Type0 = this.Value.Invoke(s) in ty0
                member this.GetTailTypeContext (d: outref<'a>) = d <- Ftc.tail this.Value
                member this.GetWitness (d: outref<(thunk<'a> -> Type)>) = d <- Ff.toArrow (this :> ``->``<'a, Type>)
            interface Type with
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.tail this |> box
                member this.GetWitness (d: outref<objnull>) = d <- Ftc.witness this |> box
        end

    (** squashed (non-dependent) implication, specialized to [Type0],
        written with an infix binary [==>]. Note, [==>] binds weaker than
        [/\] and [\/] *)
    [<tac_opaque; smt_theory_symbol>]
    type l_imp<'p, 'q when 'p :> logical and 'q :> logical> = squash<``->``<'p, 'q>>
    (* ^^^ NB: The GTot effect is primitive;            *)
    (*         elaborated using GHOST a few lines below *)

    (** squashed double implication, infix binary [<==>] *)
    [<smt_theory_symbol>]
    type l_iff<'p, 'q when 'p :> logical and 'q :> logical> = l_and<l_imp<'p, 'q> , l_imp<'q, 'p>>

    (** squashed negation, prefix unary [~] *)
    [<smt_theory_symbol>]
    type l_not<'p when 'p :> logical> = l_imp<'p, l_False>

    (** l_ITE is a weak form of if-then-else at the level of
        logical formulae. It's not much used.

        TODO: Can we remove it *)
    [<unfold>]
    type l_ITE<'p, 'q, 'r when 'p :> logical and 'q :> logical and 'r :> logical> = l_and<l_imp<'p, 'q> , l_imp<l_not<'p>, 'r>>

    (** One of the main axioms provided by prims is [precedes], a
        built-in well-founded partial order over all terms. It's typically
        written with an infix binary [<<].

        The [<<] order includes:
            * The [<] ordering on natural numbers
            * The subterm ordering on inductive types
            * [f x << D f] for data constructors D of an inductive t whose
                arguments include a ghost or total function returning a t *)
    [<Interface>]
    type precedes<'a, 'b, '_0, '_1 when 'a :> Type and 'b :> Type and '_0 :> thunk<'a> and '_1 :> thunk<'b>> = inherit Type0
        

    (** The type of primitive strings of characters; See FStar.String *)
    type string = EqType<Core.string>

    (** This attribute can be added to the declaration or definition of
        any top-level symbol. It causes F* to report a warning on any
        use of that symbol, printing the [msg] argument.
        
        This is used, for instance to:
        
        - tag every escape hatch, e.g., [assume], [admit], etc

        Reports for uses of symbols tagged with this attribute
        are controlled using the `--report_assumes` option
        and warning number 334. 
        
        See tests/micro-benchmarks/WarnOnUse.fst
    *)
    [<AttributeUsage(AttributeTargets.All)>]
    type warn_on_use (msg: string) = inherit Attribute()
        

    (** The [deprecated "s"] attribute: "s" is an alternative function
        that should be printed in the warning it can be omitted if the use
        case has no such function *)
    [<AttributeUsage(AttributeTargets.All)>]
    type deprecated (s: Core.string) = inherit Attribute()

    (** Within the SMT encoding, we have a relation [(HasType e t)]
        asserting that (the encoding of) [e] has a type corresponding to
        (the encoding of) [t].

        It is sometimes convenient, e.g., when writing triggers for
        quantifiers, to have access to this relation at the source
        level. The [has_type] predicate below reflects the SMT encodings
        [HasType] relation. We also use it to define the type [prop] or
        proof irrelevant propositions, below.

        Note, unless you have a really good reason, you probably don't
        want to use this [has_type] predicate. F*'s type theory certainly
        does not internalize its own typing judgment *)
    [<deprecated "'has_type' is intended for internal use and debugging purposes only; \
                    do not rely on it for your proofs">]
    [<Interface>]
    [<FStarTypeProxy(typedefof<FStarHasTypeProxy<_,_,_>>)>]
    type has_type<'a, '_0, 'Type when 'a :> Type and '_0 :> thunk<'a>> = inherit ``->``<'a, Type0>
        
    and [<AbstractClass>]
        FStarHasTypeProxy<'a, '_0, 'Type when 'a :> Type and '_0 :> thunk<'a>>() =
            do
                match
                    typeof<'Type>.IsAssignableFrom typeof<'_0>
                with
                | true -> ()
                | false -> Fty.raiseInvalid typeof<has_type<'a, '_0, 'Type>>
            
            abstract member GetTailTypeContext: outref<'a> -> Core.unit
            abstract member GetTailTypeContext: outref<objnull> -> Core.unit

            interface has_type<'a, '_0, 'Type> with
                member this.GetTailTypeContext (d: outref<'a>) = d <- Ftc.tail this
                member this.GetTailTypeContext (d: outref<objnull>) = d <- Ftc.Boxed.tail this
                member this.GetWitness (d: outref<(thunk<'a> -> Type0)>) = d <- Unchecked.defaultof<_>
                member this.GetWitness (d: outref<objnull>) = d <- Unchecked.defaultof<_>
                member this.Invoke (s: thunk<'a>, d: outref<Type0>) = d <- Unchecked.defaultof<_>
            

    (** Squashed universal quantification, or dependent products, written
        [forall (x:a). p x], specialized to Type0 *)
    [<tac_opaque; smt_theory_symbol>]
    [<Interface>]
    [<FStarTypeProxy(typedefof<FStarForallProxy<_,_,_>>)>]
    type l_Forall<'a, 'p
                    when 'a :> Type 
                    and 'p :> ``->``<'a, Type0>> = 
        inherit logical
    
    and FStarForallProxy<'a, 'p, 'x
                    when 'a :> Type 
                    and 'p :> ``->``<'a, Type0>
                    and 'x :> thunk<'a>> =
        squash<imp<'a, 'x, dvalue<'a, 'x, Type0, 'p, Type0>>>

    (** [p1 `subtype_of` p2] when every element of [p1] is also an element
        of [p2]. *)
    [<Interface>]
    [<FStarTypeProxy(typedefof<FStarSubtypeOfProxy<_,_,_>>)>]
    type subtype_of<'p1, 'p2 when 'p1 :> Type and 'p2 :> Type> = 
        inherit logical

    and FStarSubtypeOfProxy<'p1, 'p2, 'x when 'p1 :> Type and 'p2 :> Type and 'x :> thunk<'p1>> =
            FStarForallProxy<'p1, has_type<'p1, 'x, 'p2>, 'x>

    (** The type of squashed types.

        Note, the [prop] type is a work in progress in F*. In particular,
        we would like in the future to more systematically use [prop] for
        proof-irrelevant propositions throughout the libraries. However,
        we still use [Type0] in many places. 

        See https://github.com/FStarLang/FStar/issues/1048 for more
        details and the current status of the work.
        *)
    [<FStarTypeProxy(typedefof<IFStarPropProxy<_>>)>]
    [<Interface>]
    type prop = inherit term<Type0, unit>
        
    and IFStarPropProxy<'a when 'a :> IFStarPropProxy<'a>> =
        inherit prop
        inherit refine<subtype_of<'a, unit>>

    (**** The PURE effect *)

    (** The type of pure preconditions *)
    type pure_pre = Type0

    (** Pure postconditions, predicates on [a], on which the precondition
        [pre] is also valid. This provides a way for postcondition formula
        to be typed in a context where they can assume the validity of the
        precondition. This is discussed extensively in Issue #57 *)
    type pure_post'<'a, 'pre when 'pre :> Type and 'a :> Type and 'a :> refine<'pre>> = ``->``<'a, Type0>
    type pure_post<'a when 'a :> Type and 'a :> refine<l_True>> = pure_post'<'a, l_True>

    (** A pure weakest precondition transforms postconditions on [a]-typed
        results to pure preconditions

        We require the weakest preconditions to satisfy the monotonicity
        property over the postconditions
        To enforce it, we first define a vanilla wp type,
        and then refine it with the monotonicity condition *)
    type pure_wp'<'a when 'a :> Type and 'a :> refine<l_True>> = ``->``<pure_post<'a>, pure_pre>

    (** The monotonicity predicate is marked opaque_to_smt,
        meaning that its definition is hidden from the SMT solver,
        and if required, will need to be explicitly revealed
        This has the advantage that clients that do not need to work with it
        directly, don't have the (quantified) definition in their solver context *)
    [<MeasureAnnotatedAbbreviation>]
    type pure_wp_monotonic0<'a> = prop

    [<opaque_to_smt>]
    type pure_wp_monotonic<'a> = pure_wp_monotonic0<'a>

    [<MeasureAnnotatedAbbreviation>]
    type pure_wp<'a> = prop

    (** This predicate is an internal detail, used to optimize the
        encoding of some quantifiers to SMT by omitting their typing
        guards. This is safe to use only when the quantifier serves to
        introduce a local macro---use with caution. *)
    [<MeasureAnnotatedAbbreviation>]
    type guard_free<'a> = prop

    (** The return combinator for the PURE effect requires
        proving the postcondition only on [x]
        
        Clients should not use it directly,
        instead use FStar.Pervasives.pure_return *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_return0<'a, 'x, 'p when 'a :> Type and 'x :> thunk<'a>> = prop

    (** Sequential composition for the PURE effect

        Clients should not use it directly,
        instead use FStar.Pervasives.pure_bind_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_bind_wp0<'r1, 'a, 'b, 'wp1, 'wp2, 'p> = 'wp1

    (** Conditional composition for the PURE effect 

        The combinator is optimized to make use of how the typechecker generates VC
        for conditionals.

        The more intuitive form of the combinator would have been:
        [(p ==> wp_then post) /\ (~p ==> wp_else post)]

        However, the way the typechecker constructs the VC, [wp_then] is already
        weakened with [p].

        Hence, here we only weaken [wp_else]

        Clients should not use it directly,
        instead use FStar.Pervasives.pure_if_then_else *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_if_then_else0<'a,'p,'wp_then,'wp_else,'post> = prop

    (** Conditional composition for the PURE effect, while trying to avoid
        duplicating the postcondition by giving it a local name [k].

        Note the use of [guard_free] here: [k] is just meant to be a macro
        for [post].
            
        Clients should not use it directly,
        instead use FStar.Pervasives.pure_ite_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_ite_wp0<'a,'wp,'post> = prop

    (** Subsumption for the PURE effect *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_stronger<'a,'wp1,'wp2> = prop

    (** Closing a PURE WP under a binder for [b]
        
        Clients should not use it directly,
        instead use FStar.Pervasives.pure_close_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_close_wp0<'a,'b,'wp,'p> = prop

    (** Trivial WP for PURE: Prove the WP with the trivial postcondition *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_trivial<'a,'wp> = prop

#if false
    (** Introduces the PURE effect.
        The definition of the PURE effect is fixed.
        NO USER SHOULD EVER CHANGE THIS. *)
    total
    new_effect {
        PURE : a: Type -> wp: pure_wp a -> Effect
        with
        return_wp    = pure_return0
        ; bind_wp      = pure_bind_wp0
        ; if_then_else = pure_if_then_else0
        ; ite_wp       = pure_ite_wp0
        ; stronger     = pure_stronger
        ; close_wp     = pure_close_wp0
        ; trivial      = pure_trivial
    }

    (** [Pure] is a Hoare-style counterpart of [PURE]
        
        Note the type of post, which allows to assume the precondition
        for the well-formedness of the postcondition. c.f. #57 *)
    effect Pure (a: Type) (pre: pure_pre) (post: pure_post' a pre) =
        PURE a
        (fun (p: pure_post a) -> pre /\ (forall (pure_result: a). post pure_result ==> p pure_result))

    (** [Admit] is an effect abbreviation for a computation that
        disregards the verification condition of its continuation *)
    effect Admit (a: Type) = PURE a (fun (p: pure_post a) -> True)
#endif

    (** The primitive effect [Tot] is definitionally equal to an instance of [PURE] *)

    (** Clients should not use it directly, instead use FStar.Pervasives.pure_null_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type pure_null_wp0<'a,'p> = prop

#if false
    (** [Tot]: From here on, we have [Tot] as a defined symbol in F*. *)
    effect Tot (a: Type) = PURE a (pure_null_wp0 a)
#endif

    (** Clients should not use it directly, instead use FStar.Pervasives.pure_assert_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<opaque_to_smt>]
    [<unfold>]
    type pure_assert_wp0<'p,'post> = prop

    (** Clients should not use it directly, instead use FStar.Pervasives.pure_assume_wp *)
    [<MeasureAnnotatedAbbreviation>]
    [<opaque_to_smt>]
    [<unfold>]
    type pure_assume_wp0<'p, 'post> = prop

    (**** The [GHOST] effect *)

#if false
    (** [GHOST] is logically equivalent to [PURE], but distinguished from
        it nominally so that specific, computationally irrelevant
        operations, are provided only in [GHOST] and are erased during
        extraction *)
    total
    new_effect GHOST = PURE
#endif

    [<MeasureAnnotatedAbbreviation>]
    [<unfold>]
    type purewp_id<'a, 'wp> = 'wp

#if false
    (** [PURE] computations can be lifted to the [GHOST] effect (but not
        vice versa) using just the identity lifting on pure wps *)
    sub_effect PURE ~> GHOST { lift_wp = purewp_id }

    (** [Ghost] is a the Hoare-style counterpart of [GHOST] *)
    effect Ghost (a: Type) (pre: Type) (post: pure_post' a pre) =
        GHOST a
        (fun (p: pure_post a) -> pre /\ (forall (ghost_result: a). post ghost_result ==> p ghost_result)
        )

    (** As with [Tot], the primitive effect [GTot] is definitionally equal
        to an instance of GHOST *)
    effect GTot (a: Type) = GHOST a (pure_null_wp0 a)
#endif

    (***** End trusted primitives *****)

    (** This point onward, F* fully verifies all the definitions *)

    (** [===] heterogeneous equality *)
    let ( === ) (x: 'a) (y: 'b) : bool = 
        match 
            Teq.tryRefl<'a, 'b>.IsSome && x.Equals(y)
        with
        | true -> bool.create true
        | false -> bool.create false

    (** Dependent pairs [dtuple2] in concrete syntax is [x:a & b x].
        Its values can be constructed with the concrete syntax [(| x, y |)] *)
    [<unopteq>]
    [<RequireQualifiedAccess>]
    [<FStarTypeProxy(typedefof<FStarDTuple2Proxy<_,_,_,_>>)>]
    type dtuple2<'a, 'b 
                    when 'a :> Type 
                    and 'b :> ``->``<'a, Type>> = { b: 'b; _1: thunk<'a> }
        with
            member this._2 = let r: Type = this.b.Invoke(this._1) in r
        end

    and FStarDTuple2Proxy<'a, 'b, '_1, '_2
                            when 'a :> Type
                            and 'b :> ``->``<'a, Type>
                            and '_1 :> thunk<'a>
                            and '_2 :> dvalue<'a, '_1, Type, 'b, '_2>> =
        pair<'_1, '_2>

    let (|Mkdtuple2|) (dt: dtuple2<'a, 'b>) = Pair (dt._1, dt._2)


    (** Squashed existential quantification, or dependent sums,
        are written [exists (x:a). p x] : specialized to Type0 *)
    [<tac_opaque; smt_theory_symbol>]
    [<Interface>]
    [<FStarTypeProxy(typedefof<FStarExistsProxy<_,_,_,_>>)>]
    type l_Exists<'a, 'p when 'a :> Type and 'p :> ``->``<'a, Type0>> = inherit logical
        

    and FStarExistsProxy<'a, 'p, 'x, '``p x``
                            when 'a :> Type 
                            and 'p :> ``->``<'a, Type0>
                            and 'x :> thunk<'a>
                            and '``p x`` :> dvalue<'a, 'x, Type, ``->0``<'a,'p>, '``p x``>> =
        squash<FStarDTuple2Proxy<'a, ``->0``<'a,'p>, 'x, '``p x``>>
 

    (** Primitive type of mathematical integers, mapped to zarith in OCaml
        extraction and to the SMT sort of integers *)
    type int = EqType<Core.bigint>

    module Flv = Nemonuri.FStarDotNet.Primitives.FStarLiftedValues
    module Intr = Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

    (**** Basic operators on booleans and integers *)

    (** [&&] boolean conjunction *)

    [<smt_theory_symbol>]
    let op_AmpAmp x y = (Intr.(&&) |> Flv.map2) x y

    (** [||] boolean disjunction *)

    [<smt_theory_symbol>]
    let op_BarBar x y = (Intr.(||) |> Flv.map2) x y

    (** [not] boolean negation *)

    [<smt_theory_symbol>]
    let op_Negation x = (not |> Flv.map1) x

    (** Integer multiplication, no special symbol. See FStar.Mul *)

    [<smt_theory_symbol>]
    let op_Multiply x y = (bigint.Multiply |> Flv.curryMap2) x y

    (** [-] integer subtraction *)

    [<smt_theory_symbol>]
    let op_Subtraction x y = (bigint.Subtract |> Flv.curryMap2) x y

    (** [+] integer addition *)

    [<smt_theory_symbol>]
    let op_Addition x y = (bigint.Add |> Flv.curryMap2) x y

    (** [-] prefix unary integer negation *)

    [<smt_theory_symbol>]
    let op_Minus i = (bigint.Negate |> Flv.map1) i

    (** [<=] integer comparison *)

    let inline private tyFix (f: bigint -> bigint -> Core.bool) = f

    [<smt_theory_symbol>]
    let op_LessThanOrEqual x y = ((<=) |> tyFix |> Flv.map2) x y

    (** [>] integer comparison *)

    [<smt_theory_symbol>]
    let op_GreaterThan x y = ((>) |> tyFix |> Flv.map2) x y

    (** [>=] integer comparison *)

    [<smt_theory_symbol>]
    let op_GreaterThanOrEqual x y = ((>=) |> tyFix |> Flv.map2) x y

    (** [<] integer comparison *)

    [<smt_theory_symbol>]
    let op_LessThan x y = ((<) |> tyFix |> Flv.map2) x y

    (** [=] decidable equality on [eqtype] *)

    let inline private eq' (x: 'a) (y: 'a) = x = y

    [<smt_theory_symbol>]
    let inline op_Equality<[<unrefine>] 'a when 'a :> eqtype<'a> and 'a : equality> (x: 'a) (y: 'a) = (=) x y |> bool.create

    (** [<>] decidable dis-equality on [eqtype] *)

    [<smt_theory_symbol>]
    let op_disEquality<[<unrefine>] 'a when 'a :> eqtype<'a> and 'a : equality> (x: 'a) (y: 'a) = (<>) x y |> bool.create

    (** The extensible open inductive type of exceptions *)
    type exn = FStarLiftedValue<Core.exn>

    (** String concatenation and its abbreviation as [^].  TODO, both
        should be removed in favor of what is present in FStar.String *)
    let strcat (s1: string) (s2: string) : string = Flv.map2 (fun (t1: Core.string) (t2: Core.string) -> System.String.Concat(t1, t2)) s1 s2
    
    [<inline_for_extraction; unfold>]
    let op_Hat s1 s2 = strcat s1 s2

    (** The inductive type of polymorphic lists *)
    type list<'a> = FStarLiftedValue<Microsoft.FSharp.Collections.list<'a>>

    let (|Nil|Cons|) (l: list<'a>) =
        match Fv.embed l with
        | [] -> Nil l
        | hd::tl -> Cons (Flv.lift hd, Flv.lift tl)

#if false
    (** The [M] marker is interpreted by the Dijkstra Monads for Free
        construction. It has a "double meaning", either as an alias for
        reasoning about the direct definitions, or as a marker for places
        where a CPS transformation should happen. *)
    effect M (a: Type) = Tot a (attributes cps)
#endif

    (** Returning a value into the [M] effect *)
    let returnM<'a> (x: 'a) : 'a = x

#if false
    (** [as_requires] turns a WP into a precondition, by applying it to
        a trivial postcondition *)
    [<unfold>]
    let as_requires (#a: Type) (wp: pure_wp a) : pure_pre = wp (fun x -> True)

    (** [as_ensures] turns a WP into a postcondition, relying on a kind of
        double negation translation. *)
    unfold
    let as_ensures (#a: Type) (wp: pure_wp a) : pure_post a = fun (x:a) -> ~(wp (fun y -> (y =!= x)))

    (** The keyword term-level keyword [assume] is desugared to [_assume].
        It explicitly provides an escape hatch to assume a given property
        [p]. *)
    [@@ warn_on_use "Uses an axiom"]
    let _assume (p: Type) : Pure unit (requires (True)) (ensures (fun x -> p))

    (** [admit] is another escape hatch: It discards the continuation and
        returns a value of any type *)
    [@@ warn_on_use "Uses an axiom"]
    let admit: #a: Type -> unit -> Admit a

    (** [magic] is another escape hatch: It retains the continuation but
        returns a value of any type *)
    [@@ warn_on_use "Uses an axiom"]
    let magic: #a: Type -> unit -> Tot a

    (** [unsafe_coerce] is another escape hatch: It coerces an [a] to a
        [b].  *)
    [@@ warn_on_use "Uses an axiom"]
    irreducible
    let unsafe_coerce (#a #b: Type) (x: a) : b =
        admit ();
        x

    (** [admitP]: TODO: Unused ... remove? *)
    [@@ warn_on_use "Uses an axiom"]
    let admitP (p: Type) : Pure unit True (fun x -> p)

    (** The keyword term-level keyword [assert] is desugared to [_assert].
        It force a proof of a property [p], then assuming [p] for the
        continuation. *)
    val _assert (p: Type) : Pure unit (requires p) (ensures (fun x -> p))
    let _assert p = ()

    (** Logically equivalent to assert; TODO remove? *)
    val cut (p: Type) : Pure unit (requires p) (fun x -> p)
    let cut p = ()

    (** The type of non-negative integers *)
    type nat = i: int{i >= 0}

    (** The type of positive integers *)
    type pos = i: int{i > 0}

    (** The type of non-zero integers *)
    type nonzero = i: int{i <> 0}

    /// Arbitrary precision ints are compiled to zarith (big_ints) in
    /// OCaml and to .NET BigInteger in F#. Both the modulus and division
    /// operations are Euclidean and are mapped to the corresponding
    /// theory symbols in the SMT encoding

    (** Euclidean modulus *)

    [<smt_theory_symbol>]
    let op_Modulus: int -> nonzero -> Tot int

    (** Euclidean division, written [/] *)

    [<smt_theory_symbol>]
    let op_Division: int -> nonzero -> Tot int

    (** [pow2 x] is [2^x]:

        TODO: maybe move this to FStar.Int *)
    let rec pow2 (x: nat) : Tot pos =
        match x with
        | 0 -> 1
        | _ -> 2 `op_Multiply` (pow2 (x - 1))

    (** [min] computes the minimum of two [int]s *)
    let min x y = if x <= y then x else y

    (** [abs] computes the absolute value of an [int] *)
    let abs (x: int) : Tot int = if x >= 0 then x else - x

    (** A primitive printer for booleans:

        TODO: unnecessary, this could easily be defined *)
    let string_of_bool: bool -> Tot string

    (** A primitive printer for [int] *)
    let string_of_int: int -> Tot string

    (** THIS IS MEANT TO BE KEPT IN SYNC WITH FStar.CheckedFiles.fs
        Incrementing this forces all .checked files to be invalidated *)
    irreducible
    let __cache_version_number__ = 77
#endif
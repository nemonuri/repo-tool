namespace Nemonuri.FStarDotNet.Primitives

module Functionals =

    (** 
        - Reference: https://fsprojects.github.io/FSharpPlus/abstraction-misc.html
     *)

    type IInvariantFunctorPremise<'TSource, 'TTarget> =
        interface
            abstract member ConvertTo: 'TSource -> 'TTarget
            abstract member ConvertFrom: 'TTarget -> 'TSource
        end
    
    [<CompiledName("InvariantFunctorPattern`3")>]
    let (|InvariantFunctor|) (premise: #IInvariantFunctorPremise<'s,'t>) = premise.ConvertTo, premise.ConvertFrom
        
    type InvariantFunctorPremise<'TSource, 'TTarget>(convertTo: 'TSource -> 'TTarget, convertFrom: 'TTarget -> 'TSource) =
        struct
            interface IInvariantFunctorPremise<'TSource, 'TTarget> with
                member _.ConvertTo (arg: 'TSource): 'TTarget = convertTo arg
                member _.ConvertFrom (arg: 'TTarget): 'TSource = convertFrom arg
        end
    
    let invMap (premise: #IInvariantFunctorPremise<'s1,'t>) (f2: 's2 -> 's1) (g2: 's1 -> 's2) =
        match premise with
        | InvariantFunctor (f, g) -> InvariantFunctorPremise<_,_>(f << f2, g2 << g)

    type ILiftPremise =
        interface
            abstract member Lift: 'TSource -> 'TTarget
        end

(*
    type IBijectionPremise =
        interface
            inherit ILiftPremise
            abstract member Embed: 'TTarget -> 'TSource
        end
*)

(*
    type IBijectionPremise<'TPremise when 'TPremise :> IBijectionPremise<'TPremise>> =
        interface
            abstract member Lift: 'TSource -> DependentTypeProxy<'TTypeContext, 'TSource>
            abstract member Embed: DependentTypeProxy<'TTypeContext, 'TSource> -> 'TSource
        end
*)


(*
    let inline map (bij: #IBijectionPremise) (f: 'TSource1 -> 'TSource2) (x: 'TTarget1) = struct (bij, bij.Embed x |> f)
         
    let inline intro (bij: #IBijectionPremise) (f: 'TSource1 -> 'TSource2) = map bij f
*)
    

    let inline toLazy (f: 'heads -> 'last) (x: 'heads) (_: unit) = f x

    let inline flip (f: 'a -> 'b -> 'c) (b: 'b) (a: 'a) = f a b

(*
    let inline map (bij: #IBijectionPremise) (f: 'TSource1 -> 'TSource2 -> 'TSource3) (x: 'TTarget1) = 
        let fTail = bij.Embed x |> f in
        fun (x2: 'TTarget2) -> bij.Embed x2 |> fTail
*)

(*
    let inline intro (bij: #IBijectionPremise) (f: 's1 -> 's2 -> 's3) (t1: 't1) (t2: 't2) =
        let fTail = bij.Embed t1 |> f in
        match Teq.tryRefl<'t2, 's2> with
        | None -> bij.Embed t2 |> fTail
        | Some teq -> Teq.cast teq t2 |> fTail
*)

(*
    let inline apply (bij: #IBijection) (t1: 't1) (f: 's1 -> 's2) =
        match Teq.tryRefl<'t1, 's1> with
        | None -> bij.Embed t1 |> f
        | Some teq -> Teq.cast teq t1 |> f
*)
(*


    let inline intro2 (bij: #IBijectionPremise) (f: 's1 -> 's2 -> 's3) (t1: 't1) (t2: 't2) =
        let fTail = intro bij f t1 in
        intro bij fTail t2
*)
    
(*
    let private dft<'a when 'a : unmanaged> = Unchecked.defaultof<'a>

    [<Struct>]
    [<RequireQualifiedAccess>]
    type Bijectioned<'b, 's1, 's2 when 'b :> IBijectionPremise> = { Bijection: 'b; Function: 's1 -> 's2 } 
        with
            static member create (bij: 'b) (f: 's1 -> 's2) = { Bijection = bij; Function = f }

            static member op_Implicit (self: Bijectioned<'b, 's1, 's2>) : 't1 -> Bijectioned<'b, 's2, 's3> =
                fun t1 -> let r: 's2 = apply self.Bijection t1 self.Function in 
                
        end
    
    let intro (bij: #IBijectionPremise) (f: 's1 -> 's2) = Bijectioned<_,_,_>.create bij f
*)


    type IBijection<'TContext when 'TContext :> IBijection<'TContext>> =
        interface
            abstract member Lift<'TSource, 'TTarget when 'TTarget :> IContexted<'TContext, 'TSource>> : 'TSource -> 'TTarget
        end
    
    and IContexted<'TContext, 'TSource when 'TContext :> IBijection<'TContext>> =
        interface
            abstract member Witness: 'TSource
        end


    [<Struct>]
    [<RequireQualifiedAccess>]
    type BijectionMonad<'TBijection when 'TBijection :> IBijection<'TBijection> and 'TBijection : unmanaged> = 
        struct
            member inline this.Bind<'s1, 't1, 's2, 't2 
                                        when 't1 :> IContexted<'TBijection, 's1>
                                        and 't2 :> IContexted<'TBijection, 's2>>
                                        (x: 't1, f: 's1 -> 't2) : 't2 =
                                        x.Witness |> f
            
            member inline this.Return<'s1, 't1
                                        when 't1 :> IContexted<'TBijection, 's1>>
                                        (x: 's1) : 't1 =
                                        Unchecked.defaultof<'TBijection>.Lift<'s1, 't1>(x)
        end

    [<Struct>]
    [<RequireQualifiedAccess>]
    type TargetBox<'TSource> = { Value: obj }
    
    type ITargetUnboxer<'TSource> = 
        interface 
            abstract member Unbox: TargetBox<'TSource> * outref<obj> -> unit
        end



module Tests =

    open Functionals

    type Bijection1 =
        struct
            interface IBijection<Bijection1> with
                member this.Lift (arg: 'TSource): 'TTarget = 
                    { Bijection1Contexted.Value = System.ValueTuple.Create(arg) } |> unbox
        end
    
    and 
        [<Struct>]
        [<RequireQualifiedAccess>]
        Bijection1Contexted<'TSource> = internal { Value: System.ValueTuple<'TSource> }
        with
            interface IContexted<Bijection1, 'TSource> with
                member this.Witness with get () = this.Value.Item1
        end

    let add4 a b c d = a + b + c + d


    let add4M a b c d =
        BijectionMonad<Bijection1>() {
                let! a2 = a in
                let! b2 = b in
                let! c2 = c in
                let! d2 = d in
                return add4 a2 b2 c2 d2 
            }

module Tests2 =

    type BindReturn =
        struct
            member this.Bind(x: System.ValueTuple<'a>, f: 'a -> 'b) = x.Item1 |> f //|> System.ValueTuple.Create
            member this.Return(x: 'a) : System.ValueTuple<'a> = System.ValueTuple.Create(x)
        end
    
    let add4 a b c d = a + b + c + d

    let add4M a b c d =
        BindReturn() {
                let! a2 = a in
                let! b2 = b in
                let! c2 = c in
                let! d2 = d in
                return add4 a2 b2 c2 d2
            }


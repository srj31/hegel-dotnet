namespace Hegel

/// The built-in generators plus the combinators over `IGenerator<'T>`. This is
/// the idiomatic-F# replacement for the C# `Gen` static class and the LINQ
/// `Select`/`Where`/`SelectMany` extensions: factory functions, the functional
/// trio (`map`/`filter`/`bind`), and a `gen { }` computation expression for
/// sequencing dependent draws.
[<RequireQualifiedAccess>]
module Gen =

    /// 32-bit integers (chain `.Min(...).Max(...)` to bound).
    let integers () = IntegerGenerator()

    /// Booleans.
    let booleans () = BooleanGenerator()

    /// Always produces `value`.
    let just (value: 'T) : IGenerator<'T> = failwith "Not implemented"

    /// Picks uniformly from `values`.
    let sampledFrom (values: 'T seq) : IGenerator<'T> = failwith "Not implemented"

    /// Picks one of the given generators.
    let oneOf (options: IGenerator<'T> seq) : IGenerator<'T> = failwith "Not implemented"

    /// Lists of elements drawn from `elements`.
    let lists (elements: IGenerator<'T>) : IGenerator<'T list> = failwith "Not implemented"

    /// Transform each generated value.
    let map (f: 'T -> 'U) (source: IGenerator<'T>) : IGenerator<'U> = failwith "Not implemented"

    /// Keep only values matching `predicate`.
    let filter (predicate: 'T -> bool) (source: IGenerator<'T>) : IGenerator<'T> = failwith "Not implemented"

    /// Chain a dependent generator (monadic bind).
    let bind (f: 'T -> IGenerator<'U>) (source: IGenerator<'T>) : IGenerator<'U> = failwith "Not implemented"

    /// Computation-expression builder enabling `gen { let! x = … in … }`.
    type GenBuilder() =
        member _.Bind(source: IGenerator<'T>, f: 'T -> IGenerator<'U>) : IGenerator<'U> = bind f source
        member _.Return(value: 'T) : IGenerator<'T> = just value
        member _.ReturnFrom(source: IGenerator<'T>) : IGenerator<'T> = source

/// Exposes the `gen { }` computation expression on `open Hegel`.
[<AutoOpen>]
module GenBuilderInstance =
    /// The `gen { }` computation expression for sequencing dependent draws.
    let gen = Gen.GenBuilder()

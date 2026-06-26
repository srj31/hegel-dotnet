namespace Hegel

open System

/// `IGenerator<'T>` and `TestCase` reference each other (a generator draws from a
/// test case; a test case draws a generator), so F# requires them in a single
/// mutually-recursive `type … and …` group.
/// A generator of `'T` values, driven by a `TestCase`.
type IGenerator<'T> =
    /// Draw a value, pulling entropy from `testCase`.
    abstract member Draw: testCase: TestCase -> 'T

/// The handle a property body uses to draw values and steer a single test case.
/// Created by the engine for each generated case; not constructed directly.
and [<Sealed>] TestCase internal (source: IDataSource, reporting: bool) =

    /// True on the final replay of a minimal counterexample (when draws are printed).
    member internal _.Reporting = reporting

    /// Draw a value from `generator`.
    member _.Draw<'T>(generator: IGenerator<'T>) : 'T = failwith "Not implemented"

    /// Draw a value, naming it `label` in counterexample output.
    member _.Draw<'T>(generator: IGenerator<'T>, label: string) : 'T = failwith "Not implemented"

    /// Reject the current case unless `condition` holds.
    member _.Assume(condition: bool) : unit =
        if not condition then
            raise (AssumeRejected())

    /// Attach a note shown only when a failing case is replayed.
    member _.Note(message: string) : unit = failwith "Not implemented"

    /// Report a coverage-guided search score (optionally labelled).
    member _.Target(value: float, ?label: string) : unit =
        source.Target(value, defaultArg label null)

    // --- Engine primitives used by composite generators (same assembly only). ---
    member internal _.GenerateFromSchema(schema: ReadOnlyMemory<byte>) : byte[] = source.Generate schema
    member internal _.StartSpan(label: int64) : int64 = source.StartSpan label
    member internal _.StopSpan(discard: bool) : unit = source.StopSpan discard
    member internal _.NewCollection(minSize: int64, maxSize: int64) : int64 = source.NewCollection(minSize, maxSize)
    member internal _.CollectionMore(collectionId: int64) : bool = source.CollectionMore collectionId

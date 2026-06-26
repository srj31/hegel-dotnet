namespace Hegel

open System

/// An `IDataSource` backed by a live native test-case handle. Each call translates
/// to a libhegel ABI call and maps the return code to either a value, a
/// control-flow signal (`StopTest` / `AssumeRejected`), or a `HegelException`.
[<Sealed>]
type internal LiveDataSource(testCase: nativeint) =

    /// The native test-case handle this source draws from.
    member _.Handle = testCase

    interface IDataSource with
        member _.Generate(schema: ReadOnlyMemory<byte>) : byte[] = failwith "Not implemented"
        member _.StartSpan(label: int64) : int64 = failwith "Not implemented"
        member _.StopSpan(discard: bool) : unit = failwith "Not implemented"
        member _.NewCollection(minSize: int64, maxSize: int64) : int64 = failwith "Not implemented"
        member _.CollectionMore(collectionId: int64) : bool = failwith "Not implemented"
        member _.Target(value: float, label: string) : unit = failwith "Not implemented"

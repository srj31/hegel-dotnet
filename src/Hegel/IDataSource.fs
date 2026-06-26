namespace Hegel

open System

/// The seam between a `TestCase` and the engine. The default implementation binds
/// the native `libhegel`; abstracting it here keeps the door open for an alternate
/// transport (e.g. the subprocess protocol).
type IDataSource =
    /// Generate a value for the given CBOR-encoded schema; returns the CBOR-encoded value.
    abstract member Generate: schema: ReadOnlyMemory<byte> -> byte[]
    /// Open a structured-draw span with the given label; sub-draws nest inside it.
    abstract member StartSpan: label: int64 -> int64
    /// Close the most recently opened span. `discard` drops a rejected attempt.
    abstract member StopSpan: discard: bool -> unit
    /// Begin a variable-length collection draw; returns its id.
    abstract member NewCollection: minSize: int64 * maxSize: int64 -> int64
    /// Ask whether the collection should draw another element.
    abstract member CollectionMore: collectionId: int64 -> bool
    /// Report a coverage-guided search score.
    abstract member Target: value: float * label: string -> unit

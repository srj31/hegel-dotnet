namespace Hegel.Interop

/// Constants from the libhegel C ABI (hegel-rust's `hegel.h`). These must match
/// the native library exactly; they are ported verbatim from the reference
/// clients. See `docs/ARCHITECTURE.md` §3.
module internal Abi =

    /// Result codes returned by the per-test-case draw primitives (`hegel_error_t`).
    /// Declared as literals so they can be used directly in `match` patterns.
    [<RequireQualifiedAccess>]
    module Error =
        [<Literal>]
        let Ok = 0

        [<Literal>]
        let StopTest = -1

        [<Literal>]
        let Assume = -2

        [<Literal>]
        let Backend = -3

        [<Literal>]
        let InvalidHandle = -4

        [<Literal>]
        let InvalidArg = -5

        [<Literal>]
        let AlreadyComplete = -6

        [<Literal>]
        let NotComplete = -7

        [<Literal>]
        let Internal = -8

    /// Status passed to `hegel_mark_complete` (`hegel_status_t`).
    type Status =
        | Valid = 0
        | Invalid = 1
        | Overrun = 2
        | Interesting = 3

    /// Span labels (`hegel_span_label`), 1–17, identifying structured draws.
    type SpanLabel =
        | List = 1L
        | ListElement = 2L
        | Set = 3L
        | SetElement = 4L
        | Map = 5L
        | MapEntry = 6L
        | Tuple = 7L
        | OneOf = 8L
        | Optional = 9L
        | FixedDict = 10L
        | FlatMap = 11L
        | Filter = 12L
        | Mapped = 13L
        | SampledFrom = 14L
        | EnumVariant = 15L
        | Stateful = 16L
        | Composite = 17L

    /// Sentinel for "no maximum collection size" (`UINT64_MAX`).
    let Unbounded: uint64 = System.UInt64.MaxValue

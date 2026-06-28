namespace Hegel.Interop

/// Constants from the libhegel C ABI (hegel-rust's `hegel.h`). These must match
/// the native library exactly; they are ported verbatim from the reference
/// clients. See `docs/ARCHITECTURE.md` §3.
module internal Abi =

    /// Result codes returned by C ABI entry points (`hegel_result_t`). An enum, so it
    /// is a blittable P/Invoke return type yet type-safe at call sites.
    type Result =
        | Ok = 0
        | StopTest = -1
        | Assume = -2
        | Backend = -3
        | InvalidHandle = -4
        | InvalidArg = -5
        | AlreadyComplete = -6
        | NotComplete = -7
        | Internal = -8

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

namespace Hegel

open System

/// An engine, configuration, or native-binding error — distinct from a property
/// *failure* (which surfaces as the user's own assertion exception).
type HegelException =
    inherit Exception

    new() = { inherit Exception() }
    new(message: string) = { inherit Exception(message) }
    new(message: string, innerException: Exception) = { inherit Exception(message, innerException) }

/// Raised when a `HealthCheck` aborts a run.
[<Sealed>]
type HealthCheckFailure =
    inherit HegelException

    new() = { inherit HegelException() }
    new(message: string) = { inherit HegelException(message) }
    new(message: string, innerException: Exception) = { inherit HegelException(message, innerException) }

/// Control-flow signal: the current test case's assumptions were rejected (via
/// `TestCase.Assume` or a filter), so it does not count.
[<Sealed>]
type AssumeRejected() =
    inherit Exception("Assumption rejected.")

/// Control-flow signal: the engine abandoned the current test case (e.g. the
/// generation budget was exhausted).
[<Sealed>]
type StopTest() =
    inherit Exception("Test stopped by the engine.")

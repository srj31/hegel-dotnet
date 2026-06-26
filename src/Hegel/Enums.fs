namespace Hegel

open System

/// How much the engine prints while running.
type Verbosity =
    | Quiet = 0
    | Normal = 1
    | Verbose = 2
    | Debug = 3

/// Run mode.
type Mode =
    /// A full run: generate, target, shrink, replay.
    | TestRun = 0
    /// A single generated case with no shrinking, replay, or database.
    | SingleTestCase = 1

/// The phases of a run. Enable a subset to, e.g., generate without shrinking.
/// Values are a bitmask matching the C ABI.
[<Flags>]
type Phase =
    | None = 0
    /// Replay explicitly provided examples.
    | Explicit = 1
    /// Replay counterexamples persisted in the example database.
    | Reuse = 2
    /// Generate fresh test cases.
    | Generate = 4
    /// Hill-climb on `target()` scores (coverage-guided search).
    | Target = 8
    /// Minimize a failing example.
    | Shrink = 16
    /// All phases (`Explicit ||| Reuse ||| Generate ||| Target ||| Shrink`).
    | All = 31

/// Health checks that can be individually suppressed. Bitmask matching the C ABI.
[<Flags>]
type HealthCheck =
    | None = 0
    | FilterTooMuch = 1
    | TooSlow = 2
    | TestCasesTooLarge = 4
    | LargeInitialTestCase = 8

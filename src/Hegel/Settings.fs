namespace Hegel

/// Immutable configuration for a property run. Use `with`-expressions to derive
/// variants, e.g. `{ Settings.Default with TestCases = 1000L }`.
type Settings =
    {
        /// Number of test cases to run. Default 100.
        TestCases: int64
        /// Fixed RNG seed, or `None` to let the engine choose.
        Seed: int64 option
        /// Force deterministic generation, or `None` for the env default (on in CI).
        Derandomize: bool option
        /// Example-database configuration.
        Database: Database
        /// Health checks to suppress.
        SuppressHealthCheck: HealthCheck
        /// The phases to run, or `None` for all phases.
        Phases: Phase option
        /// Engine output verbosity.
        Verbosity: Verbosity
        /// Run mode.
        Mode: Mode
        /// Report every distinct failure instead of just the first.
        ReportMultipleFailures: bool
        /// Stable name used to derive the database key; defaults to the test method name.
        Name: string option
    }

    /// The default settings.
    static member Default =
        { TestCases = 100L
          Seed = None
          Derandomize = None
          Database = Database.Unset
          SuppressHealthCheck = HealthCheck.None
          Phases = None
          Verbosity = Verbosity.Normal
          Mode = Mode.TestRun
          ReportMultipleFailures = false
          Name = None }
